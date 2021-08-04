using VueCore.Models;

namespace VueCore.Services.Security
{
    public interface IPasswordHasher
    {
        HashedPassword HashPassword(string password);
        HashedPassword HashPassword(string password, byte[] salt);
         
    }
}