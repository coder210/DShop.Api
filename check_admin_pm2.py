import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
print('admin(1) 有 admin::product-management:categories:')
print(c.execute("""SELECT 1 FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.PermissionCode='admin::product-management:categories'""").fetchone())
print('admin(1) 有 admin::product-management:brands:')
print(c.execute("""SELECT 1 FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.PermissionCode='admin::product-management:brands'""").fetchone())
print('admin(1) 全部 ProductManagement 权限:')
for r in c.execute("""SELECT p.PermissionCode FROM RolePermissions rp JOIN Permissions p ON rp.PermissionId=p.Id WHERE rp.RoleId=1 AND p.Module='ProductManagement' ORDER BY p.PermissionCode"""):
    print(' ', r[0])
c.close()
