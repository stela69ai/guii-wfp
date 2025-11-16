using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SimpleCrypt.Functions.Encryption.RSA
{
    public static class RSA
    {
        public static void EncryptFile(string inputFile, string outputFile, string publicKey)
        {
            try
            {
                byte[] data = File.ReadAllBytes(inputFile);

                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    rsa.FromXmlString(publicKey);

                    // RSA can only encrypt data smaller than the key size
                    // So we'll encrypt with a hybrid approach: generate a random AES key,
                    // encrypt the file with AES, then encrypt the AES key with RSA

                    using (Aes aes = Aes.Create())
                    {
                        aes.KeySize = 256;
                        aes.GenerateKey();
                        aes.GenerateIV();

                        // Encrypt the data with AES
                        byte[] encryptedData;
                        using (var encryptor = aes.CreateEncryptor())
                        using (var ms = new MemoryStream())
                        {
                            ms.Write(aes.IV, 0, aes.IV.Length);
                            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            {
                                cs.Write(data, 0, data.Length);
                                cs.FlushFinalBlock();
                            }
                            encryptedData = ms.ToArray();
                        }

                        // Encrypt the AES key with RSA
                        byte[] encryptedKey = rsa.Encrypt(aes.Key, false);

                        // Write both to output file
                        using (var fs = new FileStream(outputFile, FileMode.Create))
                        {
                            // Write RSA encrypted key length and key
                            byte[] keyLength = BitConverter.GetBytes(encryptedKey.Length);
                            fs.Write(keyLength, 0, keyLength.Length);
                            fs.Write(encryptedKey, 0, encryptedKey.Length);

                            // Write AES encrypted data
                            fs.Write(encryptedData, 0, encryptedData.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"RSA Encryption failed: {ex.Message}");
            }
        }

        public static void DecryptFile(string inputFile, string outputFile, string privateKey)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(inputFile);

                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    rsa.FromXmlString(privateKey);

                    // Read the encrypted AES key
                    int keyLength = BitConverter.ToInt32(fileData, 0);
                    byte[] encryptedKey = new byte[keyLength];
                    Array.Copy(fileData, 4, encryptedKey, 0, keyLength);

                    // Read the encrypted data
                    byte[] encryptedData = new byte[fileData.Length - 4 - keyLength];
                    Array.Copy(fileData, 4 + keyLength, encryptedData, 0, encryptedData.Length);

                    // Decrypt the AES key with RSA
                    byte[] aesKey = rsa.Decrypt(encryptedKey, false);

                    // Decrypt the data with AES
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = aesKey;

                        // Read IV from the beginning of encrypted data
                        byte[] iv = new byte[16];
                        Array.Copy(encryptedData, 0, iv, 0, 16);
                        aes.IV = iv;

                        byte[] data = new byte[encryptedData.Length - 16];
                        Array.Copy(encryptedData, 16, data, 0, data.Length);

                        using (var decryptor = aes.CreateDecryptor())
                        using (var ms = new MemoryStream(data))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        using (var fs = new FileStream(outputFile, FileMode.Create))
                        {
                            cs.CopyTo(fs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"RSA Decryption failed: {ex.Message}");
            }
        }

        public static string GenerateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                return rsa.ToXmlString(true); // true for private key, false for public only
            }
        }

        public static string GetPublicKey(string keyPair)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(keyPair);
                return rsa.ToXmlString(false);
            }
        }
    }
}