using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.SourceControls.SVN
{
    public class SvnSourceControlModel : ISourceControlModel
    {
        public Func<SourceControl, object> DetailsSerializer { get; } = sourceControl => new
        {
            id = sourceControl.Id,
            name = sourceControl.Name,
            type = sourceControl.Type,
            url = sourceControl.Properties.Where(p => p.Key == "URL").Select(p => p.Value).FirstOrDefault(),
            login = sourceControl.Properties.Where(p => p.Key == "login").Select(p => p.Value).FirstOrDefault(),
            password = sourceControl.Properties.Where(p => p.Key == "password").Select(p => p.Value).FirstOrDefault()
        };

        public Action<SourceControl, dynamic> PropertyUpdater { get; } = (sourceControl, data) =>
        {
            sourceControl.SetStringProperty("URL", (string) data.url);
            sourceControl.SetStringProperty("login", (string) data.login);
            sourceControl.SetStringProperty("password", (string) data.password);
        };

        public Func<dynamic, IDictionary<string, string>> PropertyValidator { get; } = data =>
        {
            return new Dictionary<string, string>();
        };

        public Func<SourceControlVersion, object> VersionDetailsSerializer { get; } = scv => new
        {
            id = scv.Id,
            parentId = scv.ParentVersionId,
            sourceControlId = scv.SourceControlId,
            scv.Name,
            workState = scv.WorkState,
            url = scv.Properties.Where( p => p.Key == "URL").Select( p => p.Value).FirstOrDefault()
        };

        public Action<SourceControlVersion, dynamic> VersionPropertyUpdater { get; } = (sourceControlVersion, data) =>
        {
            sourceControlVersion.SetStringProperty("URL", ((string)data.url).Trim('/'));
        };

        public Func<dynamic, IDictionary<string, string>> VersionPropertyValidator { get; } = (data) =>
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty((string)data.url))
            {
                result.Add("URL", "Required");
            }

            return result;
        };
    }
}