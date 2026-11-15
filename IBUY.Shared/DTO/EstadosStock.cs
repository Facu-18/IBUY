namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Estado de una existencia según su cantidad actual y su cantidad mínima. Lo usan el
    /// servidor (listado) y el cliente (simulación de ajustes), así ambos clasifican igual.
    /// </summary>
    public static class EstadosStock
    {
        public const string Normal = "Normal";
        public const string Bajo = "Bajo";
        public const string Agotado = "Agotado";

        /// <summary>
        /// Agotado si no queda stock; Bajo si la cantidad no supera el mínimo (cuando hay
        /// mínimo); Normal en cualquier otro caso.
        /// </summary>
        public static string Calcular(decimal cantidadActual, decimal cantidadMinima)
        {
            if (cantidadActual <= 0)
            {
                return Agotado;
            }

            if (cantidadMinima > 0 && cantidadActual <= cantidadMinima)
            {
                return Bajo;
            }

            return Normal;
        }

        /// <summary>Crítico = bajo o agotado.</summary>
        public static bool EsCritico(string estado) => estado is Bajo or Agotado;
    }
}
