using IBUY.Cliente.Estado;
using Microsoft.AspNetCore.Components.Routing;

namespace IBUY.Cliente.Layout
{
    /// <summary>
    /// Lista unica de secciones de la aplicacion. La usan el menu lateral y el inicio
    /// para que ambos muestren los mismos modulos y respeten los mismos permisos.
    /// </summary>
    public static class SeccionesNavegacion
    {
        public record Seccion(
            string Titulo,
            string Ruta,
            string Icono,
            string Descripcion,
            NavLinkMatch Coincidencia = NavLinkMatch.Prefix,
            bool SoloSuperAdmin = false,
            bool SoloGestionUsuarios = false,
            bool EnInicio = true);

        public record Grupo(string Titulo, Seccion[] Secciones);

        public static readonly Grupo[] Grupos =
        [
            new Grupo("Administracion",
            [
                new Seccion("Empresas", "empresas", "empresas",
                    "Empresas registradas en la plataforma.",
                    SoloSuperAdmin: true, EnInicio: false),
                new Seccion("Usuarios", "usuarios", "usuarios",
                    "Usuarios con acceso a la empresa y sus depositos a cargo.",
                    SoloGestionUsuarios: true, EnInicio: false),
                new Seccion("Depositos", "depositos", "depositos",
                    "Depositos y obras donde la empresa almacena su mercaderia."),
                new Seccion("Productos", "productos", "productos",
                    "Catalogo de productos de la empresa.")
            ]),
            new Grupo("Inventario",
            [
                new Seccion("Stock", "stock", "stock",
                    "Existencias por deposito, segun los depositos a tu cargo."),
                new Seccion("Necesidades", "necesidades", "necesidades",
                    "Faltantes detectados entre depositos de la empresa."),
                new Seccion("Remitos", "remitos", "remitos",
                    "Movimientos internos o externos y recepcion de mercaderia.")
            ]),
            new Grupo("Compras",
            [
                new Seccion("Notas de pedido", "notas-pedido", "notas",
                    "Notas generadas a partir de una necesidad."),
                new Seccion("Cotizaciones", "cotizaciones", "cotizaciones",
                    "Cotizaciones recibidas de proveedores sobre tus notas de pedido.")
            ])
        ];

        /// <summary>Indica si la sesion puede ver la seccion en el menu y en el inicio.</summary>
        public static bool EsVisible(Seccion seccion, EstadoSesion sesion)
        {
            return (!seccion.SoloSuperAdmin || sesion.EsSuperAdmin)
                && (!seccion.SoloGestionUsuarios || sesion.PuedeGestionarUsuarios);
        }

        /// <summary>Secciones que se muestran como tarjetas en el inicio.</summary>
        public static IEnumerable<Seccion> DeInicio(EstadoSesion sesion)
        {
            return Grupos
                .SelectMany(grupo => grupo.Secciones)
                .Where(seccion => seccion.EnInicio && EsVisible(seccion, sesion));
        }
    }
}
