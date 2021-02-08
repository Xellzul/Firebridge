using FireBridgeCore.Kernel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Windows.Sdk;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace FireBridgeCore
{
    public static class ApplicationLoader
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

        public static (Process process, Stream writeStream, Stream errorStream, Stream readStream) StartProcess(string name, string[] parameters, IIntegrityLevel il)
        {
            return StartProcess(name, parameters, PInvoke.WTSGetActiveConsoleSessionId(), il);
        }


        private static bool GetTokenHandle(uint sessionID, out CloseHandleSafeHandle tokenHandle)
        {
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.nLength = (uint)Marshal.SizeOf(sa);

            CloseHandleSafeHandle hPProcess = CloseHandleSafeHandle.Null;
            try
            {
                IntPtr tokenPointer = IntPtr.Zero;
                if (sessionID == 0)
                {
                    IntPtr processPointer = IntPtr.Zero;
                    if (!PInvoke.OpenProcessToken(PInvoke.GetCurrentProcess(), TOKEN_DUPLICATE, out processPointer))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    hPProcess = new CloseHandleSafeHandle(processPointer);

                    if (!PInvoke.DuplicateTokenEx(
                            hPProcess,
                            MAXIMUM_ALLOWED,
                            sa,
                            SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                            TOKEN_TYPE.TokenPrimary,
                            out tokenPointer)
                            )
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                else
                {
                    if (!PInvoke.WTSQueryUserToken(sessionID, ref tokenPointer))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                tokenHandle = new CloseHandleSafeHandle(tokenPointer);
                return true;
            }
            catch
            {
                tokenHandle = CloseHandleSafeHandle.Null;
                return false;
            }
            finally
            {
                if (!hPProcess.IsClosed)
                    hPProcess.Close(); //todo: Check for correctness
            }
        }


        public static (Process process, Stream write, Stream error, Stream read) StartProcess(string name, string[] parameters, uint sessionID, IIntegrityLevel il)
        {
            unsafe
            {
                void* sidPointer = null;
                CloseHandleSafeHandle tokenHandle = CloseHandleSafeHandle.Null;
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

                    IntPtr pStdOutMyRead = IntPtr.Zero;
                    IntPtr pStdOutProcessWrite = IntPtr.Zero;
                    IntPtr pStdErrMyRead = IntPtr.Zero;
                    IntPtr pStdErrProcessWrite = IntPtr.Zero;
                    IntPtr pStdInProcessRead = IntPtr.Zero;
                    IntPtr pStdInMyWrite = IntPtr.Zero;


                    if (!PInvoke.CreatePipe(out pStdOutMyRead, out pStdOutProcessWrite, sa, 0))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    // Create a child stderr pipe.
                    if (!PInvoke.CreatePipe(out pStdErrMyRead, out pStdErrProcessWrite, sa, 0))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    // Create a child stdin pipe.
                    if (!PInvoke.CreatePipe(out pStdInProcessRead, out pStdInMyWrite, sa, 0))
                        throw new Win32Exception(Marshal.GetLastWin32Error());


                    uint dwCreationFlags = NORMAL_PRIORITY_CLASS | 0x00000010; //CREATE_NEW_CONSOLE | WinApi.CREATE_UNICODE_ENVIRONMENT;

                    PROCESS_INFORMATION procInfo;

                    var hStdOutMyRead = new CloseHandleSafeHandle(pStdOutMyRead);
                    var hStdErrMyRead = new CloseHandleSafeHandle(pStdErrMyRead);
                    var hStdInMyWrite = new CloseHandleSafeHandle(pStdInMyWrite);

                    PInvoke.SetHandleInformation(hStdOutMyRead, 0, HANDLE_FLAG_OPTIONS.HANDLE_FLAG_INHERIT);
                    PInvoke.SetHandleInformation(hStdErrMyRead, 0, HANDLE_FLAG_OPTIONS.HANDLE_FLAG_INHERIT);
                    PInvoke.SetHandleInformation(hStdInMyWrite, 0, HANDLE_FLAG_OPTIONS.HANDLE_FLAG_INHERIT);

                    si.hStdInput = new HANDLE(pStdInProcessRead);
                    si.hStdError = new HANDLE(pStdErrProcessWrite);
                    si.hStdOutput = new HANDLE(pStdOutProcessWrite);
                    si.dwFlags = 0x100 | 0x1;

                    if (!PInvoke.CreateProcessAsUser(tokenHandle,        // client's access token
                                null,                   // file to execute
                                name + " " + string.Join(" ", parameters),        // command line
                                sa,                 // pointer to process SECURITY_ATTRIBUTES
                                sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                true,                  // handles are not inheritable
                                dwCreationFlags,        // creation flags
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
                        new FileStream(new SafeFileHandle(pStdInMyWrite, true), FileAccess.Write),
                        new FileStream(new SafeFileHandle(pStdErrMyRead, true), FileAccess.Read),
                        new FileStream(new SafeFileHandle(pStdOutMyRead, true), FileAccess.Read));
                }
                catch
                {
                    return (null, null, null, null);
                }
                finally
                {
                    if (!tokenHandle.IsClosed)
                        tokenHandle.Close();
                    //todo close handles
                    if (sidPointer != null)
                        PInvoke.LocalFree((nint)sidPointer); //todo: check result for success??, is conversion safe?
                }
            }
        }
    }


    public static class IntegrityLevelHelper
    {

        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int TOKEN_QUERY = 0x0008;
        const int SECURITY_MANDATORY_UNTRUSTED_RID = (0x00000000);
        const int SECURITY_MANDATORY_LOW_RID = (0x00001000);
        const int SECURITY_MANDATORY_MEDIUM_RID = (0x00002000);
        const int SECURITY_MANDATORY_HIGH_RID = (0x00003000);
        const int SECURITY_MANDATORY_SYSTEM_RID = (0x00004000);
        const int SECURITY_MANDATORY_PROTECTED_PROCESS_RID = (0x00005000);

        public static IIntegrityLevel GetCurrentIntegrity()
        {
            int il = GetProcessIntegrityLevel();

            switch (il)
            {
                case SECURITY_MANDATORY_SYSTEM_RID:
                    return IIntegrityLevel.System;
                case SECURITY_MANDATORY_HIGH_RID:
                    return IIntegrityLevel.High;
                case SECURITY_MANDATORY_MEDIUM_RID:
                    return IIntegrityLevel.Medium;
                case SECURITY_MANDATORY_LOW_RID:
                    return IIntegrityLevel.Low;
                default:
                    return IIntegrityLevel.Low;
            }

        }

        //https://social.msdn.microsoft.com/Forums/en-US/d1437d05-7a3d-4b3f-91cf-a49f45527543/getting-process-integrity-level-in-vista-using-pinvoke?forum=Vsexpressvcs
        /// <summary>
        /// The function gets the integrity level of the current process.
        /// </summary>
        /// <returns>
        /// Returns the integrity level of the current process. It is usually one of
        /// these values:
        ///
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level.
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level.
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level.
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level.
        ///
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception
        /// with the last error code.
        /// </exception>
        private static int GetProcessIntegrityLevel()
        {
            unsafe
            {
                int IL = -1;
                IntPtr hToken = IntPtr.Zero;
                CloseHandleSafeHandle hTokenHabndle = CloseHandleSafeHandle.Null;
                uint cbTokenIL = 0;
                IntPtr pTokenIL = IntPtr.Zero;

                try
                {

                    // Open the access token of the current process with TOKEN_QUERY.
                    if (!PInvoke.OpenProcessToken(new CloseHandleSafeHandle(Process.GetCurrentProcess().Handle), TOKEN_QUERY, out hToken))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    hTokenHabndle = new CloseHandleSafeHandle(hToken);
                    // Then we must query the size of the integrity level information
                    // associated with the token. Note that we expect GetTokenInformation
                    // to return false with the ERROR_INSUFFICIENT_BUFFER error code
                    // because we've given it a null buffer. On exit cbTokenIL will tell
                    // the size of the group information.
                    if (!PInvoke.GetTokenInformation(hTokenHabndle,
                        TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, null, 0,
                        out cbTokenIL))
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error != ERROR_INSUFFICIENT_BUFFER)
                        {
                            // When the process is run on operating systems prior to
                            // Windows Vista, GetTokenInformation returns false with the
                            // ERROR_INVALID_PARAMETER error code because
                            // TokenIntegrityLevel is not supported on those OS's.
                            throw new Win32Exception(error);
                        }
                    }

                    // Now we allocate a buffer for the integrity level information.
                    pTokenIL = Marshal.AllocHGlobal((int)cbTokenIL);
                    if (pTokenIL == IntPtr.Zero)
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    // Now we ask for the integrity level information again. This may fail
                    // if an administrator has added this account to an additional group
                    // between our first call to GetTokenInformation and this one.
                    if (!PInvoke.GetTokenInformation(hTokenHabndle,
                        TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL.ToPointer(), cbTokenIL,
                        out cbTokenIL))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                    TOKEN_MANDATORY_LABEL tokenIL = (TOKEN_MANDATORY_LABEL)
                        Marshal.PtrToStructure(pTokenIL, typeof(TOKEN_MANDATORY_LABEL));

                    IntPtr pIL = new IntPtr(PInvoke.GetSidSubAuthority(tokenIL.Label.Sid, 0));
                    IL = Marshal.ReadInt32(pIL);
                }
                finally
                {
                    // Centralized cleanup for all allocated resources. Clean up only
                    // those which were allocated, and clean them up in the right order.


                    if (hTokenHabndle != null)
                        hTokenHabndle.Close();

                    if (pTokenIL != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pTokenIL);
                        pTokenIL = IntPtr.Zero;
                        cbTokenIL = 0;
                    }
                }

                return IL;
            }
        }
    }
}