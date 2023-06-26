#nullable enable

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Threading;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Firebridge.Common.Models;
using Firebridge.Service.Models.Services;

namespace Firebridge.Service.Win;

public class ApplicationLoader : IApplicationLoader
{
    public const int MAXIMUM_ALLOWED = 0x2000000;
    public const int TOKEN_DUPLICATE = 0x0002;
    public const int SE_GROUP_INTEGRITY = 0x00000020;
    public const int NORMAL_PRIORITY_CLASS = 0x20;

    private static string SidStringFromIL(IIntegrityLevel il)
    {
        switch (il)
        {
            case IIntegrityLevel.System:
                return "S-1-16-16384";
            case IIntegrityLevel.High:
                return "S-1-16-12288";
            case IIntegrityLevel.Medium:
                return "S-1-16-8192";
            case IIntegrityLevel.Low:
                return "S-1-16-4096";
        }
        throw new Exception("IntegrityLevel is not valid");
    }

    public static uint GetActiveSession() => PInvoke.WTSGetActiveConsoleSessionId();


    public (Process process, Stream writeStream, Stream errorStream, Stream readStream) StartProcess(string name, string[] parameters, IIntegrityLevel il)
    {
        return StartProcess(name, parameters, PInvoke.WTSGetActiveConsoleSessionId(), il);
    }


