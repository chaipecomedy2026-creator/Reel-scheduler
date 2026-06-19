using System.Text;
using System.Security.Cryptography;
using ReelSchedulerPro.Application.Services;
using Serilog;

namespace ReelSchedulerPro.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    private readonly string _encryptionKey;
    private readonly ILogger _logger = Log.ForContext<EncryptionService>();

    public EncryptionService(IConfiguration configuration)
    {
        _encryptionKey = configuration["Encryption:Key"] ?? "default-encryption-key-change-in-production-1234";
    }

    public string Encrypt(string plainText)
    {
        try
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error encrypting data");
            throw;
        }
    }

    public string Decrypt(string cipherText)
    {
        try
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var buffer = Convert.FromBase64String(cipherText);
                using (var ms = new MemoryStream(buffer))
                {
                    var iv = new byte[aes.IV.Length];
                    ms.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error decrypting data");
            throw;
        }
    }
}
