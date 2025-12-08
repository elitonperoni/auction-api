using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class add_properties_auction : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_bids_user_id",
            schema: "public",
            table: "bids",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_bids_users_user_id",
            schema: "public",
            table: "bids",
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
            name: "fk_bids_users_user_id",
            schema: "public",
            table: "bids");

        migrationBuilder.DropIndex(
            name: "ix_bids_user_id",
            schema: "public",
            table: "bids");
    }
}