    private static bool GetTokenHandle(uint sessionID, out SafeHandle tokenHandle)
    {
        SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
        sa.nLength = (uint)Marshal.SizeOf(sa);

        SafeFileHandle hPProcess = null;
        try
        {
            SafeFileHandle hPNewToken = null;
            if (sessionID == 0)
            {

                if (!PInvoke.OpenProcessToken(new SafeFileHandle(PInvoke.GetCurrentProcess(), true), TOKEN_ACCESS_MASK.TOKEN_ALL_ACCESS, out hPProcess))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                if (!PInvoke.DuplicateTokenEx(
                        hPProcess,
                        TOKEN_ACCESS_MASK.TOKEN_ALL_ACCESS,
                        sa,
                        SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                        TOKEN_TYPE.TokenPrimary,
                        out hPNewToken)
                        )
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
            {

                if (!PInvoke.OpenProcessToken(new SafeFileHandle(Process.GetProcessesByName("Explorer").Where(x => x.SessionId == sessionID).First().Handle, false), TOKEN_ACCESS_MASK.TOKEN_ALL_ACCESS, out hPProcess))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                if (!PInvoke.DuplicateTokenEx(
                        hPProcess,
                        TOKEN_ACCESS_MASK.TOKEN_ALL_ACCESS,
                        sa,
                        SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                        TOKEN_TYPE.TokenPrimary,
                        out hPNewToken)
                        )
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            tokenHandle = hPNewToken;
            return true;
        }
        catch
        {
            tokenHandle = null;
            return false;
        }
        finally
        {
            if (hPProcess != null && !(hPProcess.IsClosed))
                hPProcess.Close(); //todo: Check for correctness
        }
    }


    public static (Process process, Stream write, Stream error, Stream read) StartProcess(string name, string[] parameters, uint sessionID, IIntegrityLevel il)
    {
        unsafe
        {
            PSID sidPointer;
            SafeHandle tokenHandle = null;
            try
            {
                GetTokenHandle(sessionID, out tokenHandle);


                //SID
                if (!PInvoke.ConvertStringSidToSid(SidStringFromIL(il), out sidPointer))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var sidLenght = PInvoke.GetLengthSid(sidPointer);


                //Token manipulation
                TOKEN_MANDATORY_LABEL TIL;
                TIL.Label.Attributes = SE_GROUP_INTEGRITY;
                TIL.Label.Sid = sidPointer;

                IntPtr hPTokenInf = Marshal.AllocHGlobal(Marshal.SizeOf<TOKEN_MANDATORY_LABEL>()); //TODO: should i release later?
                Marshal.StructureToPtr(TIL, hPTokenInf, false); //delete here?

                if (!PInvoke.SetTokenInformation(
                    tokenHandle,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel,
                    hPTokenInf.ToPointer(),
                    (uint)Marshal.SizeOf<TOKEN_MANDATORY_LABEL>() + sidLenght))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                //process starting
                STARTUPINFOW si = new STARTUPINFOW();
                si.cb = (uint)Marshal.SizeOf(si);

                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.nLength = (uint)Marshal.SizeOf(sa);
                sa.bInheritHandle = true;

                SafeFileHandle hStdOutMyRead;
                SafeFileHandle hStdOutProcessWrite;
                SafeFileHandle hStdErrMyRead;
                SafeFileHandle hStdErrProcessWrite;
                SafeFileHandle hStdInProcessRead;
                SafeFileHandle hStdInMyWrite;


                if (!PInvoke.CreatePipe(out hStdOutMyRead, out hStdOutProcessWrite, sa, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // Create a child stderr pipe.
                if (!PInvoke.CreatePipe(out hStdErrMyRead, out hStdErrProcessWrite, sa, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // Create a child stdin pipe.
                if (!PInvoke.CreatePipe(out hStdInProcessRead, out hStdInMyWrite, sa, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                PROCESS_INFORMATION procInfo;

                PInvoke.SetHandleInformation(hStdOutMyRead, 0, HANDLE_FLAGS.HANDLE_FLAG_INHERIT);
                PInvoke.SetHandleInformation(hStdErrMyRead, 0, HANDLE_FLAGS.HANDLE_FLAG_INHERIT);
                PInvoke.SetHandleInformation(hStdInMyWrite, 0, HANDLE_FLAGS.HANDLE_FLAG_INHERIT);

                si.hStdInput = new HANDLE(hStdInProcessRead.DangerousGetHandle());
                si.hStdError = new HANDLE(hStdErrProcessWrite.DangerousGetHandle());
                si.hStdOutput = new HANDLE(hStdOutProcessWrite.DangerousGetHandle());

                bool succ = false;
                hStdInProcessRead.DangerousAddRef(ref succ); // :( fix later 
                hStdErrProcessWrite.DangerousAddRef(ref succ); // :(
                hStdOutProcessWrite.DangerousAddRef(ref succ); // :(

                si.dwFlags = STARTUPINFOW_FLAGS.STARTF_USESTDHANDLES | STARTUPINFOW_FLAGS.STARTF_USESHOWWINDOW;

                Span<char> commandLine = new Span<char>((name + " " + string.Join(" ", parameters) + '\0').ToCharArray());
                if (!PInvoke.CreateProcessAsUser(tokenHandle,        // client's access token
                            null,                   // file to execute
                            ref commandLine,        // command line
                            sa,                 // pointer to process SECURITY_ATTRIBUTES
                            sa,                 // pointer to thread SECURITY_ATTRIBUTES
                            true,                  // handles are not inheritable
                            PROCESS_CREATION_FLAGS.CREATE_NEW_CONSOLE | PROCESS_CREATION_FLAGS.PROFILE_KERNEL,        // creation flags
                            null,           // pointer to new environment block 
                            null,                  // name of current directory 
                            si,                 // pointer to STARTUPINFO structure
                            out procInfo            // receives information about new process
                            ))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                PInvoke.CloseHandle(si.hStdError);
                PInvoke.CloseHandle(si.hStdInput);
                PInvoke.CloseHandle(si.hStdOutput);

                Process process = Process.GetProcessById((int)procInfo.dwProcessId);

                PInvoke.CloseHandle(procInfo.hThread);

                process.EnableRaisingEvents = true;

                return (
                    process,
                    new FileStream(hStdInMyWrite, FileAccess.Write),
                    new FileStream(hStdErrMyRead, FileAccess.Read),
                    new FileStream(hStdOutMyRead, FileAccess.Read));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (tokenHandle != null && !tokenHandle.IsClosed)
                    tokenHandle.Close();
                //todo close handles
                //if (sidPointer != 0)
                //    PInvoke.LocalFree((nint)sidPointer); //todo: check result for success??, is conversion safe?
            }
        }
    }

    public uint GetActiveSessionId() => PInvoke.WTSGetActiveConsoleSessionId();
}