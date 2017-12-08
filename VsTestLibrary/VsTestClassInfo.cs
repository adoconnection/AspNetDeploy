using System;
using System.Collections.Generic;

namespace VsTestLibrary
{
    public class VsTestClassInfo
    {
        public Type Type { get; internal set; }
        public string InitializeMethod { get; internal set; }
        public string CleanupMethod { get; internal set; }
        public IList<string> TestMethods { get; internal set; }
    }
}