using System;
using System.Globalization;
using System.IO;
using System.Net;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using SharpSvn;

namespace AspNetDeploy.SourceControls.SVN
{
    public class SvnSourceControlRepository : ISourceControlRepository
    {
        public LoadSourcesResult LoadSources(SourceControl sourceControl, string version, string path)
        {
            NetworkCredential credentials = new NetworkCredential(
                        sourceControl.GetStringProperty("Login"),
                        sourceControl.GetStringProperty("Password"));

            using (SvnClient client = new SvnClient())
            {
                client.Authentication.DefaultCredentials = credentials;

                if (!Directory.Exists(path))
                {
                    return this.LoadSourcesFromScratch(sourceControl, path, client);
                }
                
                return this.LoadSourcesWithUpdate(path, client);
            }
        }

        private LoadSourcesResult LoadSourcesWithUpdate(string path, SvnClient client)
        {
            SvnUpdateResult result;
            try
            {
                client.Update(path, out result);
            }
            catch (SvnWorkingCopyLockException e)
            {
                client.CleanUp(path);
                client.Update(path, out result);
            }

            SvnInfoEventArgs info;
            client.GetInfo(path, out info);

            return new LoadSourcesResult
            {
                RevisionId = info.LastChangeRevision.ToString(CultureInfo.InvariantCulture)
            };
        }

        private LoadSourcesResult LoadSourcesFromScratch(SourceControl sourceControl, string path, SvnClient client)
        {
            SvnUpdateResult result;
            Directory.CreateDirectory(path);

            client.CheckOut(new Uri(sourceControl.GetStringProperty("URL") + "/trunk"), path, out result);

            SvnInfoEventArgs info;
            client.GetInfo(path, out info);

            return new LoadSourcesResult
            {
                RevisionId = info.LastChangeRevision.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
