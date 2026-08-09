import sqlite3, os, sys
sys.stdout.reconfigure(encoding='utf-8')
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str
rows = c.execute("SELECT Id, Name, Logo, Status, IsDeleted FROM Brands").fetchall()
print('Brands:')
for r in rows:
    print('  ', r)
c.close()
