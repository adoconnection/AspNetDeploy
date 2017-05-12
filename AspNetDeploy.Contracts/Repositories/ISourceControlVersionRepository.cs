using System.Collections.Generic;
using AspNetDeploy.Contracts.Entities;

namespace AspNetDeploy.Contracts.Repositories
{
    public interface ISourceControlVersionRepository
    {
        IList<ISourceControlVersion> ListSourceControlVersions(SelectMode selectMode = SelectMode.ActiveOnly);
    }
}