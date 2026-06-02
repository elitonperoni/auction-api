using Microsoft.EntityFrameworkCore.Migrations;
using SharedKernel.Enum;

namespace Infrastructure.Database.Migrations;

public partial class InsertIntegrationTypeValues : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($@"insert into public.notification_types (id, name) 
            values {(int)NotificationType.Telegram}");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        //No need drop
    }
}

