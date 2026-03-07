using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddRegionProductDetail : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "city",
            schema: "public",
            table: "product_detail",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "country",
            schema: "public",
            table: "product_detail",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "state",
            schema: "public",
            table: "product_detail",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "city",
            schema: "public",
            table: "product_detail");

        migrationBuilder.DropColumn(
            name: "country",
            schema: "public",
            table: "product_detail");

        migrationBuilder.DropColumn(
            name: "state",
            schema: "public",
            table: "product_detail");
    }
}
