using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IProjectTestRunner
    {
        IList<TestResult> Run(ProjectVersion projectVersion);
    }
}