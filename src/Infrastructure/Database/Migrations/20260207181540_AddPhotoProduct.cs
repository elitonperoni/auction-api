using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddPhotoProduct : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_product_photo_auctions_auction_id",
            schema: "public",
            table: "product_photo");

        migrationBuilder.DropPrimaryKey(
            name: "pk_product_photo",
            schema: "public",
            table: "product_photo");

        migrationBuilder.RenameTable(
            name: "product_photo",
            schema: "public",
            newName: "product_photos",
            newSchema: "public");

        migrationBuilder.RenameIndex(
            name: "ix_product_photo_auction_id",
            schema: "public",
            table: "product_photos",
            newName: "ix_product_photos_auction_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_product_photos",
            schema: "public",
            table: "product_photos",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_product_photos_auctions_auction_id",
            schema: "public",
            table: "product_photos",
            column: "auction_id",
            principalSchema: "public",
            principalTable: "auctions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_product_photos_auctions_auction_id",
            schema: "public",
            table: "product_photos");

        migrationBuilder.DropPrimaryKey(
            name: "pk_product_photos",
            schema: "public",
            table: "product_photos");

        migrationBuilder.RenameTable(
            name: "product_photos",
            schema: "public",
            newName: "product_photo",
            newSchema: "public");

        migrationBuilder.RenameIndex(
            name: "ix_product_photos_auction_id",
            schema: "public",
            table: "product_photo",
            newName: "ix_product_photo_auction_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_product_photo",
            schema: "public",
            table: "product_photo",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_product_photo_auctions_auction_id",
            schema: "public",
            table: "product_photo",
            column: "auction_id",
            principalSchema: "public",
            principalTable: "auctions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
