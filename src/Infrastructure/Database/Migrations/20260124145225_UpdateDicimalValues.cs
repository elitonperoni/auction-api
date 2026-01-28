using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class UpdateDicimalValues : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "starting_price",
            schema: "public",
            table: "auctions",
            type: "numeric(18,4)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric");

        migrationBuilder.AlterColumn<decimal>(
            name: "current_price",
            schema: "public",
            table: "auctions",
            type: "numeric(18,4)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "starting_price",
            schema: "public",
            table: "auctions",
            type: "numeric",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,4)");

        migrationBuilder.AlterColumn<decimal>(
            name: "current_price",
            schema: "public",
            table: "auctions",
            type: "numeric",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,4)");
    }
}
