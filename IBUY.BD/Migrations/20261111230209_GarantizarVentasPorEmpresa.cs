using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBUY.BD.Migrations
{
    /// <inheritdoc />
    public partial class GarantizarVentasPorEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Solo migra datos: la migracion RolesVentasEncargado pudo dejar una empresa sin
            // ningun usuario de Ventas si todos sus usuarios tenian depositos a cargo.
            // Por cada empresa sin un Ventas activo se promueve un usuario no SuperAdmin,
            // prefiriendo los activos y los que no tienen depositos asignados.
            migrationBuilder.Sql(
                ";WITH EmpresasSinVentas AS (" +
                "  SELECT e.Id FROM Empresas e " +
                "  WHERE NOT EXISTS (SELECT 1 FROM Usuarios v " +
                "                    WHERE v.EmpresaId = e.Id AND v.Rol = 'Ventas' AND v.Estado = 1)" +
                "), Candidatos AS (" +
                "  SELECT u.Id, ROW_NUMBER() OVER (PARTITION BY u.EmpresaId ORDER BY " +
                "    CASE WHEN EXISTS (SELECT 1 FROM Depositos d WHERE d.UsuarioResponsableId = u.Id) THEN 1 ELSE 0 END, " +
                "    CASE WHEN u.Estado = 1 THEN 0 ELSE 1 END, u.Id) AS Orden " +
                "  FROM Usuarios u INNER JOIN EmpresasSinVentas e ON e.Id = u.EmpresaId " +
                "  WHERE u.Rol <> 'SuperAdmin'" +
                ") " +
                "UPDATE u SET u.Rol = 'Ventas' " +
                "FROM Usuarios u INNER JOIN Candidatos c ON c.Id = u.Id AND c.Orden = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No se puede reconstruir con certeza el rol anterior de los usuarios promovidos.
        }
    }
}
