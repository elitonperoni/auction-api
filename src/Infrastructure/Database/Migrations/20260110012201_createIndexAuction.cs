using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class createIndexAuction : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_bids_auction_id",
            schema: "public",
            table: "bids");

        migrationBuilder.CreateIndex(
            name: "ix_bids_auction_id_amount",
            schema: "public",
            table: "bids",
            columns: ["auction_id", "amount"],
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_bids_auction_id_amount",
            schema: "public",
            table: "bids");

        migrationBuilder.CreateIndex(
            name: "ix_bids_auction_id",
            schema: "public",
            table: "bids",
            column: "auction_id");
    }
}
