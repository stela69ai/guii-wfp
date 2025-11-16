using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SimpleCrypt.Functions.Encryption.RC4
{
    public static class RC4
    {
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            byte[] key = DeriveKey(password, 256);
            byte[] data = File.ReadAllBytes(inputFile);
            byte[] encrypted = RC4Transform(data, key);
            File.WriteAllBytes(outputFile, encrypted);
        }

        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            // RC4 is symmetric, so decryption is the same as encryption
            EncryptFile(inputFile, outputFile, password);
        }

        private static byte[] DeriveKey(string password, int keySize)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("RC4_SALT"), 1000))
            {
                return deriveBytes.GetBytes(keySize / 8);
            }
        }

        private static byte[] RC4Transform(byte[] data, byte[] key)
        {
            byte[] s = new byte[256];
            byte[] k = new byte[256];
            byte[] result = new byte[data.Length];

            // Initialize S and K arrays
            for (int i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
                k[i] = key[i % key.Length];
            }

            // Key scheduling algorithm
            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                // Swap s[i] and s[j]
                byte temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            // Pseudo-random generation algorithm
            int x = 0, y = 0;
            for (int i = 0; i < data.Length; i++)
            {
                x = (x + 1) % 256;
                y = (y + s[x]) % 256;

                // Swap s[x] and s[y]
                byte temp = s[x];
                s[x] = s[y];
                s[y] = temp;

                int t = (s[x] + s[y]) % 256;
                result[i] = (byte)(data[i] ^ s[t]);
            }

            return result;
        }
    }
}