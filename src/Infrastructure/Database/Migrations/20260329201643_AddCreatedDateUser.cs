using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddCreatedDateUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "language_id",
            schema: "public",
            table: "users",
            newName: "language");

        migrationBuilder.AddColumn<DateTime>(
            name: "last_update_date",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "last_update_date",
            schema: "public",
            table: "users");

        migrationBuilder.RenameColumn(
            name: "language",
            schema: "public",
            table: "users",
            newName: "language_id");
    }
}
