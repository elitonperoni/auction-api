using Microsoft.EntityFrameworkCore.Migrations;
using SharedKernel.Enum;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class InsertCategoryProduc : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@$"
            INSERT INTO public.category_product (id, name) VALUES 
            ({(int)CategoryProduct.VEHICLES}, '{nameof(CategoryProduct.VEHICLES)}'),
            ({(int)CategoryProduct.ELECTRONICS}, '{nameof(CategoryProduct.ELECTRONICS)}'),
            ({(int)CategoryProduct.COMPUTING}, '{nameof(CategoryProduct.COMPUTING)}')");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@$"
            DELETE FROM public.category_product WHERE id IN 
            ({(int)CategoryProduct.VEHICLES}, {(int)CategoryProduct.ELECTRONICS}, {(int)CategoryProduct.COMPUTING})");
    }
}
