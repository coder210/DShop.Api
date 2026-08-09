import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
print('Menus columns:', [r[1] for r in c.execute('PRAGMA table_info(Menus)')])
print('---')
for r in c.execute('SELECT * FROM Menus ORDER BY Id'):
    print(r)
c.close()
