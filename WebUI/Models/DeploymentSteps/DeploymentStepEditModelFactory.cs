using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class DeploymentStepEditModelFactory
    {
        private readonly AspNetDeployEntities entities;

        public DeploymentStepEditModelFactory()
        {
            this.entities = entities;
        }

        public DeploymentStepModel Create(DeploymentStep deploymentStep)
        {
            WebSiteDeploymentStepModel model = new WebSiteDeploymentStepModel();

            model.OrderIndex = deploymentStep.OrderIndex;

            return model;
        }
    }
}