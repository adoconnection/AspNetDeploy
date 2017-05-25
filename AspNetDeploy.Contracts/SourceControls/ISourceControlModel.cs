using System;
using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlModel
    {
        Func<SourceControl, object> DetailsSerializer { get; }
        Action<SourceControl, dynamic> PropertyUpdater { get; }
        Func<dynamic, IDictionary<string, string>> PropertyValidator { get; }

        Func<SourceControlVersion, object> VersionDetailsSerializer { get; }
        Action<SourceControlVersion, dynamic> VersionPropertyUpdater { get; }
        Func<dynamic, IDictionary<string, string>> VersionPropertyValidator { get; }
    }
}