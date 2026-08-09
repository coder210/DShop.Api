import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str

# 检查是否已存在同名菜单，避免重复插入
exists = c.execute("SELECT Id FROM Menus WHERE Controller='CustomerManagement'").fetchone()
if exists:
    print(f"菜单已存在 Id={exists[0]}，跳过插入")
    c.close()
else:
    menu = ('客户管理', '/home/customer-management', 'User', 0, 'CustomerManagement', 3, '2026-08-09 00:00:00')
    c.execute(
        "INSERT INTO Menus (Name, Path, Icon, ParentId, Controller, SortOrder, CreatedAt) VALUES (?,?,?,?,?,?,?)",
        menu
    )
    menu_id = c.execute("SELECT last_insert_rowid()").fetchone()[0]
    print(f"已插入菜单 客户管理 (Id={menu_id})")

    # 给所有角色绑定该菜单（便于测试）
    roles = c.execute("SELECT Id FROM Roles").fetchall()
    for (rid,) in roles:
        bound = c.execute("SELECT COUNT(*) FROM RoleMenus WHERE RoleId=? AND MenuId=?", (rid, menu_id)).fetchone()[0]
        if bound == 0:
            c.execute("INSERT INTO RoleMenus (RoleId, MenuId) VALUES (?,?)", (rid, menu_id))
            print(f"已给角色Id={rid} 绑定菜单Id={menu_id}")
    c.commit()
    c.close()
    print("完成")
