using DataEncryptionLibrary.Models;
using NSec.Cryptography;

namespace DataEncryptionLibrary.Interfaces
{
    public interface IDataEncryptor
    {
        EncryptorKey CreateX25519EncryptorKey();
        byte[] GetX25519EncryptorPublicKeyBytes(Key serverKey);
        Key CreateAes256GcmSymmetricKey(byte[] clientPublicKeyBytes, Key serverKey);
        byte[] EncryptDataByAes256Gcm(string data, EncryptorKey encryptorKey);
        string Decrypt(byte[] cipherText, EncryptorKey encryptorKey);
        byte[] GetNonceBytes(Nonce nonce);
    }
}
