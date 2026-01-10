using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "reset_token",
            schema: "public",
            table: "users",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "reset_token_expiry",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "reset_token",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "reset_token_expiry",
            schema: "public",
            table: "users");
    }
}
