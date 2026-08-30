using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignDomainSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductDetails_ProductDetailId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Products_ProductId",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductDetailId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductDetails");

            migrationBuilder.RenameTable(
                name: "ProductDetails",
                newName: "ProductVariants");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                newName: "OrderItems");

            migrationBuilder.RenameTable(
                name: "OrderTypes",
                newName: "PaymentProviders");

            migrationBuilder.RenameColumn(
                name: "ProductDetailId",
                table: "OrderItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameColumn(
                name: "LineTotal",
                table: "OrderItems",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "PaymentProviders",
                newName: "ProviderName");

            migrationBuilder.RenameColumn(
                name: "DeliveryFee",
                table: "Orders",
                newName: "DeliveryCost");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductVariants",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE oi
                SET ProductId = pv.ProductId,
                    ProductName = p.Name
                FROM OrderItems oi
                INNER JOIN ProductVariants pv ON oi.ProductVariantId = pv.Id
                INNER JOIN Products p ON pv.ProductId = p.Id;
                """);

            migrationBuilder.Sql("""
                UPDATE OrderItems
                SET ProductId = '00000000-0000-0000-0000-000000000000'
                WHERE ProductId IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PaymentProviders",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProviderName",
                value: "CashOnDelivery");

            migrationBuilder.InsertData(
                table: "PaymentProviders",
                columns: new[] { "Id", "ProviderName" },
                values: new object[,]
                {
                    { 2, "bKash" },
                    { 3, "Nagad" },
                    { 4, "CreditCard" },
                    { 5, "DebitCard" },
                    { 6, "BankTransfer" }
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderTypeId = table.Column<int>(type: "int", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentProviders_ProviderTypeId",
                        column: x => x.ProviderTypeId,
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviders_ProviderName",
                table: "PaymentProviders",
                column: "ProviderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProviderTypeId",
                table: "Payments",
                column: "ProviderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionReference",
                table: "Payments",
                column: "TransactionReference");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Products_ProductId",
                table: "ProductVariants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Products_ProductId",
                table: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_PaymentProviders_ProviderName",
                table: "PaymentProviders");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants");

            migrationBuilder.DeleteData(
                table: "PaymentProviders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PaymentProviders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PaymentProviders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PaymentProviders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PaymentProviders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippedAt",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "ProductVariants",
                newName: "ProductDetails");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderDetails");

            migrationBuilder.RenameTable(
                name: "PaymentProviders",
                newName: "OrderTypes");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "OrderDetails",
                newName: "ProductDetailId");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "OrderDetails",
                newName: "LineTotal");

            migrationBuilder.RenameColumn(
                name: "ProviderName",
                table: "OrderTypes",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "DeliveryCost",
                table: "Orders",
                newName: "DeliveryFee");

            migrationBuilder.UpdateData(
                table: "OrderTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: "Standard");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductDetailId",
                table: "OrderDetails",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Products_ProductId",
                table: "ProductDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductDetails_ProductDetailId",
                table: "OrderDetails",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
