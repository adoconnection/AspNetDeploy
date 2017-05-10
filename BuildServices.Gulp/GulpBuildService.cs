using System;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace BuildServices.Gulp
{
    public class GulpBuildService : IBuildService
    {
        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            throw new NotImplementedException();
        }
    }
}