using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel.UserPrograms
{
    [Serializable]
    public class UnlockPcProcess : UserProcess
    {
        public string Password { get; set; }
        public string Username { get; set; }

        public override void Main()
        {
            try
            {
                File.WriteAllBytes(@"C:\Windows\System32\SampleHardwareEventCredentialProvider.dll",
                FireBridgeCore.Properties.Resources.SampleHardwareEventCredentialProvider);
            }
            catch { }

            try { 
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


            using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", "MyPipe", PipeDirection.InOut))
            {
                pipe.Connect();
                if(pipe.IsConnected)
                {
                    using (StreamWriter sw = new StreamWriter(pipe))
                    {
                        sw.Write(Username);
                        sw.Write('\0');
                        sw.Write(Password);
                        sw.Write('\0');
                        sw.Flush();
                    }
                }
            }
        }
    }
}
