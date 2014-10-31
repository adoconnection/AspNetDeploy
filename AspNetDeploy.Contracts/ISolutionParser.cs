using System.Collections;
using System.Collections.Generic;

namespace AspNetDeploy.Contracts
{
    public interface ISolutionParser
    {
        IList<ISolutionProject> Parse(string solutionFilePath);
    }
}