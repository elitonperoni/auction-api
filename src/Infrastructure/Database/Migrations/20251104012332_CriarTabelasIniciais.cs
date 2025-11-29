using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class CriarTabelasIniciais : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "auctions",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                starting_price = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_auctions", x => x.id));

        migrationBuilder.CreateTable(
            name: "bids",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                auction_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<decimal>(type: "numeric", nullable: false),
                bid_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_bids", x => x.id);
                table.ForeignKey(
                    name: "fk_bids_auctions_auction_id",
                    column: x => x.auction_id,
                    principalSchema: "public",
                    principalTable: "auctions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_bids_auction_id",
            schema: "public",
            table: "bids",
            column: "auction_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "bids",
            schema: "public");

        migrationBuilder.DropTable(
            name: "auctions",
            schema: "public");
    }
}
