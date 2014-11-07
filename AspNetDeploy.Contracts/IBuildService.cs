using System;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IBuildService
    {
        BuildSolutionResult Build(string solutionFilePath, Action<string> projectBuildStarted, Action<string, bool> projectBuildComplete);
    }
}