using System;
using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISolutionProject
    {
        string Name { get; }
        string ProjectFile { get; }
        Guid Guid { get; }
        ProjectType Type { get; }
        
        IList<string> ContentFiles { get; }
        IList<string> ReferenceLibraries { get; }

        IList<KeyValuePair<String, string>> GetProperties();
    }
}