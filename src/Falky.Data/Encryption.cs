using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Flaky.Data
{
    public class Encryption
    {
        
        
        private RSA _rsa;

        private Encryption()
        {
            _rsa = RSA.Create();
        }

        public EncryptionDetails PublicDetails
        {
             get
            {
                var rsaParams = _rsa.ExportParameters(false);

                return new EncryptionDetails
                {
                    Modulus = Convert.ToBase64String(rsaParams.Modulus),
                    Exponent = Convert.ToBase64String(rsaParams.Exponent)
                };
            }
        }

        public static Encryption CreateEncryption()
        {
            return new Encryption();
        }

        public static Encryption CreateEncryption(EncryptionDetails details)
        {
            var encryption = CreateEncryption();

            encryption._rsa.ImportParameters(new RSAParameters()
            {
                Modulus = Convert.FromBase64String(details.Modulus),
                Exponent = Convert.FromBase64String(details.Exponent)
            });

            return encryption;
        }

        public string EncryptToBase64(string message)
        {
            using (var aes = Aes.Create())
            {
                var encrypedMessage = EncryptStringToBytes_Aes(message, aes.Key, aes.IV);
                var encrypedAesKey = _rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
                var encrypedAesIV = _rsa.Encrypt(aes.IV, RSAEncryptionPadding.Pkcs1);
                return String.Join(".",Convert.ToBase64String(encrypedAesKey), Convert.ToBase64String(encrypedAesIV), Convert.ToBase64String(encrypedMessage));
            }
        }

        public string DecryptFromBase64(string encodedMessage)
        {
            using (var aes = Aes.Create())
            {
                var parts = encodedMessage.Split(".");

                if (parts.Length != 3)
                {
                    throw new Exception("Invalided number of segments");
                }

                aes.Key = _rsa.Decrypt(Convert.FromBase64String(parts[0]), RSAEncryptionPadding.Pkcs1);
                aes.IV = _rsa.Decrypt(Convert.FromBase64String(parts[1]), RSAEncryptionPadding.Pkcs1);
                var decoded = DecryptStringFromBytes_Aes(Convert.FromBase64String(parts[2]), aes.Key, aes.IV);

                return decoded;
            }
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}

