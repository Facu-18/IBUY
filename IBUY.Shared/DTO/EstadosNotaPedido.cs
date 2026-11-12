namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Estados posibles de una nota de pedido. Son los textos que se guardan en la base.
    /// </summary>
    public static class EstadosNotaPedido
    {
        /// <summary>Nota recién creada, todavía sin aprobar ni publicar.</summary>
        public const string Borrador = "Borrador";

        /// <summary>Nota aprobada por Ventas, lista para publicarse.</summary>
        public const string Aprobada = "Aprobada";

        /// <summary>Nota publicada para recibir cotizaciones.</summary>
        public const string Publicada = "Publicada";

        /// <summary>Nota cerrada con una cotización elegida.</summary>
        public const string Cerrada = "Cerrada";

        /// <summary>Nota anulada por la empresa compradora.</summary>
        public const string Cancelada = "Cancelada";

        /// <summary>Estados aceptados por la API, en el orden en que se muestran.</summary>
        public static readonly string[] Todos = [Borrador, Aprobada, Publicada, Cerrada, Cancelada];

        public static bool EsValido(string? estado)
        {
            return estado is not null && Array.Exists(Todos, e => e == estado);
        }
    }
}
