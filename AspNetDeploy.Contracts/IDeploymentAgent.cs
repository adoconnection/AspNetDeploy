﻿using System;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IDeploymentAgent
    {
        bool IsReady();
        bool BeginPublication(int publicationId);
        void Commit();
        void Rollback();
        void UploadPackage(string file, Action<int, int> progress = null);
        void ResetPackage();
        void ProcessDeploymentStep(DeploymentStep deploymentStep);
    }
}