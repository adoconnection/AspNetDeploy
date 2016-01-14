using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using SharpSvn;

namespace AspNetDeploy.SourceControls.SVN
{
    public class SvnSourceControlRepository : ISourceControlRepository
    {
        public bool CreateNewVersion(SourceControlVersion from, SourceControlVersion to)
        {
            NetworkCredential credentials = new NetworkCredential(
                        from.SourceControl.GetStringProperty("Login"),
                        from.SourceControl.GetStringProperty("Password"));

            using (SvnClient client = new SvnClient())
            {
                client.Authentication.DefaultCredentials = credentials;

                SvnTarget svnTarget = new SvnPathTarget(this.GetVersionURI(from));

                client.RemoteCopy(svnTarget, new Uri(this.GetVersionURI(to)));

                return true;
            }
        }

        public TestSourceResult TestConnection(SourceControlVersion sourceControlVersion)
        {
            NetworkCredential credentials = new NetworkCredential(
                        sourceControlVersion.SourceControl.GetStringProperty("Login"),
                        sourceControlVersion.SourceControl.GetStringProperty("Password"));

            using (SvnClient client = new SvnClient())
            {
                client.LoadConfiguration(Path.Combine(Path.GetTempPath(), "Svn"), true);

                client.Authentication.DefaultCredentials = credentials;

                try
                {
                    SvnInfoEventArgs info;
                    client.GetInfo(new Uri(this.GetVersionURI(sourceControlVersion)), out info);
                }
                catch (SvnException e)
                {
                    return new TestSourceResult()
                    {
                        IsSuccess = false,
                        ErrorMessage = e.Message
                    };
                }

                return new TestSourceResult()
                {
                    IsSuccess = true,
                };
            }
        }

        public LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path)
        {
            NetworkCredential credentials = new NetworkCredential(
                        sourceControlVersion.SourceControl.GetStringProperty("Login"),
                        sourceControlVersion.SourceControl.GetStringProperty("Password"));

            try
            {
                using (SvnClient client = new SvnClient())
                {
                    client.LoadConfiguration(Path.Combine(Path.GetTempPath(), "Svn"), true);

                    client.Authentication.DefaultCredentials = credentials;

                    if (!Directory.Exists(path))
                    {
                        return this.LoadSourcesFromScratch(sourceControlVersion, path, client);
                    }

                    return this.LoadSourcesWithUpdate(sourceControlVersion, path, client);
                }
            }
            catch (SvnException e)
            {
                throw new AspNetDeployException("SvnClient failed to load sources", e);
            }
        }

        private LoadSourcesResult LoadSourcesWithUpdate(SourceControlVersion sourceControlVersion, string path, SvnClient client)
        {
            SvnUpdateResult result;

            try
            {
                client.Update(path, out result);
            }
            catch (SvnInvalidNodeKindException e)
            {
                Directory.Delete(path, true);
                return this.LoadSourcesFromScratch(sourceControlVersion, path, client);
            }
            catch (SvnWorkingCopyException e)
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

        private LoadSourcesResult LoadSourcesFromScratch(SourceControlVersion sourceControlVersion, string path, SvnClient client)
        {
            SvnUpdateResult result;
            Directory.CreateDirectory(path);

            string uriString = this.GetVersionURI(sourceControlVersion);

            client.CheckOut(new Uri(uriString), path, out result);

            SvnInfoEventArgs info;
            client.GetInfo(path, out info);

            return new LoadSourcesResult
            {
                RevisionId = info.LastChangeRevision.ToString(CultureInfo.InvariantCulture)
            };
        }

        public void Archive(SourceControlVersion sourceControlVersion, string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private string GetVersionURI(SourceControlVersion sourceControlVersion)
        {
            return sourceControlVersion.SourceControl.GetStringProperty("URL") + "/" + sourceControlVersion.GetStringProperty("URL").TrimStart('/');
        }
    }
}
