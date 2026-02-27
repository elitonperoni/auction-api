using Microsoft.EntityFrameworkCore.Migrations;
using SharedKernel.Enum;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class InsertProductDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@$"
        INSERT INTO public.condition_packaging (id, name) VALUES 
            ({(int)ConditionPackaging.ORIGINAL_INTACT}, '{nameof(ConditionPackaging.ORIGINAL_INTACT)}'),
            ({(int)ConditionPackaging.ORIGINAL_DAMAGED}, '{nameof(ConditionPackaging.ORIGINAL_DAMAGED)}'),
            ({(int)ConditionPackaging.REPACKAGED}, '{nameof(ConditionPackaging.REPACKAGED)}'),
            ({(int)ConditionPackaging.NO_PACKAGING}, '{nameof(ConditionPackaging.NO_PACKAGING)}')
        ");

        migrationBuilder.Sql(@$"
        INSERT INTO public.condition_product (id, name) VALUES 
            ({(int)ConditionProduct.NEW}, '{nameof(ConditionProduct.NEW)}'),
            ({(int)ConditionProduct.OPEN_BOX}, '{nameof(ConditionProduct.OPEN_BOX)}'),
            ({(int)ConditionProduct.LIKE_NEW}, '{nameof(ConditionProduct.LIKE_NEW)}'),
            ({(int)ConditionProduct.USED}, '{nameof(ConditionProduct.USED)}'),
            ({(int)ConditionProduct.INCOMPLETE}, '{nameof(ConditionProduct.INCOMPLETE)}'),
            ({(int)ConditionProduct.SALVAGE}, '{nameof(ConditionProduct.SALVAGE)}')
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@$"
        DELETE FROM public.condition_packaging WHERE id IN (
            {(int)ConditionPackaging.ORIGINAL_INTACT},
            {(int)ConditionPackaging.ORIGINAL_DAMAGED},
            {(int)ConditionPackaging.REPACKAGED},
            {(int)ConditionPackaging.NO_PACKAGING}
        )");

        migrationBuilder.Sql(@$"
         DELETE FROM public.condition_product WHERE id IN (
            {(int)ConditionProduct.NEW},
            {(int)ConditionProduct.OPEN_BOX},
            {(int)ConditionProduct.LIKE_NEW},
            {(int)ConditionProduct.USED},
            {(int)ConditionProduct.INCOMPLETE},
            {(int)ConditionProduct.SALVAGE}
        ");

    }
}
