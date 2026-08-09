import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
print('=== ProductManagement模块权限 ===')
for r in c.execute("SELECT Id, PermissionCode, Module, Client FROM Permissions WHERE Module='ProductManagement' ORDER BY PermissionCode"):
    print(r)
print('=== admin角色(1) product-management:categories ===')
print(c.execute("SELECT 1 FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.PermissionCode='product-management:categories'").fetchone())
print('=== admin角色(1) product-management:brands ===')
print(c.execute("SELECT 1 FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.PermissionCode='product-management:brands'").fetchone())
c.close()
