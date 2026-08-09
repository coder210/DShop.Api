import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
print('Categories:', c.execute("SELECT Id, ParentId, Name, IsDeleted FROM Categories").fetchall())
print('Brands:', c.execute("SELECT Id, Name, IsDeleted FROM Brands").fetchall())
print('Attrs:', c.execute("SELECT Id, CategoryId, Name, AttrType, Status, IsDeleted FROM Attrs").fetchall())
c.close()
