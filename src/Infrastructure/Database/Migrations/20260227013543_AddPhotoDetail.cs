using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddPhotoDetail : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "description",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropColumn(
            name: "starting_price",
            schema: "public",
            table: "auctions");

        migrationBuilder.AddColumn<Guid>(
            name: "product_detail_id",
            schema: "public",
            table: "auctions",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateTable(
            name: "condition_packaging",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_condition_packaging", x => x.id));

        migrationBuilder.CreateTable(
            name: "condition_product",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_condition_product", x => x.id));

        migrationBuilder.CreateTable(
            name: "product_detail",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                starting_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                condition_product_id = table.Column<int>(type: "integer", nullable: false),
                condition_packaging_id = table.Column<int>(type: "integer", nullable: false),
                category_product_id = table.Column<int>(type: "integer", nullable: false),
                without_warranty = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_detail", x => x.id);
                table.ForeignKey(
                    name: "fk_product_detail_condition_packaging_condition_packaging_id",
                    column: x => x.condition_packaging_id,
                    principalSchema: "public",
                    principalTable: "condition_packaging",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_product_detail_condition_product_condition_product_id",
                    column: x => x.condition_product_id,
                    principalSchema: "public",
                    principalTable: "condition_product",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_auctions_product_detail_id",
            schema: "public",
            table: "auctions",
            column: "product_detail_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_detail_condition_packaging_id",
            schema: "public",
            table: "product_detail",
            column: "condition_packaging_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_detail_condition_product_id",
            schema: "public",
            table: "product_detail",
            column: "condition_product_id");

        migrationBuilder.AddForeignKey(
            name: "fk_auctions_product_detail_product_detail_id",
            schema: "public",
            table: "auctions",
            column: "product_detail_id",
            principalSchema: "public",
            principalTable: "product_detail",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_auctions_product_detail_product_detail_id",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropTable(
            name: "product_detail",
            schema: "public");

        migrationBuilder.DropTable(
            name: "condition_packaging",
            schema: "public");

        migrationBuilder.DropTable(
            name: "condition_product",
            schema: "public");

        migrationBuilder.DropIndex(
            name: "ix_auctions_product_detail_id",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropColumn(
            name: "product_detail_id",
            schema: "public",
            table: "auctions");

        migrationBuilder.AddColumn<string>(
            name: "description",
            schema: "public",
            table: "auctions",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<decimal>(
            name: "starting_price",
            schema: "public",
            table: "auctions",
            type: "numeric(18,4)",
            nullable: false,
            defaultValue: 0m);
    }
}
