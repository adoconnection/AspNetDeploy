using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SatelliteService
{
    public class MemoryHelper
    {
        private PerformanceCounter MemoryCouter = new PerformanceCounter("Memory", "Available MBytes");
        private PerformanceCounter PercentCommitedInUseCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint) Marshal.SizeOf(typeof (MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        public double GetTotalRamMB()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();

            if (GlobalMemoryStatusEx(memStatus))
            {
                return memStatus.ullTotalPhys/1024/1024;
            }

            return 0;
        }

        public double GetAvailableMemoryMB()
        {
            return MemoryCouter.NextValue();
        }
    }
}