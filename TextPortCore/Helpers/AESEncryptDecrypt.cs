using System;
using System.IO;
using System.Security.Cryptography;

namespace TextPortCore.Helpers
{
    public class AESEncryptDecrypt
    {
        public static string Encrypt(string input, string pass)
        {
            try
            {
                return Convert.ToBase64String(EncryptStringToBytes(input, System.Text.Encoding.Default.GetBytes(pass)));
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string Decrypt(string input, string pass)
        {
            try
            {
                return DecryptStringFromBytes(Convert.FromBase64String(input), System.Text.Encoding.Default.GetBytes(pass));
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key)
        {
            return EncryptStringToBytes(plainText, Key, null);
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if ((plainText == null) || (plainText.Length <= 0))
            {
                throw (new ArgumentNullException("plainText"));
            }
            if ((Key == null) || (Key.Length <= 0))
            {
                throw (new ArgumentNullException("Key"));
            }
            // Create an RijndaelManaged object
            // with the specified key and IV.
            RijndaelManaged rijAlg = new RijndaelManaged();
            rijAlg.Key = Key;
            if (!(IV == null))
            {
                if (IV.Length > 0)
                {
                    rijAlg.IV = IV;
                }
                else
                {
                    rijAlg.Mode = CipherMode.ECB;
                }
            }
            else
            {
                rijAlg.Mode = CipherMode.ECB;
            }
            byte[] encrypted = null;
            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor();
            // Create the streams used for encryption.
            encrypted = encryptor.TransformFinalBlock(System.Text.Encoding.Default.GetBytes(plainText), 0, plainText.Length); //msEncrypt.ToArray
                                                                                                                              // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static byte[] EncryptBytesToBytes(byte[] Input, byte[] Key)
        {
            return EncryptBytesToBytes(Input, Key, null);
        }

        private static byte[] EncryptBytesToBytes(byte[] Input, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if ((Input == null) || (Input.Length <= 0))
            {
                throw (new ArgumentNullException("plainText"));
            }
            if ((Key == null) || (Key.Length <= 0))
            {
                throw (new ArgumentNullException("Key"));
            }
            // Create an RijndaelManaged object
            // with the specified key and IV.
            RijndaelManaged rijAlg = new RijndaelManaged();
            rijAlg.Key = Key;
            if (!(IV == null))
            {
                if (IV.Length > 0)
                {
                    rijAlg.IV = IV;
                }
                else
                {
                    rijAlg.Mode = CipherMode.ECB;
                }
            }
            else
            {
                rijAlg.Mode = CipherMode.ECB;
            }
            byte[] encrypted = null;
            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            encrypted = encryptor.TransformFinalBlock(Input, 0, Input.Length);
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key)
        {
            return DecryptStringFromBytes(cipherText, Key, null);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if ((cipherText == null) || (cipherText.Length <= 0))
            {
                throw (new ArgumentNullException("cipherText"));
            }
            if ((Key == null) || (Key.Length <= 0))
            {
                throw (new ArgumentNullException("Key"));
            }
            // Create an RijndaelManaged object
            // with the specified key and IV.
            RijndaelManaged rijAlg = new RijndaelManaged();
            rijAlg.Key = Key;
            rijAlg.Mode = CipherMode.CBC;
            if (!(IV == null))
            {
                if (IV.Length > 0)
                {
                    rijAlg.IV = IV;
                }
                else
                {
                    rijAlg.Mode = CipherMode.ECB;
                }
            }
            else
            {
                rijAlg.Mode = CipherMode.ECB;
            }
            string plaintext = null;
            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for decryption.
            MemoryStream msDecrypt = new MemoryStream(cipherText);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            StreamReader srDecrypt = new StreamReader(csDecrypt);
            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            plaintext = srDecrypt.ReadToEnd();
            return plaintext;
        }

        private static byte[] DecryptBytesFromBytes(byte[] cipherText, byte[] Key)
        {
            return DecryptBytesFromBytes(cipherText, Key, null);
        }

        private static byte[] DecryptBytesFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if ((cipherText == null) || (cipherText.Length <= 0))
            {
                throw (new ArgumentNullException("cipherText"));
            }
            if ((Key == null) || (Key.Length <= 0))
            {
                throw (new ArgumentNullException("Key"));
            }
            // Create an RijndaelManaged object
            // with the specified key and IV.
            RijndaelManaged rijAlg = new RijndaelManaged();
            rijAlg.Key = Key;
            if (!(IV == null))
            {
                if (IV.Length > 0)
                {
                    rijAlg.IV = IV;
                }
                else
                {
                    rijAlg.Mode = CipherMode.ECB;
                }
            }
            else
            {
                rijAlg.Mode = CipherMode.ECB;
            }
            byte[] output = null;
            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
            // Create the streams used for decryption.
            MemoryStream msDecrypt = new MemoryStream(cipherText);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            StreamReader srDecrypt = new StreamReader(csDecrypt);
            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            MemoryStream ms = new MemoryStream();
            while (!srDecrypt.EndOfStream)
            {
                ms.WriteByte((byte)(srDecrypt.Read()));
            }
            ms.Position = 0;
            output = ms.ToArray();
            return output;
        }
    }
}
