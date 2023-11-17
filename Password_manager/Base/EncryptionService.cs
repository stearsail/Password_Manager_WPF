using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Password_manager.Base
{
    public class EncryptionService
    {
        public static string Encrypt(string accountPassword, string masterPassword, string accountSalt)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(accountPassword);
            using (Aes aes = GetAesManaged(masterPassword, accountSalt))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Decrypt(string encryptedPassword, string masterPassword, string accountSalt)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedPassword);
            using (Aes aes = GetAesManaged(masterPassword, accountSalt))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
                return Encoding.Unicode.GetString(ms.ToArray());
            }
        }

        private static Aes GetAesManaged(string masterPassword, string salt)
        {
            Aes aes = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(masterPassword, Encoding.Unicode.GetBytes(salt));
            aes.Key = pdb.GetBytes(32); // AES key size is 256 bits
            aes.IV = pdb.GetBytes(16); // AES block size is 128 bits
            return aes;
        }

        public static string GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
    }
}
