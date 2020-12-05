using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Huby.Data
{
    public interface ISimpleCrypto
    {
        byte[] Encrypt(object data);
        string Decrypt(byte[] bytes);
    }

    public sealed class SimpleCrypto : ISimpleCrypto
    {
        public ICryptoTransform Encryptor { get; }
        public ICryptoTransform Decryptor { get; }

        public SimpleCrypto(ICryptoTransform encryptor, ICryptoTransform decryptor)
        {
            Encryptor = encryptor;
            Decryptor = decryptor;
        }

        public byte[] Encrypt(object data)
        {
            using (var stream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(stream, Encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(data);
                }

                return stream.ToArray();
            }
        }

        public string Decrypt(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var cryptoStream = new CryptoStream(stream, Decryptor, CryptoStreamMode.Read))
            {
                var reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
        }
    }
}
