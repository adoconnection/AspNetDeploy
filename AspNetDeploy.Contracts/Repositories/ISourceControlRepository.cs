using System.Collections.Generic;
using AspNetDeploy.Contracts.Entities;

namespace AspNetDeploy.Contracts.Repositories
{
    public interface ISourceControlRepository
    {
        IList<ISourceControl> ListSourceControls(SelectMode selectMode = SelectMode.ActiveOnly);
    }
}