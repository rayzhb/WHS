using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace WHS.Infrastructure
{
    public class WindowsLoadLibrary
    {
        private static readonly NLog.Logger s_log = NLog.LogManager.GetCurrentClassLogger(typeof(WindowsLoadLibrary));

        private static WindowsLoadLibrary _instance;
        private static object _lock = new object();

        private WindowsLoadLibrary()
        {
        }

        public static WindowsLoadLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new WindowsLoadLibrary();
                        }
                    }
                }
                return _instance;
            }
        }

        public IntPtr LoadLibrary(string mainDLL, List<string> DependDlls)
        {
            IntPtr libraryHandle = IntPtr.Zero;
            foreach (var depend in DependDlls)
            {
                try
                {
                    IntPtr depends_libraryHandle = IntPtr.Zero;
                    depends_libraryHandle = Win32LoadLibrary(depend);
                    if (depends_libraryHandle == IntPtr.Zero)
                    {
                        s_log.Warn(String.Format(
                           "Failed to load native library \"{0}\".\r\nCheck windows event log.",
                            depend));
                    }
                }
                catch
                {
                    s_log.Warn(String.Format(
                           "Failed to load native library \"{0}\".\r\nCheck windows event log.",
                            depend));
                }
            }
            string fileName =  mainDLL;
            if (File.Exists(fileName))
            {
                try
                {
                    s_log.Info(String.Format(
                          "Trying to load native library \"{0}\"...",
                          fileName));

                    libraryHandle = Win32LoadLibrary(fileName);
                }
                catch (Exception ex)
                {
                    var lastError = Marshal.GetLastWin32Error();
                    s_log.Error(String.Format(
                        "Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}",
                        fileName, lastError, ex));
                }
            }
            else
            {
                s_log.Warn(String.Format(CultureInfo.CurrentCulture,
                          "The native library \"{0}\" does not exist.",
                          fileName));
            }

            return libraryHandle;
        }

        public bool UnLoadLibrary(IntPtr handle)
        {
            return FreeLibrary(handle);
        }


        [DllImport("kernel32", EntryPoint = "LoadLibrary", CallingConvention = CallingConvention.Winapi,
SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr Win32LoadLibrary(string dllPath);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);
    }
}
