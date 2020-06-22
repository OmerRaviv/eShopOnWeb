using Flaky.Data;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Flaky.API.Services
{
    public static class EncryptionService
    {
        private static Encryption _encryption;

        static EncryptionService()
        {
            _encryption = Encryption.CreateEncryption();
        }

        public static string PublicKey
        {
            get
            { 
                return Newtonsoft.Json.JsonConvert.SerializeObject(_encryption.PublicDetails);
            }
        }

        public static string Dercypt(string base64)
        {
            return _encryption.DecryptFromBase64(base64);
        }

        public static T Dercypt<T>(string base64)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Dercypt(base64));
        }

        public static string Encrypt(string message)
        {
            return _encryption.EncryptToBase64(message);
        }
    }
}