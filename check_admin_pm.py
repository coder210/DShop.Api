import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
print('admin(1) 是否含 product-management:categories (by code):')
print(c.execute("""SELECT 1 FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.PermissionCode='product-management:categories'""").fetchone())
print('admin(1) ProductManagement模块权限数:')
print(c.execute("""SELECT COUNT(*) FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.Module='ProductManagement'""").fetchone())
print('admin(1) 全部权限码(前20):')
for r in c.execute("""SELECT p.PermissionCode FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 LIMIT 20"""):
    print(' ', r[0])
c.close()
