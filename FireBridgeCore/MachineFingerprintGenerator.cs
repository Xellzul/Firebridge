using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;

namespace FireBridgeCore;

public static class MachineFingerprintGenerator
{

    public static Guid GetFingerprint()
    {
        using (var md5Hasher = MD5.Create())
        {
            using var inputStream = new MemoryStream();
            using var inputWriter = new StreamWriter(inputStream);

            inputWriter.Write(GetCpuInfo());

            inputWriter.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);

            var data = md5Hasher.ComputeHash(inputStream);
            return new Guid(data);
        }
    }

    static string GetCpuInfo()
    {
        var moc = new ManagementClass("win32_processor").GetInstances();

        foreach (ManagementObject mo in moc)
        {
            return mo.Properties["processorID"].Value.ToString();
        }

        throw new SystemException("No processor Id");
    }
}
