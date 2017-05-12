﻿using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IBuildServiceFactory
    {
        IBuildService Create(ProjectType project);
    }
}