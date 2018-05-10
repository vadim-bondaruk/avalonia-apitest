using NSec.Cryptography;
using System;

namespace DataEncryptionLibrary.Models
{
    public class EncryptorKey
    {
        public Key Key { get; set; }
        public Nonce Nonce { get; set; }
    }
}
