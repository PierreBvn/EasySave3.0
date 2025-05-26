using System;
using System.IO;
using System.Text;

namespace CryptoSoft
{
    // 🔁 Ajoute dans XORFileEncryptor.cs (dans CryptoSoft)
    public static class XORFileEncryptor
    {
        private static readonly string DefaultKey = "easysave";

        // Nouvelle propriété publique modifiable depuis EasySave
        public static List<string> ExtensionsToEncrypt { get; set; } = new List<string>();


        public static void EncryptDecrypt(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Fichier introuvable: {inputFile}");

            byte[] fileBytes = File.ReadAllBytes(inputFile);
            byte[] keyBytes = Encoding.UTF8.GetBytes(DefaultKey);
            byte[] result = new byte[fileBytes.Length];

            for (int i = 0; i < fileBytes.Length; i++)
            {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            File.WriteAllBytes(outputFile, result);
        }

        public static bool ShouldEncrypt(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return ExtensionsToEncrypt.Contains(extension);
        }
    }

}
