using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBUY.BD.Migrations
{
    /// <inheritdoc />
    public partial class RolesVentasEncargado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Solo migra datos, no cambia el esquema. Todo usuario que no sea SuperAdmin queda como
            // EncargadoDeposito si tiene algún depósito a cargo y como Ventas si no. Esto alcanza
            // también a los roles de texto libre que había en la base (Administrador,
            // Administradora, Aprobadora, Comprador, Deposito, Vendedor, Vendedora).
            migrationBuilder.Sql(
                "UPDATE u SET Rol = CASE " +
                "WHEN EXISTS (SELECT 1 FROM Depositos d WHERE d.UsuarioResponsableId = u.Id) " +
                "THEN 'EncargadoDeposito' ELSE 'Ventas' END " +
                "FROM Usuarios u WHERE u.Rol <> 'SuperAdmin';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Los roles de texto libre originales no se pueden recuperar: se perdieron en el Up.
            // Para dejar la base usable, los dos roles nuevos vuelven a Administrador.
            migrationBuilder.Sql(
                "UPDATE Usuarios SET Rol = 'Administrador' WHERE Rol IN ('Ventas', 'EncargadoDeposito');");
        }
    }
}
