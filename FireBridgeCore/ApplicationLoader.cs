using FireBridgeCore.Kernel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace FireBridgeCore
{
    public static class WinApi
    {
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int TOKEN_QUERY = 0x0008;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr GetSidSubAuthority(IntPtr sid, UInt32 subAuthorityIndex);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            int TokenInformationLength,
            out int ReturnLength);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        public enum TOKEN_INFORMATION_CLASS
        {
            /// <summary>
            /// The buffer receives a TOKEN_USER structure that contains the user account of the token.
            /// </summary>
            TokenUser = 1,

            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
            /// </summary>
            TokenGroups,

            /// <summary>
            /// The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
            /// </summary>
            TokenPrivileges,

            /// <summary>
            /// The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.
            /// </summary>
            TokenOwner,

            /// <summary>
            /// The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.
            /// </summary>
            TokenPrimaryGroup,

            /// <summary>
            /// The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
            /// </summary>
            TokenDefaultDacl,

            /// <summary>
            /// The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.
            /// </summary>
            TokenSource,

            /// <summary>
            /// The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
            /// </summary>
            TokenType,

            /// <summary>
            /// The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.
            /// </summary>
            TokenImpersonationLevel,

            /// <summary>
            /// The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
            /// </summary>
            TokenStatistics,

            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
            /// </summary>
            TokenRestrictedSids,

            /// <summary>
            /// The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with the token.
            /// </summary>
            TokenSessionId,

            /// <summary>
            /// The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.
            /// </summary>
            TokenGroupsAndPrivileges,

            /// <summary>
            /// Reserved.
            /// </summary>
            TokenSessionReference,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
            /// </summary>
            TokenSandBoxInert,

            /// <summary>
            /// Reserved.
            /// </summary>
            TokenAuditPolicy,

            /// <summary>
            /// The buffer receives a TOKEN_ORIGIN value.
            /// </summary>
            TokenOrigin,

            /// <summary>
            /// The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
            /// </summary>
            TokenElevationType,

            /// <summary>
            /// The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this token.
            /// </summary>
            TokenLinkedToken,

            /// <summary>
            /// The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
            /// </summary>
            TokenElevation,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
            /// </summary>
            TokenHasRestrictions,

            /// <summary>
            /// The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the token.
            /// </summary>
            TokenAccessInformation,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
            /// </summary>
            TokenVirtualizationAllowed,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
            /// </summary>
            TokenVirtualizationEnabled,

            /// <summary>
            /// The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level.
            /// </summary>
            TokenIntegrityLevel,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
            /// </summary>
            TokenUIAccess,

            /// <summary>
            /// The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
            /// </summary>
            TokenMandatoryPolicy,

            /// <summary>
            /// The buffer receives the token's logon security identifier (SID).
            /// </summary>
            TokenLogonSid,

            /// <summary>
            /// The maximum value for this enumeration
            /// </summary>
            MaxTokenInfoClass
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_MANDATORY_LABEL
        {
            public SID_AND_ATTRIBUTES label;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength);

        [DllImport("advapi32.dll")]
        public static extern uint GetLengthSid(IntPtr ptrSid);

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int Length;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        public const int MAXIMUM_ALLOWED = 0x2000000;
        public const int NORMAL_PRIORITY_CLASS = 0x20;
        public const int CREATE_NEW_CONSOLE = 0x00000010;
        public const int SE_GROUP_INTEGRITY = 0x00000020;
        public const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        public const int TOKEN_DUPLICATE = 0x0002;

        public enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        public enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }

        [DllImport("kernel32.dll")]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

        [DllImport("kernel32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public static extern bool DuplicateTokenEx(IntPtr hExistingToken, Int32 dwDesiredAccess,
                     ref SECURITY_ATTRIBUTES lpThreadAttributes,
                     Int32 ImpersonationLevel, Int32 dwTokenType,
                     ref IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);
    }

    public static class ApplicationLoader
    {
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

        public static Process StartProcess(string name, string[] parameters, IIntegrityLevel il)
        {
            return StartProcess(name, parameters, WinApi.WTSGetActiveConsoleSessionId(), il);
        }

        public static Process StartProcess(string name, string[] parameters, uint sessionID, IIntegrityLevel il)
        {
            IntPtr hPToken = IntPtr.Zero;
            IntPtr hPSid = IntPtr.Zero;
            IntPtr hPProcess = IntPtr.Zero;
            string integrityString = SidStringFromIL(il);
            WinApi.PROCESS_INFORMATION procInfo;

            WinApi.SECURITY_ATTRIBUTES sa = new WinApi.SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            try
            {
                if (sessionID == 0) //duplicate myself for token
                {
                    if (!WinApi.OpenProcessToken(Process.GetCurrentProcess().Handle, WinApi.TOKEN_DUPLICATE, ref hPProcess))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    if (!WinApi.DuplicateTokenEx(
                            hPProcess,
                            WinApi.MAXIMUM_ALLOWED,
                            ref sa,
                            (int)WinApi.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                            (int)WinApi.TOKEN_TYPE.TokenPrimary,
                            ref hPToken)
                            )
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                else //or get token for session
                {
                    if (!WinApi.WTSQueryUserToken(sessionID, out hPToken))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (!WinApi.ConvertStringSidToSid(integrityString, out hPSid))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var sidLenght = WinApi.GetLengthSid(hPSid);

                WinApi.TOKEN_MANDATORY_LABEL TIL;
                TIL.label.Attributes = WinApi.SE_GROUP_INTEGRITY;
                TIL.label.Sid = hPSid;

                IntPtr hPTokenInf = Marshal.AllocHGlobal(Marshal.SizeOf<WinApi.TOKEN_MANDATORY_LABEL>());
                Marshal.StructureToPtr(TIL, hPTokenInf, false);

                if (!WinApi.SetTokenInformation(
                    hPToken,
                    WinApi.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel,
                    hPTokenInf,
                    Marshal.SizeOf<WinApi.TOKEN_MANDATORY_LABEL>() + (int)sidLenght))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                WinApi.STARTUPINFO si = new WinApi.STARTUPINFO();
                si.cb = (int)Marshal.SizeOf(si);

                IntPtr UserEnvironment = IntPtr.Zero;

                uint dwCreationFlags = WinApi.NORMAL_PRIORITY_CLASS; //| WinApi.CREATE_NEW_CONSOLE | WinApi.CREATE_UNICODE_ENVIRONMENT;

                // create a new process in the current user's logon session
                if (!WinApi.CreateProcessAsUser(hPToken,        // client's access token
                                                null,                   // file to execute
                                                name + " " + string.Join(" ", parameters),        // command line
                                                ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                                                ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                                false,                  // handles are not inheritable
                                                dwCreationFlags,        // creation flags
                                                IntPtr.Zero,            // pointer to new environment block 
                                                null,                   // name of current directory 
                                                ref si,                 // pointer to STARTUPINFO structure
                                                out procInfo            // receives information about new process
                                                ))
                    throw new Win32Exception(Marshal.GetLastWin32Error());


                Process process = Process.GetProcessById((int)procInfo.dwProcessId);
                process.EnableRaisingEvents = true;
                return process;
            }
            catch (Exception)
            {

            }
            finally
            {
                if (hPToken != IntPtr.Zero && !WinApi.CloseHandle(hPToken))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                if (hPProcess != IntPtr.Zero && !WinApi.CloseHandle(hPProcess))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return null;
        }
    }


    public static class IntegrityLevelHelper
    {

        const int SECURITY_MANDATORY_UNTRUSTED_RID = (0x00000000);
        const int SECURITY_MANDATORY_LOW_RID = (0x00001000);
        const int SECURITY_MANDATORY_MEDIUM_RID = (0x00002000);
        const int SECURITY_MANDATORY_HIGH_RID = (0x00003000);
        const int SECURITY_MANDATORY_SYSTEM_RID = (0x00004000);
        const int SECURITY_MANDATORY_PROTECTED_PROCESS_RID = (0x00005000);

        public static IIntegrityLevel GetCurrentIntegrity()
        {
            int il = GetProcessIntegrityLevel();

            switch(il)
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
            int IL = -1;
            IntPtr hToken = IntPtr.Zero;
            int cbTokenIL = 0;
            IntPtr pTokenIL = IntPtr.Zero;

            try
            {

                // Open the access token of the current process with TOKEN_QUERY.
                if (!WinApi.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    WinApi.TOKEN_QUERY, ref hToken))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Then we must query the size of the integrity level information
                // associated with the token. Note that we expect GetTokenInformation
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code
                // because we've given it a null buffer. On exit cbTokenIL will tell
                // the size of the group information.
                if (!WinApi.GetTokenInformation(hToken,
                    WinApi.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0,
                    out cbTokenIL))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != WinApi.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to
                        // Windows Vista, GetTokenInformation returns false with the
                        // ERROR_INVALID_PARAMETER error code because
                        // TokenIntegrityLevel is not supported on those OS's.
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information.
                pTokenIL = Marshal.AllocHGlobal(cbTokenIL);
                if (pTokenIL == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Now we ask for the integrity level information again. This may fail
                // if an administrator has added this account to an additional group
                // between our first call to GetTokenInformation and this one.
                if (!WinApi.GetTokenInformation(hToken,
                    WinApi.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL, cbTokenIL,
                    out cbTokenIL))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                WinApi.TOKEN_MANDATORY_LABEL tokenIL = (WinApi.TOKEN_MANDATORY_LABEL)
                    Marshal.PtrToStructure(pTokenIL, typeof(WinApi.TOKEN_MANDATORY_LABEL));

                IntPtr pIL = WinApi.GetSidSubAuthority(tokenIL.label.Sid, 0);
                IL = Marshal.ReadInt32(pIL);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. Clean up only
                // those which were allocated, and clean them up in the right order.


                if (hToken != IntPtr.Zero && !WinApi.CloseHandle(hToken))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

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
