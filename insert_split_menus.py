import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str

menus = [
    ('分类管理', '/home/category-management', 'Files', 'CategoryManagement', 9),
    ('品牌管理', '/home/brand-management', 'Medal', 'BrandManagement', 10),
    ('属性管理', '/home/attr-management', 'MagicStick', 'AttrManagement', 11),
]

for name, path, icon, controller, sort_order in menus:
    existing = c.execute("SELECT Id FROM Menus WHERE Controller=? OR Name=?", (controller, name)).fetchone()
    if existing:
        mid = existing[0]
        c.execute("UPDATE Menus SET Path=?, Icon=?, Controller=?, SortOrder=? WHERE Id=?", (path, icon, controller, sort_order, mid))
        print(f"[已存在并修正] {name} (Id={mid})")
    else:
        c.execute(
            "INSERT INTO Menus (Name, Path, Icon, ParentId, Controller, SortOrder, CreatedAt) VALUES (?,?,?,0,?,?, datetime('now'))",
            (name, path, icon, controller, sort_order)
        )
        mid = c.execute("SELECT last_insert_rowid()").fetchone()[0]
        print(f"[插入] {name} (Id={mid})")

    roles = c.execute("SELECT Id FROM Roles").fetchall()
    for (rid,) in roles:
        bound = c.execute("SELECT COUNT(*) FROM RoleMenus WHERE RoleId=? AND MenuId=?", (rid, mid)).fetchone()[0]
        if bound == 0:
            c.execute("INSERT INTO RoleMenus (RoleId, MenuId) VALUES (?,?)", (rid, mid))
            print(f"  绑定角色Id={rid}")

c.commit()
c.close()
print("完成")
