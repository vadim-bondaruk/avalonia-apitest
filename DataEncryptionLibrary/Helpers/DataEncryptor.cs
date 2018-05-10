using DataEncryptionLibrary.Interfaces;
using DataEncryptionLibrary.Models;
using NSec.Cryptography;
using System;
using System.Text;

namespace DataEncryptionLibrary.Helpers
{
    public class DataEncryptor : IDataEncryptor
    {
        private static KeyAgreementAlgorithm keyAgreementAlgorithm = new X25519();
        private static AeadAlgorithm aeadAlgorithm = new Aes256Gcm();
        public EncryptorKey CreateX25519EncryptorKey()
        {
            var encryptorKey = new EncryptorKey();
            encryptorKey.Key = new Key(keyAgreementAlgorithm);
            //Create Nonce
            Random rnd = new Random();
            Byte[] serverWriteIV = new Byte[4];
            rnd.NextBytes(serverWriteIV);
            encryptorKey.Nonce = new Nonce(fixedField: serverWriteIV, counterFieldSize: 8);
            return encryptorKey;

        }
        public byte[] GetX25519EncryptorPublicKeyBytes(Key serverKey)
        {
            return serverKey.PublicKey.Export(KeyBlobFormat.RawPublicKey);
        }

        public byte[] GetNonceBytes(Nonce nonce)
        {
            Byte[] nonceBytes = new Byte[12];
            nonce.CopyTo(nonceBytes);
            return nonceBytes;
        }
        public Key CreateAes256GcmSymmetricKey(byte[] clientPublicKeyBytes, Key serverKey)
        {
            var keyDerivationAlgorithm = new HkdfSha256();
            //Import clientPublicKey from bytes
            PublicKey clientPublicKey = PublicKey.Import(keyAgreementAlgorithm, clientPublicKeyBytes, KeyBlobFormat.RawPublicKey);
            //Create SharedSecret
            SharedSecret sharedSecretServer = keyAgreementAlgorithm.Agree(serverKey, clientPublicKey);
            //Convert sharedSecret to bytes
            var sharedSecretBytes = keyDerivationAlgorithm.DeriveBytes(sharedSecretServer, null, null, sharedSecretServer.Size);
            //Create symmetric key from sharedSecret bytes
            return Key.Import(aeadAlgorithm, sharedSecretBytes, KeyBlobFormat.RawSymmetricKey);
        }

        public byte[] EncryptDataByAes256Gcm(string data, EncryptorKey encryptorKey)
        {
            //return new byte[]{};
            return aeadAlgorithm.Encrypt(encryptorKey.Key, encryptorKey.Nonce, null, Encoding.UTF8.GetBytes(data));
            // increment the counter field of the send nonce
            //if (!Nonce.TryIncrement(ref Nonce))
            //{
            //    // abort the connection when the counter field of the
            //    // send nonce reaches the maximum value
            //    simmetricKey.Dispose();
            //    serverKey.Dispose();
            //}
        }
        public string Decrypt(byte[] cipherText, EncryptorKey encryptorKey)
        {
            aeadAlgorithm.Decrypt(encryptorKey.Key, encryptorKey.Nonce, null, cipherText, out byte[] byteText);
            return Encoding.UTF8.GetString(byteText);
        }




    }
}
