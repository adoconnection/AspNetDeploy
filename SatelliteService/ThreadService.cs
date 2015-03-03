using System;
using System.Threading;

namespace SatelliteService
{
    public static class ThreadService
    {
        public delegate bool IsOperationComplete();

        public static void SleepUntil(IsOperationComplete isOperationComplete, int timeout)
        {
            DateTime startTime = DateTime.Now;

            while (!isOperationComplete() && (DateTime.Now - startTime).TotalSeconds <= timeout)
            {
                Thread.Sleep(100);
            }
        } 
    }
}