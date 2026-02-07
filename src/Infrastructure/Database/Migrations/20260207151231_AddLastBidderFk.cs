using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddLastBidderFk : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_auctions_last_bidder_id",
            schema: "public",
            table: "auctions",
            column: "last_bidder_id");

        migrationBuilder.AddForeignKey(
            name: "fk_auctions_users_last_bidder_id",
            schema: "public",
            table: "auctions",
            column: "last_bidder_id",
            principalSchema: "public",
            principalTable: "users",
            principalColumn: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_auctions_users_last_bidder_id",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropIndex(
            name: "ix_auctions_last_bidder_id",
            schema: "public",
            table: "auctions");
    }
}
