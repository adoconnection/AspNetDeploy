using System;
using AspNetDeploy.Notifications.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls
{
    public class SourceControlsList : IAppCommandProcessor
    {
        private readonly AspNetDeploy.Contracts.Repositories.ISourceControlRepository sourceControlRepository;

        public SourceControlsList(AspNetDeploy.Contracts.Repositories.ISourceControlRepository sourceControlRepository)
        {
            this.sourceControlRepository = sourceControlRepository;
        }

        public string CommandName
        {
            get
            {
                return "App/SourceControls/List";
            }
        }

        public void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;
            Guid productGuid = (Guid)message.Data.productGuid;

            this.sourceControlRepository.

            ClieneEntities entities = new ClieneEntities();
            ProductRepository productRepository = new ProductRepository(entities);
            ProductEmployee productEmployee = productRepository.UserProducts(userGuid).Include("Product.ApiKeys").First(pe => pe.ProductGuid == productGuid);

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = message.ConnectionId,
                Name = "App/Product/ApiKeys/List",
                Data = new
                {
                    guid = productGuid,
                    apiKeys = productEmployee.Product.ApiKeys.Select(apiKey => new
                    {
                        guid = apiKey.Guid,
                        key = apiKey.Key
                    }).ToList()
                }
            });
        }
    }
}