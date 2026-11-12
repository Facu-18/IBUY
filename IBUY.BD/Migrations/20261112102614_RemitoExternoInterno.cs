using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBUY.BD.Migrations
{
    /// <inheritdoc />
    public partial class RemitoExternoInterno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmpresaOrigen",
                table: "Remitos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unidad",
                table: "ItemRemitos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            // Los remitos cargados con el modelo anterior pasan a los tipos nuevos.
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Externo' WHERE Tipo = 'Entrada'");
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Interno' WHERE Tipo = 'Transferencia'");

            // Los items viejos no tenian unidad: se completa con la mas neutra.
            migrationBuilder.Sql("UPDATE ItemRemitos SET Unidad = 'Unidades' WHERE Unidad = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Entrada' WHERE Tipo = 'Externo'");
            migrationBuilder.Sql("UPDATE Remitos SET Tipo = 'Transferencia' WHERE Tipo = 'Interno'");

            migrationBuilder.DropColumn(
                name: "EmpresaOrigen",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "ItemRemitos");
        }
    }
}
