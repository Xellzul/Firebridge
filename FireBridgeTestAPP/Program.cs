using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FireBridgeTestAPP
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = new Process();
            
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            string vspath = Regex.Match(output, @"installationPath:(.*)").Groups[1].Value;
            vspath = vspath.Replace("\r", "");
            string installPath = vspath + @"\Common7\IDE\";

            string vs = "devenv.exe";

            p = new Process();

            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WorkingDirectory = installPath;
            p.StartInfo.FileName = vs;
            p.StartInfo.Arguments = "/client /joinworkspace \"vsls:?action=join&workspaceId=2A3E65DB5CFC9F5B35C6C3C89F7A776EAF1C&correlationId=null\"";
            p.Start();
        }
    }
}
