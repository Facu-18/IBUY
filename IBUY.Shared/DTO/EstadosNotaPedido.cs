namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Estados posibles de una nota de pedido. Son los textos que se guardan en la base.
    /// </summary>
    public static class EstadosNotaPedido
    {
        /// <summary>Nota recién generada, todavía sin publicar.</summary>
        public const string Borrador = "Borrador";

        /// <summary>Nota publicada para recibir cotizaciones.</summary>
        public const string Publicada = "Publicada";

        /// <summary>Nota cerrada con una cotización elegida.</summary>
        public const string Cerrada = "Cerrada";

        /// <summary>Estados aceptados por la API, en el orden en que se muestran.</summary>
        public static readonly string[] Todos = [Borrador, Publicada, Cerrada];

        public static bool EsValido(string? estado)
        {
            return estado is not null && Array.Exists(Todos, e => e == estado);
        }
    }
}
