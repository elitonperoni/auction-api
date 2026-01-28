using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddRefreshToken : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "reset_token_expiry",
            schema: "public",
            table: "users",
            newName: "reset_password_expiry");

        migrationBuilder.RenameColumn(
            name: "reset_token",
            schema: "public",
            table: "users",
            newName: "reset_password_code");

        migrationBuilder.AddColumn<string>(
            name: "refresh_token",
            schema: "public",
            table: "users",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "refresh_token_expiry_time",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "refresh_token",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "refresh_token_expiry_time",
            schema: "public",
            table: "users");

        migrationBuilder.RenameColumn(
            name: "reset_password_expiry",
            schema: "public",
            table: "users",
            newName: "reset_token_expiry");

        migrationBuilder.RenameColumn(
            name: "reset_password_code",
            schema: "public",
            table: "users",
            newName: "reset_token");
    }
}
