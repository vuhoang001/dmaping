using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Invoice.Extensions;

public static class BkavEncryptHelper
{
    private static void ExtractKeyIv(string partnerToken, out byte[] key, out byte[] iv)
    {
        var parts = partnerToken.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("PartnerToken không đúng định dạng: KeyBase64:IVBase64");

        key = Convert.FromBase64String(parts[0]);
        iv  = Convert.FromBase64String(parts[1]);
    }

    public static string EncryptCommandData(string plainText, string partnerToken)
    {
        ExtractKeyIv(partnerToken, out var key, out var iv);
        var    plainBytes = Encoding.UTF8.GetBytes(plainText);
        var    zipped     = GzipCompress(plainBytes);
        byte[] encryptedBytes;
        using (var aes = Aes.Create())
        {
            aes.KeySize   = 256;
            aes.BlockSize = 128;
            aes.Mode      = CipherMode.CBC;
            aes.Padding   = PaddingMode.PKCS7;
            aes.Key       = key;
            aes.IV        = iv;

            using var encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(zipped, 0, zipped.Length);
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string DecryptCommandData(string base64Input, string partnerToken)
    {
        ExtractKeyIv(partnerToken, out var key, out var iv);
        var    encryptedBytes = Convert.FromBase64String(base64Input);
        byte[] zipped;
        using (var aes = Aes.Create())
        {
            aes.KeySize   = 256;
            aes.BlockSize = 128;
            aes.Mode      = CipherMode.CBC;
            aes.Padding   = PaddingMode.PKCS7;
            aes.Key       = key;
            aes.IV        = iv;

            using var decryptor = aes.CreateDecryptor();
            zipped = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        }

        var plainBytes = GzipDecompress(zipped);
        return Encoding.UTF8.GetString(plainBytes);
    }

    private static byte[] GzipCompress(byte[] raw)
    {
        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            gzip.Write(raw, 0, raw.Length);
        }

        return ms.ToArray();
    }

    private static byte[] GzipDecompress(byte[] gzipped)
    {
        using var input  = new MemoryStream(gzipped);
        using var gzip   = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }
}