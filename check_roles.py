import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
print('Roles:', c.execute("SELECT Id, Code, Name FROM Roles").fetchall())
print('RolePermissions by Role:', list(c.execute("SELECT RoleId, COUNT(*) FROM RolePermissions GROUP BY RoleId").fetchall()))
c.close()
