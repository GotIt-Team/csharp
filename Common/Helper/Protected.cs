using System;
using System.Text;
using System.Security.Cryptography;

namespace GotIt.Common.Helper
{
    public static class Protected
    {
        private static readonly int saltSize = 20;
        private static readonly int saltPosition = 5;
        public static string CreatePasswordHash(string pwd)
        {
            return CreatePasswordHash(pwd, CreateSalt());
        }

        public static bool Validate(string password, string passwordHash)
        {
            try
            {
                var salt = passwordHash.Substring(saltPosition, saltSize);
                var hashedPassword = CreatePasswordHash(password, salt);
                return hashedPassword == passwordHash;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            string hashedPwd = GetHashString(saltAndPwd);
            hashedPwd = hashedPwd.Insert(saltPosition, salt);
            return hashedPwd;
        }

        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            try
            {
                byte[] buff = new byte[20];
                rng.GetBytes(buff);
                string salt = Convert.ToBase64String(buff);
                if (salt.Length > saltSize)
                {
                    salt = salt.Substring(0, saltSize);
                    return salt.ToUpper();
                }

                var saltChar = '^';
                salt = salt.PadRight(saltSize, saltChar);
                return salt.ToUpper();
            }
            finally
            {
                rng.Dispose();
            }
        }

        private static string GetHashString(string password)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(password))
                sb.Append(b.ToString("X2"));
            
            return sb.ToString();
        }

        private static byte[] GetHash(string password)
        {
            SHA384 sha = new SHA384CryptoServiceProvider();
            try
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            finally
            {
                sha.Dispose();
            }
        }
    }
}