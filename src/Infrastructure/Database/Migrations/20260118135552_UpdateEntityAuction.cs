using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class UpdateEntityAuction : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "current_price",
            schema: "public",
            table: "auctions",
            type: "numeric",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<Guid>(
            name: "last_bidder_id",
            schema: "public",
            table: "auctions",
            type: "uuid",
            nullable: true);

        //migrationBuilder.AddColumn<uint>(
        //    name: "xmin",
        //    schema: "public",
        //    table: "auctions",
        //    type: "xid",
        //    rowVersion: true,
        //    nullable: false,
        //    defaultValue: 0u);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "current_price",
            schema: "public",
            table: "auctions");

        migrationBuilder.DropColumn(
            name: "last_bidder_id",
            schema: "public",
            table: "auctions");

        //migrationBuilder.DropColumn(
        //    name: "xmin",
        //    schema: "public",
        //    table: "auctions");
    }
}
