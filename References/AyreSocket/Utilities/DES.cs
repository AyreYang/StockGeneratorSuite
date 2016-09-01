using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace AyreSocket.Utilities
{
    public class DES
    {
        public static string Encrypt(string str, string key) 
        { 
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8)); 
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str); 
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider(); 
            MemoryStream mStream = new MemoryStream(); 
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write); 
            cStream.Write(inputByteArray, 0, inputByteArray.Length); 
            cStream.FlushFinalBlock();
            return ToHexString(mStream.ToArray());
            //return Convert.ToBase64String(mStream.ToArray()); 
        }

        public static string Decrypt(string str, string key)
        { 
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8)); 
            byte[] keyIV = keyBytes; 
            //byte[] inputByteArray = Convert.FromBase64String(str);
            byte[] inputByteArray = HexToByte(str);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider(); 
            MemoryStream mStream = new MemoryStream(); 
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write); 
            cStream.Write(inputByteArray, 0, inputByteArray.Length); 
            cStream.FlushFinalBlock(); 
            return Encoding.UTF8.GetString(mStream.ToArray()); 
        }


        private static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;

            if (bytes != null)
            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                    //strB.Append(System.Convert.ToString(bytes[i], 16));
                }

                hexString = strB.ToString();

            } return hexString;

        }

        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        
    }
}
