import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str

# 目标菜单：name, path, icon, parentId, controller, sortOrder
menus = [
    ('客户管理', '/home/customer-management', 'User', 0, 'CustomerManagement', 7),
    ('商品管理', '/home/product-management', 'Goods', 0, 'ProductManagement', 8),
    ('订单管理', '/home/order-management', 'List', 0, 'OrderManagement', 9),
]

for name, path, icon, parent_id, controller, sort_order in menus:
    existing = c.execute("SELECT Id FROM Menus WHERE Controller=? OR Name=?", (controller, name)).fetchone()
    if existing:
        mid = existing[0]
        # 修正 path/icon/controller/sortOrder
        c.execute("UPDATE Menus SET Path=?, Icon=?, Controller=?, SortOrder=? WHERE Id=?", (path, icon, controller, sort_order, mid))
        print(f"[已存在并修正] {name} (Id={mid})")
    else:
        c.execute(
            "INSERT INTO Menus (Name, Path, Icon, ParentId, Controller, SortOrder, CreatedAt) VALUES (?,?,?,?,?,?, datetime('now'))",
            (name, path, icon, parent_id, controller, sort_order)
        )
        mid = c.execute("SELECT last_insert_rowid()").fetchone()[0]
        print(f"[插入] {name} (Id={mid})")

    # 绑定给所有角色
    roles = c.execute("SELECT Id FROM Roles").fetchall()
    for (rid,) in roles:
        bound = c.execute("SELECT COUNT(*) FROM RoleMenus WHERE RoleId=? AND MenuId=?", (rid, mid)).fetchone()[0]
        if bound == 0:
            c.execute("INSERT INTO RoleMenus (RoleId, MenuId) VALUES (?,?)", (rid, mid))
            print(f"  绑定角色Id={rid}")

c.commit()
c.close()
print("完成")
