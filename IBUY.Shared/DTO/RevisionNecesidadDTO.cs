namespace IBUY.Shared.DTO
{
    /// <summary>Revisión de stock de una necesidad: solo informa, no modifica nada.</summary>
    public class RevisionNecesidadDTO
    {
        public int NecesidadId { get; set; }

        public int ProductoId { get; set; }

        public int DepositoDestinoId { get; set; }

        public decimal CantidadRequerida { get; set; }

        public decimal StockDisponible { get; set; }

        public bool Alcanza { get; set; }

        public decimal Faltante { get; set; }
    }
}
