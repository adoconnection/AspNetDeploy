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
            if (!Directory.Exists(path))
            {
                return;
            }

            this.DisableReadOnly(new DirectoryInfo(path));

            Directory.Delete(path, true);
        }

        private void DisableReadOnly(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                if (file.IsReadOnly)
                {
                    file.IsReadOnly = false;
                }
            }

            foreach (var subdirectory in directory.GetDirectories())
            {
                this.DisableReadOnly(subdirectory);
            }
        }

        private LoadSourcesResult LoadSourcesFromScratch(SourceControlVersion sourceControlVersion, string path) 
        {
            Directory.CreateDirectory(path);

            string branch = sourceControlVersion.GetStringProperty("Branch");

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = sourceControlVersion.SourceControl.GetStringProperty("Login"),
                    Password = sourceControlVersion.SourceControl.GetStringProperty("AccessToken")
                },
                BranchName = branch
            };

            Repository.Clone(sourceControlVersion.SourceControl.GetStringProperty("URL"), path, cloneOptions);

            ObjectId id = ObjectId.Zero;

            using (var repo = new Repository(path))
            {
                // Ensure we are on the correct branch
                Branch localBranch = repo.Branches[branch];

                if (localBranch == null)
                {
                    // Fetch the branch if not present locally
                    Commands.Fetch(repo, "origin", new[] { branch }, new FetchOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = sourceControlVersion.SourceControl.GetStringProperty("Login"),
                            Password = sourceControlVersion.SourceControl.GetStringProperty("AccessToken")
                        }
                    }, null);

                    // Create and checkout the branch locally
                    localBranch = repo.CreateBranch(branch, repo.Branches[$"origin/{branch}"].Tip);
                    repo.Branches.Update(localBranch, b => b.TrackedBranch = $"refs/remotes/origin/{branch}");
                }

                // Checkout the branch
                Commands.Checkout(repo, localBranch);

                id = repo.Commits.QueryBy(new CommitFilter()).First().Id;
            }

            return new LoadSourcesResult()
            {
                RevisionId = id.ToString(),
            };
        }

        private LoadSourcesResult LoadSourcesWithUpdate(SourceControlVersion sourceControlVersion, string path)
        {
            // Get the branch to update
            string branch = sourceControlVersion.GetStringProperty("Branch");

            using (var repo = new Repository(path))
            {
                // Ensure the branch exists locally
                Branch localBranch = repo.Branches[branch];

                if (localBranch == null)
                {
                    // If the branch is missing locally, fetch it and create a tracking branch
                    Commands.Fetch(repo, "origin", new[] { branch }, new FetchOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = sourceControlVersion.SourceControl.GetStringProperty("Login"),
                            Password = sourceControlVersion.SourceControl.GetStringProperty("AccessToken")
                        }
                    }, null);

                    localBranch = repo.CreateBranch(branch, repo.Branches[$"origin/{branch}"].Tip);
                    repo.Branches.Update(localBranch, b => b.TrackedBranch = $"refs/remotes/origin/{branch}");
                }

                // Check out the branch if it's not already checked out
                if (repo.Head.FriendlyName != branch)
                {
                    Commands.Checkout(repo, localBranch);
                }

                // Pull the latest changes
                Commands.Pull(repo, new Signature("AspNetDeploy", "system@example.com", DateTimeOffset.Now), new PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = sourceControlVersion.SourceControl.GetStringProperty("Login"),
                            Password = sourceControlVersion.SourceControl.GetStringProperty("AccessToken")
                        }
                    }
                });

                // Get the latest commit ID
                ObjectId id = repo.Commits.First().Id;

                // Return the result with the latest commit ID
                return new LoadSourcesResult
                {
                    RevisionId = id.ToString(),
                };
            }
        }
    }
}