using InficareTaskProject.Entities;

namespace InficareTaskProject.Interfaces
{
    public interface IJwtTokenManager
    {
        public string GenerateToken(Customer identityUser);
        public string SignDataPKCS8(string data);
    }
}
