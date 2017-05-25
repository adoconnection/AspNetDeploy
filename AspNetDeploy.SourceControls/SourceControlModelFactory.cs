using System;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls.SVN;
using SourceControls.FileSystem;

namespace AspNetDeploy.SourceControls
{
    public class SourceControlModelFactory
    {
        public ISourceControlModel Create(SourceControlType type)
        {
            switch (type)
            {
                case SourceControlType.Svn:
                    return new SvnSourceControlModel();

                case SourceControlType.Git:
                    throw new NotSupportedException();

                case SourceControlType.FileSystem:
                    return new FileSystemSourceControlModel();

                default:
                    throw new AspNetDeployException("Unknown SourceControlType: " + type);
            }
        }
    }
}