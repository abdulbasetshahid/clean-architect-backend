using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignOrderAndProductSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO ProductDetails
                    (Id, VariationName, Description, Price, InStock, ProductId, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate)
                SELECT
                    NEWID(),
                    p.Name,
                    p.Description,
                    p.Price,
                    p.InStock,
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

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ShippingAmount",
                table: "Orders",
                newName: "DeliveryFee");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "DeliveryFee",
                table: "Orders",
                newName: "ShippingAmount");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InStock",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE p
                SET
                    p.Description = pd.Description,
                    p.Price = pd.Price,
                    p.InStock = pd.InStock
                FROM Products p
                INNER JOIN ProductDetails pd ON pd.Id = (
                    SELECT TOP(1) pd2.Id
                    FROM ProductDetails pd2
                    WHERE pd2.ProductId = p.Id
                    ORDER BY pd2.CreatedDate, pd2.Id
                );
                """);
        }
    }
}
