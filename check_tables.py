import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
rows = c.execute("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name").fetchall()
print('数据库表清单:')
for r in rows:
    print(' -', r[0])
c.close()
