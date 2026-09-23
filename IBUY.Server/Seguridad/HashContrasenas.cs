using System.Security.Cryptography;

namespace IBUY.Server.Seguridad
{
    /// <summary>
    /// Hashea y verifica contraseñas con PBKDF2 (SHA256), sin depender de ASP.NET Identity.
    /// Formato almacenado: "{iteraciones}.{saltBase64}.{hashBase64}".
    /// </summary>
    public static class HashContrasenas
    {
        private const int Iteraciones = 100_000;
        private const int TamanioSal = 16;
        private const int TamanioHash = 32;

        public static string Hashear(string contrasena)
        {
            byte[] sal = RandomNumberGenerator.GetBytes(TamanioSal);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);

            return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string contrasena, string hashAlmacenado)
        {
            var partes = hashAlmacenado.Split('.', 3);
            if (partes.Length != 3 || !int.TryParse(partes[0], out int iteraciones))
            {
                return false;
            }

            byte[] sal = Convert.FromBase64String(partes[1]);
            byte[] hashEsperado = Convert.FromBase64String(partes[2]);

            byte[] hashCalculado = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
    }
}
