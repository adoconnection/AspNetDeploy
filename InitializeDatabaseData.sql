USE [AspNetDeploy]

------------------------------------

SET IDENTITY_INSERT [dbo].[User] ON 

GO
INSERT [dbo].[User] ([Id], [Guid], [Name], [Email], [Password], [RoleId], [IsDisabled], [ApiKey]) VALUES (1, N'dddd3d52-526b-4bc5-afc1-c0fd19454b38', N'admin', N'r.vinorajesh@gmail.com', N'123', 10000, 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO

------------------------------------

GO
SET IDENTITY_INSERT [dbo].[SourceControl] ON 
GO
INSERT [dbo].[SourceControl] ([Id], [Name], [TypeId], [IsDeleted], [OrderIndex]) VALUES (1, N'Demo solution', 1, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[SourceControl] OFF
GO

------------------------------------

GO
SET IDENTITY_INSERT [dbo].[SourceControlProperty] ON 
GO
INSERT [dbo].[SourceControlProperty] ([Id], [SourceControlId], [Key], [Value]) VALUES (1, 2, N'URL', N'https://svn.office:8443/svn/YourProjectRoot')
GO
INSERT [dbo].[SourceControlProperty] ([Id], [SourceControlId], [Key], [Value]) VALUES (2, 2, N'Login', N'SvnUser')
GO
INSERT [dbo].[SourceControlProperty] ([Id], [SourceControlId], [Key], [Value]) VALUES (3, 2, N'Password', N'SvnPass')
GO
SET IDENTITY_INSERT [dbo].[SourceControlProperty] OFF
GO

------------------------------------

SET IDENTITY_INSERT [dbo].[SourceControlVersion] ON 
GO
INSERT [dbo].[SourceControlVersion] ([Id], [SourceControlId], [ParentVersionId], [Name], [OrderIndex], [IsHead], [IsArchivedId]) VALUES (1, 1, NULL, N'trunk', 0, 1, 1) -- trunk is a display name
GO
SET IDENTITY_INSERT [dbo].[SourceControlVersion] OFF
GO

------------------------------------

SET IDENTITY_INSERT [dbo].[SourceControlVersionProperty] ON 
GO
INSERT [dbo].[SourceControlVersionProperty] ([Id], [SourceControlVersionId], [Key], [Value]) VALUES (1, 1, N'URL', N'trunk')
GO
SET IDENTITY_INSERT [dbo].[SourceControlVersionProperty] OFF
GO


------------------------------------


SET IDENTITY_INSERT [dbo].[Environment] ON 

GO
INSERT [dbo].[Environment] ([Id], [Name]) VALUES (1, N'Test')
GO
INSERT [dbo].[Environment] ([Id], [Name]) VALUES (2, N'Staging')
GO
INSERT [dbo].[Environment] ([Id], [Name]) VALUES (3, N'Live')
GO
SET IDENTITY_INSERT [dbo].[Environment] OFF
GO

------------------------------------

SET IDENTITY_INSERT [dbo].[EnvironmentProperty] ON 
GO
INSERT [dbo].[EnvironmentProperty] ([Id], [EnvironmentId], [Key], [Value]) VALUES (1, 1, N'AllowTestDeployment', N'true')
GO
INSERT [dbo].[EnvironmentProperty] ([Id], [EnvironmentId], [Key], [Value]) VALUES (2, 2, N'AllowTestDeployment', N'true')
GO
INSERT [dbo].[EnvironmentProperty] ([Id], [EnvironmentId], [Key], [Value]) VALUES (3, 3, N'AllowTestDeployment', N'false')
GO
INSERT [dbo].[EnvironmentProperty] ([Id], [EnvironmentId], [Key], [Value]) VALUES (4, 3, N'HideSensitiveValues', N'true')
GO
SET IDENTITY_INSERT [dbo].[EnvironmentProperty] OFF
GO

------------------------------------

INSERT [dbo].[NextEnvironment] ([SourceEnvironmentId], [NextEnvironmentId]) VALUES (1, 2)
GO
INSERT [dbo].[NextEnvironment] ([SourceEnvironmentId], [NextEnvironmentId]) VALUES (2, 3)
GO

------------------------------------

SET IDENTITY_INSERT [dbo].[Machine] ON 

GO
INSERT [dbo].[Machine] ([Id], [Name], [URL], [Login], [Password]) VALUES (1, N'Lake', N'https://lake.server.office:8090/AspNetDeploySatellite', N'satellite login', N'satellite secret key')
GO
INSERT [dbo].[Machine] ([Id], [Name], [URL], [Login], [Password]) VALUES (2, N'Wind', N'https://wind.server.office:8090/AspNetDeploySatellite', N'satellite login', N'satellite secret key')
GO
SET IDENTITY_INSERT [dbo].[Machine] OFF
GO

------------------------------------

GO
SET IDENTITY_INSERT [dbo].[MachineRole] ON 

GO
INSERT [dbo].[MachineRole] ([Id], [Name]) VALUES (1, N'Web')
GO
INSERT [dbo].[MachineRole] ([Id], [Name]) VALUES (2, N'SQL')
GO
INSERT [dbo].[MachineRole] ([Id], [Name]) VALUES (3, N'SQL-Primary')
GO
SET IDENTITY_INSERT [dbo].[MachineRole] OFF
GO

------------------------------------

INSERT [dbo].[MachineToMachineRole] ([MachineId], [MachineRoleId]) VALUES (1, 1)
GO
INSERT [dbo].[MachineToMachineRole] ([MachineId], [MachineRoleId]) VALUES (1, 2)
GO
INSERT [dbo].[MachineToMachineRole] ([MachineId], [MachineRoleId]) VALUES (2, 1)
GO
INSERT [dbo].[MachineToMachineRole] ([MachineId], [MachineRoleId]) VALUES (2, 2)
GO

------------------------------------

INSERT [dbo].[MachineToEnvironment] ([MachineId], [EnvironmentId]) VALUES (1, 1)
GO
INSERT [dbo].[MachineToEnvironment] ([MachineId], [EnvironmentId]) VALUES (2, 2)
GO

------------------------------------



------------------------------------