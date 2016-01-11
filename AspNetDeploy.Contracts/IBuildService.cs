using System;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IBuildService
    {
        BuildSolutionResult Build(string projectOrSolutionFilePath, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger);
    }
}