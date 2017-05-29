using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace SourceControls.FileSystem
{
    public class FileSystemSourceControlModel : ISourceControlModel
    {
        public Func<SourceControl, object> DetailsSerializer { get; } = sourceControl => new
        {
            id = sourceControl.Id,
            name = sourceControl.Name,
            type = sourceControl.Type.ToString(),
            path = sourceControl.Properties.Where(p => p.Key == "Path").Select(p => p.Value).FirstOrDefault(),
        };

        public Func<SourceControl, object> ListSerializer { get; } = sourceControl => new
        {
            id = sourceControl.Id,
            name = sourceControl.Name,
            type = sourceControl.Type.ToString(),
            path = sourceControl.Properties.Where(p => p.Key == "Path").Select(p => p.Value).FirstOrDefault(),
        };

        public Action<SourceControl, dynamic> PropertyUpdater { get; } = (sourceControl, data) =>
        {
            sourceControl.SetStringProperty("Path", (string)data.url);
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
            path = scv.Properties.Where(p => p.Key == "Path").Select(p => p.Value).FirstOrDefault(),
        };

        public Func<SourceControlVersion, object> VersionListSerializer { get; } = scv => new
        {
            id = scv.Id,
            parentId = scv.ParentVersionId,
            sourceControlId = scv.SourceControlId,
            scv.Name,
            workState = scv.WorkState,
            path = scv.Properties.Where(p => p.Key == "Path").Select(p => p.Value).FirstOrDefault(),
            revision = scv.Properties.Where(p => p.Key == "Revision").Select(p => p.Value).FirstOrDefault(),
        };

        public Action<SourceControlVersion, dynamic> VersionPropertyUpdater { get; } = (sourceControlVersion, data) =>
        {
            sourceControlVersion.SetStringProperty("Path", ((string)data.url).Trim('/'));
        };

        public Func<dynamic, IDictionary<string, string>> VersionPropertyValidator { get; } = (data) =>
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty((string)data.url))
            {
                result.Add("Path", "Required");
            }

            return result;
        };
    }
}