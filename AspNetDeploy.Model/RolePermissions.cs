﻿using System.Collections.Generic;

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
                    UserRoleAction.ReleaseCancel,

                    UserRoleAction.VersionCreate,
                    UserRoleAction.ViewLogs,
                }
            },
            {
                UserRole.PowerDeveloper,
                new List<UserRoleAction>()
                {
                    UserRoleAction.DeploymentChangeSteps,

                    UserRoleAction.ReleasePublishTest,
                    UserRoleAction.ReleasePublishLive,
                    UserRoleAction.ReleaseCancel,

                    UserRoleAction.VersionCreate,
                    UserRoleAction.ViewLogs,

                    UserRoleAction.EnvironmentsManage,
                    UserRoleAction.EnvironmentChangeVariables,

                    UserRoleAction.SourceVersionsManage
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
                    UserRoleAction.ReleasePublishLive,
                    UserRoleAction.ReleaseCancel,

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
                    UserRoleAction.ReleaseCancel,

                    UserRoleAction.VersionCreate,

                    UserRoleAction.EnvironmentsManage,
                    UserRoleAction.EnvironmentChangeVariables,

                    UserRoleAction.ManageUsers,
                    UserRoleAction.ViewLogs,

                    UserRoleAction.SourceVersionsManage
                }
            },
        };
    }
}