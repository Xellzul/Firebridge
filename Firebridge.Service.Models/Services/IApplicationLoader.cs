using Firebridge.Common.Models;
using System.Diagnostics;

namespace Firebridge.Service.Models.Services;

public interface IApplicationLoader
{
    public uint GetActiveSessionId();

    public (Process process, Stream writeStream, Stream errorStream, Stream readStream) StartProcess(string name, string[] parameters, IIntegrityLevel il);
}
