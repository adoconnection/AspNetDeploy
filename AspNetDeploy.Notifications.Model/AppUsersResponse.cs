using System;
using System.Collections.Generic;

namespace AspNetDeploy.Notifications.Model
{
    public class AppUsersResponse : AppResponse
    {
        public IList<Guid> UserGuids { get; set; }
    }
}