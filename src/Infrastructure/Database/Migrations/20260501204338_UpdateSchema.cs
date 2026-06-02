using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class UpdateSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_user_notification_notification_type_notification_type_id",
            schema: "public",
            table: "user_notification");

        migrationBuilder.DropForeignKey(
            name: "fk_user_notification_users_user_id",
            schema: "public",
            table: "user_notification");

        migrationBuilder.DropPrimaryKey(
            name: "pk_user_notification",
            schema: "public",
            table: "user_notification");

        migrationBuilder.DropPrimaryKey(
            name: "pk_notification_type",
            schema: "public",
            table: "notification_type");

        migrationBuilder.RenameTable(
            name: "user_notification",
            schema: "public",
            newName: "user_notifications",
            newSchema: "public");

        migrationBuilder.RenameTable(
            name: "notification_type",
            schema: "public",
            newName: "notification_types",
            newSchema: "public");

        migrationBuilder.RenameIndex(
            name: "ix_user_notification_user_id",
            schema: "public",
            table: "user_notifications",
            newName: "ix_user_notifications_user_id");

        migrationBuilder.RenameIndex(
            name: "ix_user_notification_notification_type_id",
            schema: "public",
            table: "user_notifications",
            newName: "ix_user_notifications_notification_type_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_user_notifications",
            schema: "public",
            table: "user_notifications",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_notification_types",
            schema: "public",
            table: "notification_types",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_user_notifications_notification_types_notification_type_id",
            schema: "public",
            table: "user_notifications",
            column: "notification_type_id",
            principalSchema: "public",
            principalTable: "notification_types",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_notifications_users_user_id",
            schema: "public",
            table: "user_notifications",
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
            name: "fk_user_notifications_notification_types_notification_type_id",
            schema: "public",
            table: "user_notifications");

        migrationBuilder.DropForeignKey(
            name: "fk_user_notifications_users_user_id",
            schema: "public",
            table: "user_notifications");

        migrationBuilder.DropPrimaryKey(
            name: "pk_user_notifications",
            schema: "public",
            table: "user_notifications");

        migrationBuilder.DropPrimaryKey(
            name: "pk_notification_types",
            schema: "public",
            table: "notification_types");

        migrationBuilder.RenameTable(
            name: "user_notifications",
            schema: "public",
            newName: "user_notification",
            newSchema: "public");

        migrationBuilder.RenameTable(
            name: "notification_types",
            schema: "public",
            newName: "notification_type",
            newSchema: "public");

        migrationBuilder.RenameIndex(
            name: "ix_user_notifications_user_id",
            schema: "public",
            table: "user_notification",
            newName: "ix_user_notification_user_id");

        migrationBuilder.RenameIndex(
            name: "ix_user_notifications_notification_type_id",
            schema: "public",
            table: "user_notification",
            newName: "ix_user_notification_notification_type_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_user_notification",
            schema: "public",
            table: "user_notification",
            column: "id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_notification_type",
            schema: "public",
            table: "notification_type",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_user_notification_notification_type_notification_type_id",
            schema: "public",
            table: "user_notification",
            column: "notification_type_id",
            principalSchema: "public",
            principalTable: "notification_type",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_user_notification_users_user_id",
            schema: "public",
            table: "user_notification",
            column: "user_id",
            principalSchema: "public",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
