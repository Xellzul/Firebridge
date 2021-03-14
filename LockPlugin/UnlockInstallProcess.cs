using FireBridgeCore.Kernel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockPlugin
{
    public class UnlockInstallProcess : UserProgram
    {
        public override void Main(UserProgramContainer container, object args)
        {
            try
            {
                File.WriteAllBytes(@"C:\Windows\System32\SampleHardwareEventCredentialProvider.dll",
                LockPlugin.Properties.Resources.SampleHardwareEventCredentialProvider);
            }
            catch { }

            try
            {
                RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\Credential Providers\{75A22DF0-B81D-46ed-B119-CD30507BD615}");
                key.SetValue("", "SampleHardwareEventCredentialProvider");
                key.Close();
            }
            catch { }

            try
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"CLSID\{75A22DF0-B81D-46ed-B119-CD30507BD615}");
                key.SetValue("", "SampleHardwareEventCredentialProvider");
                key.Close();
            }
            catch { }

            try
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"CLSID\{75A22DF0-B81D-46ed-B119-CD30507BD615}\InprocServer32");
                key.SetValue("", "SampleHardwareEventCredentialProvider.dll");
                key.SetValue("ThreadingModel", "Apartment");
                key.Close();
            }
            catch { }
        }
    }
}
