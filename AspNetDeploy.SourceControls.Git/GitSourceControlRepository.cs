using System;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using GitSharp;
using GitSharp.Commands;

namespace AspNetDeploy.SourceControls.Git
{
    public class GitSourceControlRepository : ISourceControlRepository
    {
        public LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path)
        {
            Repository repository;

            if (Repository.IsValid(path))
            {
                repository = new Repository(path);
            }
            else
            {
                CloneCommand command = new CloneCommand
                {
                    Source = sourceControlVersion.SourceControl.GetStringProperty("URL") + "/" + sourceControlVersion.GetStringProperty("URL"),
                    GitDirectory = path,
                    
                };
                
                command.Execute();
            }

            throw new NotImplementedException();
        }

        public TestSourceResult TestConnection(SourceControlVersion sourceControlVersion)
        {
            throw new NotImplementedException();
        }

        public void Archive(SourceControlVersion sourceControlVersion, string path)
        {
            throw new NotImplementedException();
        }
    }
}