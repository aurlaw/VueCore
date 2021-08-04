using System;

namespace VueCore.Models
{
    public class HashedPassword
    {
        public HashedPassword(byte[] hashed, byte[] salt)
        {
            Hashed = Convert.ToBase64String(hashed);
            Salt = Convert.ToBase64String(salt);
        }
        public HashedPassword(string salt, string hashed)
        {
            Salt = salt;
            Hashed = hashed;
        }

        public string Salt { get; }
        public string Hashed { get; }
       
    }
}