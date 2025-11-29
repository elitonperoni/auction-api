using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class addUserBidFk : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "user_id",
            schema: "public",
            table: "auctions",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateIndex(
            name: "ix_auctions_user_id",
            schema: "public",
            table: "auctions",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_auctions_users_user_id",
            schema: "public",
            table: "auctions",
            column: "user_id",
            principalSchema: "public",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_auctions_users_user_id",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropIndex(
            name: "ix_auctions_user_id",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropColumn(
            name: "user_id",
            schema: "public",
            table: "auctions");
    }
}
