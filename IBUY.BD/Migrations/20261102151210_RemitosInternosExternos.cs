using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBUY.BD.Migrations
{
    /// <inheritdoc />
    public partial class RemitosInternosExternos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Salida ya no existe como tipo y no hay un destino correcto para convertirla:
            // si hay alguna fila, se aborta para no migrar datos en silencio.
            migrationBuilder.Sql(
                "IF EXISTS (SELECT 1 FROM Remitos WHERE Tipo = 'Salida') " +
                "THROW 50001, 'Existen remitos de tipo Salida: resolverlos manualmente antes de migrar.', 1;");

            migrationBuilder.AddColumn<string>(
                name: "OrigenExterno",
                table: "Remitos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // El valor por defecto deja a los items existentes con Unidad = 'unidad'.
            migrationBuilder.AddColumn<string>(
                name: "Unidad",
                table: "ItemRemitos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "unidad");

            // Transferencia pasa a Interno; Entrada pasa a Externo con origen "Sin especificar".
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Interno' WHERE Tipo = 'Transferencia';");
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Externo', OrigenExterno = 'Sin especificar' WHERE Tipo = 'Entrada';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Transferencia' WHERE Tipo = 'Interno';");
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Entrada' WHERE Tipo = 'Externo';");

            migrationBuilder.DropColumn(
                name: "OrigenExterno",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "ItemRemitos");
        }
    }
}
