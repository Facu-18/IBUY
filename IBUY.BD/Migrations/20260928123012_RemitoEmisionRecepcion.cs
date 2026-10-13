using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBUY.BD.Migrations
{
    /// <inheritdoc />
    public partial class RemitoEmisionRecepcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRecepcion",
                table: "Remitos",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Remitos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            // Los remitos existentes ya impactaron el stock en ambos lados (el flujo previo
            // era de un solo paso), asi que quedan como Recibidos.
            migrationBuilder.Sql("UPDATE Remitos SET Estado = 'Recibido' WHERE Estado = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Remitos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRecepcion",
                table: "Remitos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
