using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;


namespace AspNetDeploy.SourceControls.Git
{
    public class GitSourceControlRepository : ISourceControlRepository
    {
        public LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    return this.LoadSourcesFromScratch(sourceControlVersion, path);
                }

                return this.LoadSourcesWithUpdate(sourceControlVersion, path);
            }
            catch (Exception e)
            {
                throw new AspNetDeployException("GitClient failed to load sources", e);
            }
        }

        public LoadSourcesInfoResult LoadSourcesInfo(SourceControlVersion sourceControlVersion, string path)
        {
            return new LoadSourcesInfoResult
            {
                SourcesInfos = new List<SourcesInfo>()
            };
        }

        public TestSourceResult TestConnection(SourceControlVersion sourceControlVersion)
        {
            string gitUserName = sourceControlVersion.SourceControl.GetStringProperty("Login");
            string gitAccessToken = sourceControlVersion.SourceControl.GetStringProperty("AccessToken");

            try
            {
                Repository.ListRemoteReferences(sourceControlVersion.SourceControl.GetStringProperty("URL"),
                    (url, fromUrl, types) => new UsernamePasswordCredentials()
                    {
                        Username = gitUserName,
                        Password = gitAccessToken
                    });
            }
            catch (Exception e)
            {
                return new TestSourceResult()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }

            return new TestSourceResult()
            {
                IsSuccess = true
            };
        }

        public void Archive(SourceControlVersion sourceControlVersion, string path)
        {
            throw new NotImplementedException();
        }

        private LoadSourcesResult LoadSourcesFromScratch(SourceControlVersion sourceControlVersion, string path) 
        {
            Directory.CreateDirectory(path);

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = sourceControlVersion.SourceControl.GetStringProperty("Login"),
                    Password = sourceControlVersion.SourceControl.GetStringProperty("AccessToken")
                },
            };

            Repository.Clone(sourceControlVersion.SourceControl.GetStringProperty("URL"), path, cloneOptions);

            ObjectId id = ObjectId.Zero;

            using (var repo = new Repository(path))
            {
                id = repo.Commits.QueryBy(new CommitFilter()).First().Id;
            }

            return new LoadSourcesResult()
            {
                RevisionId = id.ToString(),
            };
        }

        private LoadSourcesResult LoadSourcesWithUpdate(SourceControlVersion sourceControlVersion, string path)
        {
            ObjectId id = ObjectId.Zero;

            FetchOptions fetchOptions = new FetchOptions()
            {
                CredentialsProvider = (url, fromUrl, types) => new UsernamePasswordCredentials()
                {
                    Username = sourceControlVersion.SourceControl.GetStringProperty("Login"),
                    Password = sourceControlVersion.SourceControl.GetStringProperty("AccessToken")
                }
            };

            using (Repository repo = new Repository(path))
            {
                string remoteName = repo.Head.RemoteName;
                Remote remote = repo.Network.Remotes[remoteName];

                Commands.Fetch(repo, remoteName, remote.FetchRefSpecs.Select(rf => rf.Specification), fetchOptions, "");

                foreach (Branch branch in repo.Branches)
                {
                    MergeResult merge = repo.Merge(branch, new Signature("Dnaiw", "email", DateTimeOffset.Now), new MergeOptions());
                }

                id = repo.Commits.QueryBy(new CommitFilter()).First().Id;
            }

            return new LoadSourcesResult()
            {
                RevisionId = id.ToString(),
            };
        }
    }
}