using System;
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
                    Directory.CreateDirectory(path);

                    client.CheckOut(new Uri(sourceControl.GetStringProperty("URL") + "/trunk"), path);
                    return LoadSourcesResult.HasChanges;
                }
                
                SvnUpdateResult result;
                client.Update(path , out result);

                return result.HasRevision ? LoadSourcesResult.HasChanges : LoadSourcesResult.NoChanges;
            }
        }
    }
}
