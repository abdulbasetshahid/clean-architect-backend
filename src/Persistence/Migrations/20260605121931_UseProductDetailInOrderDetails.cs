using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseProductDetailInOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductDetailId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO ProductDetails
                    (Id, VariationName, Description, Price, InStock, ProductId, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate)
                SELECT
                    NEWID(),
                    p.Name,
                    NULL,
                    0,
                    1,
                    p.Id,
                    p.CreatedBy,
                    p.CreatedDate,
                    p.LastModifiedBy,
                    p.LastModifiedDate
                FROM Products p
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM ProductDetails pd
                    WHERE pd.ProductId = p.Id
                );
                """);

            migrationBuilder.Sql("""
                UPDATE od
                SET ProductDetailId = pd.Id
                FROM OrderDetails od
                INNER JOIN ProductDetails pd ON pd.Id = (
                    SELECT TOP(1) pd2.Id
                    FROM ProductDetails pd2
                    WHERE pd2.ProductId = od.ProductId
                    ORDER BY pd2.CreatedDate, pd2.Id
                );
                """);

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductDetailId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductDetailId",
                table: "OrderDetails",
                column: "ProductDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductDetails_ProductDetailId",
                table: "OrderDetails",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductDetails_ProductDetailId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<Guid>(
                table: "OrderDetails",
                name: "ProductId",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE od
                SET ProductId = pd.ProductId
                FROM OrderDetails od
                INNER JOIN ProductDetails pd ON pd.Id = od.ProductDetailId;
                """);

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductDetailId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductDetailId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
