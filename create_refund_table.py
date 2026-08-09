import sqlite3, os
db = os.path.join('DShop.WebApi', 'DShop.db')
c = sqlite3.connect(db)
c.text_factory = str

exists = c.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='RefundOrders'").fetchone()
if exists:
    print("RefundOrders 表已存在，跳过")
else:
    c.execute("""
    CREATE TABLE RefundOrders (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        OrderId INTEGER NOT NULL,
        OrderSn TEXT NOT NULL,
        CustomerId INTEGER NOT NULL,
        CustomerMobile TEXT,
        RefundType INTEGER NOT NULL,
        Reason TEXT,
        RefundAmount INTEGER NOT NULL,
        Status INTEGER NOT NULL,
        AuditorId INTEGER NOT NULL,
        AuditorName TEXT,
        AuditTime TEXT,
        AuditRemark TEXT,
        RefundTime TEXT,
        IsDeleted INTEGER NOT NULL,
        ModifiedBy INTEGER NOT NULL,
        ModifiedAt TEXT NOT NULL,
        CreatedBy INTEGER NOT NULL,
        CreatedAt TEXT NOT NULL
    )
    """)
    c.commit()
    print("已创建 RefundOrders 表")
c.close()
