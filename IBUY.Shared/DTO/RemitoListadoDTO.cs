namespace IBUY.Shared.DTO
{
    public class RemitoListadoDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;

        /// <summary>Nombre del depósito origen (Interno) o texto de OrigenExterno (Externo).</summary>
        public string Origen { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int? DepositoOrigenId { get; set; }
        public int? DepositoDestinoId { get; set; }
    }
}
