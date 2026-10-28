using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryOptionId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Zone = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOptions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DeliveryOptions",
                columns: new[] { "Id", "Cost", "IsActive", "Name", "SortOrder", "Zone" },
                values: new object[,]
                {
                    { new Guid("8f3c1a2e-4b6d-4e91-9c07-1a2b3c4d5e6f"), 80m, true, "Inside Dhaka", 1, 0 },
                    { new Guid("9a4d2b3f-5c7e-4f02-ad18-2b3c4d5e6f70"), 150m, true, "Outside Dhaka", 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryOptionId",
                table: "Orders",
                column: "DeliveryOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOptions_IsActive",
                table: "DeliveryOptions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOptions_Zone",
                table: "DeliveryOptions",
                column: "Zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryOptions_DeliveryOptionId",
                table: "Orders",
                column: "DeliveryOptionId",
                principalTable: "DeliveryOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryOptions_DeliveryOptionId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DeliveryOptions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryOptionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryOptionId",
                table: "Orders");
        }
    }
}
