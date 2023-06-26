using Firebridge.Common.Models.Services;
using Microsoft.Extensions.Logging;
using System.Management;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace Firebridge.Common;

[SupportedOSPlatform("windows")]
public class FingerprintService : IFingerprintService
{
    private Guid? guid = null;
    private readonly ILogger<FingerprintService> logger;
    public FingerprintService(ILogger<FingerprintService> logger)
    {
        this.logger = logger;
    }

    public Guid GetFingerprint()
    {
        if (guid == null)
        {
            guid = GetFingerprintWithoutCache();
            logger.LogInformation($"Generated fingerprint {guid}");

            ArgumentNullException.ThrowIfNull(guid);
        }

        return (Guid)guid;
    }

    private Guid GetFingerprintWithoutCache()
    {
        using (var md5Hasher = MD5.Create())
        {
            using var inputStream = new MemoryStream();
            using var inputWriter = new StreamWriter(inputStream);

            inputWriter.Write(GetCpuInfo());

            inputWriter.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);

            var data = md5Hasher.ComputeHash(inputStream);
            var guid = new Guid(data);

            return guid;
        }
    }

    private static string GetCpuInfo()
    {
        var moc = new ManagementClass("win32_processor").GetInstances();

        foreach (ManagementObject mo in moc)
        {
            var processorId = mo.Properties["processorID"].Value.ToString();

            if (processorId == null)
                throw new SystemException("No processor Id");

            return processorId;
        }

        throw new SystemException("No processor Id");
    }
}