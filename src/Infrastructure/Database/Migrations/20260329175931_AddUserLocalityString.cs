using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddUserLocalityString : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "time_zone_id",
            schema: "public",
            table: "users");

        migrationBuilder.AddColumn<string>(
            name: "time_zone",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "time_zone",
            schema: "public",
            table: "users");

        migrationBuilder.AddColumn<int>(
            name: "time_zone_id",
            schema: "public",
            table: "users",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }
}
