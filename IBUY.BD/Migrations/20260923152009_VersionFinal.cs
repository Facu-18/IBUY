using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBUY.BD.Migrations
{
    /// <inheritdoc />
    public partial class VersionFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemRemitos_Productos_ProductoId",
                table: "ItemRemitos");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemRemitos_Remitos_RemitoId",
                table: "ItemRemitos");

            migrationBuilder.DropForeignKey(
                name: "FK_Necesidades_Depositos_DepositoId",
                table: "Necesidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_Empresas_EmpresaId",
                table: "Remitos");

            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_Usuarios_UsuarioId",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "CantidadMinima",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "DepositoId",
                table: "Necesidades",
                newName: "DepositoSolicitanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Necesidades_DepositoId",
                table: "Necesidades",
                newName: "IX_Necesidades_DepositoSolicitanteId");

            migrationBuilder.AddColumn<int>(
                name: "DepositoDestinoId",
                table: "Necesidades",
                type: "int",
                nullable: true);

            // Backfill: para las necesidades existentes no habia un destino explicito;
            // se completa con cualquier deposito distinto del solicitante para no violar la FK.
            migrationBuilder.Sql(@"
                UPDATE n
                SET n.DepositoDestinoId = (SELECT TOP 1 d.Id FROM Depositos d WHERE d.Id <> n.DepositoSolicitanteId ORDER BY d.Id)
                FROM Necesidades n;");

            migrationBuilder.AlterColumn<int>(
                name: "DepositoDestinoId",
                table: "Necesidades",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "ItemRemitos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioResponsableId",
                table: "Depositos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Necesidades_DepositoDestinoId",
                table: "Necesidades",
                column: "DepositoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_UsuarioResponsableId",
                table: "Depositos",
                column: "UsuarioResponsableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Depositos_Usuarios_UsuarioResponsableId",
                table: "Depositos",
                column: "UsuarioResponsableId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRemitos_Productos_ProductoId",
                table: "ItemRemitos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRemitos_Remitos_RemitoId",
                table: "ItemRemitos",
                column: "RemitoId",
                principalTable: "Remitos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Necesidades_Depositos_DepositoDestinoId",
                table: "Necesidades",
                column: "DepositoDestinoId",
                principalTable: "Depositos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Necesidades_Depositos_DepositoSolicitanteId",
                table: "Necesidades",
                column: "DepositoSolicitanteId",
                principalTable: "Depositos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_Empresas_EmpresaId",
                table: "Remitos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_Usuarios_UsuarioId",
                table: "Remitos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depositos_Usuarios_UsuarioResponsableId",
                table: "Depositos");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemRemitos_Productos_ProductoId",
                table: "ItemRemitos");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemRemitos_Remitos_RemitoId",
                table: "ItemRemitos");

            migrationBuilder.DropForeignKey(
                name: "FK_Necesidades_Depositos_DepositoDestinoId",
                table: "Necesidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Necesidades_Depositos_DepositoSolicitanteId",
                table: "Necesidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_Empresas_EmpresaId",
                table: "Remitos");

            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_Usuarios_UsuarioId",
                table: "Remitos");

            migrationBuilder.DropIndex(
                name: "IX_Necesidades_DepositoDestinoId",
                table: "Necesidades");

            migrationBuilder.DropIndex(
                name: "IX_Depositos_UsuarioResponsableId",
                table: "Depositos");

            migrationBuilder.DropColumn(
                name: "DepositoDestinoId",
                table: "Necesidades");

            migrationBuilder.DropColumn(
                name: "UsuarioResponsableId",
                table: "Depositos");

            migrationBuilder.RenameColumn(
                name: "DepositoSolicitanteId",
                table: "Necesidades",
                newName: "DepositoId");

            migrationBuilder.RenameIndex(
                name: "IX_Necesidades_DepositoSolicitanteId",
                table: "Necesidades",
                newName: "IX_Necesidades_DepositoId");

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadMinima",
                table: "Stocks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "ItemRemitos",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRemitos_Productos_ProductoId",
                table: "ItemRemitos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRemitos_Remitos_RemitoId",
                table: "ItemRemitos",
                column: "RemitoId",
                principalTable: "Remitos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Necesidades_Depositos_DepositoId",
                table: "Necesidades",
                column: "DepositoId",
                principalTable: "Depositos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_Empresas_EmpresaId",
                table: "Remitos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_Usuarios_UsuarioId",
                table: "Remitos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
