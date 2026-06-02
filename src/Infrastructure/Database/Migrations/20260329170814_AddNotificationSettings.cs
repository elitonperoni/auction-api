using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddNotificationSettings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "first_name",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "last_name",
            schema: "public",
            table: "users");

        migrationBuilder.AlterColumn<string>(
            name: "email",
            schema: "public",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<string>(
            name: "complete_name",
            schema: "public",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "language_id",
            schema: "public",
            table: "users",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "phone",
            schema: "public",
            table: "users",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "time_zone_id",
            schema: "public",
            table: "users",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "user_name",
            schema: "public",
            table: "users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "notification_type",
            schema: "public",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_notification_type", x => x.id));

        migrationBuilder.CreateTable(
            name: "user_notification",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                notification_type_id = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_notification", x => x.id);
                table.ForeignKey(
                    name: "fk_user_notification_notification_type_notification_type_id",
                    column: x => x.notification_type_id,
                    principalSchema: "public",
                    principalTable: "notification_type",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_notification_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "public",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_user_notification_notification_type_id",
            schema: "public",
            table: "user_notification",
            column: "notification_type_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_notification_user_id",
            schema: "public",
            table: "user_notification",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "user_notification",
            schema: "public");

        migrationBuilder.DropTable(
            name: "notification_type",
            schema: "public");

        migrationBuilder.DropColumn(
            name: "complete_name",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "language_id",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "phone",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "time_zone_id",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "user_name",
            schema: "public",
            table: "users");

        migrationBuilder.AlterColumn<string>(
            name: "email",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(100)",
            oldMaxLength: 100);

        migrationBuilder.AddColumn<string>(
            name: "first_name",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "last_name",
            schema: "public",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");
    }
}
