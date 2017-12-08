using System;

namespace VsTestLibrary
{
    public class VsTestRunResult
    {
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
    }
}