namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Estados posibles de una necesidad de reposición. Son los textos que se guardan en
    /// la base y los que usa el filtro del listado; viven acá para que el cliente no
    /// dependa del proyecto de entidades.
    /// </summary>
    public static class EstadosNecesidad
    {
        /// <summary>Faltante detectado, todavía sin nota de pedido.</summary>
        public const string Pendiente = "Pendiente";

        /// <summary>Ya tiene una nota de pedido generada.</summary>
        public const string EnNota = "En nota";

        /// <summary>El faltante ya se resolvió.</summary>
        public const string Cubierta = "Cubierta";

        /// <summary>Estados aceptados por la API, en el orden en que se muestran.</summary>
        public static readonly string[] Todos = [Pendiente, EnNota, Cubierta];

        public static bool EsValido(string? estado)
        {
            return estado is not null && Array.Exists(Todos, e => e == estado);
        }
    }
}
