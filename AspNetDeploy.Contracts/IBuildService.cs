using System;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IBuildService
    {
        BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger);
    }
}