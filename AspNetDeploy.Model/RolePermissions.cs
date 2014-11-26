using System.Collections.Generic;

namespace AspNetDeploy.Model
{
    public class RolePermissions
    {
        public static readonly IDictionary<UserRole, IList<UserRoleAction>> MappingDictionary = new Dictionary<UserRole, IList<UserRoleAction>>
        {
            {
                UserRole.Observer,
                new List<UserRoleAction>()
                {

                }
            },
            {
                UserRole.Developer,
                new List<UserRoleAction>()
                {
                    UserRoleAction.DeploymentChangeSteps,

                    UserRoleAction.ReleasePublishTest,

                    UserRoleAction.VersionCreate,
                }
            },
            {
                UserRole.Tester,
                new List<UserRoleAction>()
                {
                    UserRoleAction.ReleaseApprove,
                    UserRoleAction.ReleasePublishTest
                }
            },
            {
                UserRole.Publisher,
                new List<UserRoleAction>()
                {
                    UserRoleAction.ReleaseApprove,
                    UserRoleAction.ReleasePublishTest,
                    UserRoleAction.ReleasePublishLive
                }
            },
            {
                UserRole.Admin,
                new List<UserRoleAction>()
                {
                    UserRoleAction.DeploymentChangeSteps,

                    UserRoleAction.ReleaseApprove,
                    UserRoleAction.ReleasePublishTest,
                    UserRoleAction.ReleasePublishLive,

                    UserRoleAction.VersionCreate,

                    UserRoleAction.EnvironmentCreate,
                    UserRoleAction.EnvironmentChangeVariables,

                    UserRoleAction.ManageUsers,

                }
            },
        };
    }
}