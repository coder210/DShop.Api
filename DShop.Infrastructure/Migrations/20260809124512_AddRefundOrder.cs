using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DShop.Infrastructure.Migrations
{
    public partial class AddRefundOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefundOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<long>(type: "INTEGER", nullable: false),
                    OrderSn = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CustomerId = table.Column<long>(type: "INTEGER", nullable: false),
                    CustomerMobile = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RefundType = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RefundAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AuditorId = table.Column<long>(type: "INTEGER", nullable: false),
                    AuditorName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AuditTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditRemark = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RefundTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundOrders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefundOrders");
        }
    }
}
