namespace IBUY.Shared.DTO
{
    public class LoginRespuestaDTO
    {
        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public int EmpresaId { get; set; }

        public string EmpresaRazonSocial { get; set; } = string.Empty;

        public List<DepositoResumenDTO> Depositos { get; set; } = [];
    }
}
