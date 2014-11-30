using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;
using System.Text;


namespace MothershipUtility
{
    /// <summary>
    /// Provides utility functions for working with MD5 hashes
    /// </summary>
    public static class HashUtility
    {
        public static string GetMD5Hash(string input)
        {
            string hash;
            using (MD5 md5Hash = MD5.Create())
            {
                hash = CreateMD5Hash(input, md5Hash);
            }
            return hash;
        }

        public static string CreateMD5Hash(string input, MD5 hash)
        {
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Format each byte of the hash as a hexadecimal string. 
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                strBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return strBuilder.ToString();
        }

        public static bool VerifyMD5Hash(string inputString, string inputHash, MD5 hash)
        {
            // Hash the input string 
            string hashFromInput = CreateMD5Hash(inputString, hash);

            // Check the new hash agains the input hash
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(inputHash, hashFromInput) == 0;
        }
        
    }
}
