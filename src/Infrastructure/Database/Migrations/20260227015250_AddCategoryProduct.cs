using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddCategoryProduct : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "category_product",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_category_product", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_product_detail_category_product_id",
            schema: "public",
            table: "product_detail",
            column: "category_product_id");

        migrationBuilder.AddForeignKey(
            name: "fk_product_detail_category_product_category_product_id",
            schema: "public",
            table: "product_detail",
            column: "category_product_id",
            principalSchema: "public",
            principalTable: "category_product",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_product_detail_category_product_category_product_id",
            schema: "public",
            table: "product_detail");

        migrationBuilder.DropTable(
            name: "category_product",
            schema: "public");

        migrationBuilder.DropIndex(
            name: "ix_product_detail_category_product_id",
            schema: "public",
            table: "product_detail");
    }
}
