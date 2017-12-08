/****** Object:  Table [dbo].[AspNetDeployException]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetDeployException](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExceptionId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_AspNetDeployException] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bundle]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bundle](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSingleInstance] [bit] NOT NULL,
	[OrderIndex] [int] NOT NULL,
 CONSTRAINT [PK_Bundle] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BundleVersion]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BundleVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BundleId] [int] NOT NULL,
	[ParentBundleVersionId] [int] NULL,
	[Name] [nvarchar](500) NOT NULL,
	[OrderIndex] [int] NOT NULL,
	[IsHead] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_BundleVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BundleVersionProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BundleVersionProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BundleVersionId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_BundleVersionProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DataField]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataField](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Key] [nvarchar](4000) NOT NULL,
	[IsSensitive] [bit] NOT NULL,
	[ModeId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProjectConfigurationField] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DataFieldToBundleVersion]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataFieldToBundleVersion](
	[DataFieldId] [int] NOT NULL,
	[BundleVersionId] [int] NOT NULL,
 CONSTRAINT [PK_DataFieldToBundleVersion] PRIMARY KEY CLUSTERED 
(
	[DataFieldId] ASC,
	[BundleVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DataFieldValue]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataFieldValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DataFieldId] [int] NOT NULL,
	[Value] [nvarchar](max) NULL,
	[EnvironmentId] [int] NULL,
	[MachineId] [int] NULL,
 CONSTRAINT [PK_ProjectConfigurationValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeploymentStep]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeploymentStep](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeploymentStepId] [int] NULL,
	[BundleVersionId] [int] NOT NULL,
	[OrderIndex] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
 CONSTRAINT [PK_DeploymentStep] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeploymentStepProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeploymentStepProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeploymentStepId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_DeploymentStepProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeploymentStepToMachineRole]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeploymentStepToMachineRole](
	[DeploymentStepId] [int] NOT NULL,
	[MachineRoleId] [int] NOT NULL,
 CONSTRAINT [PK_DeploymentStepToMachineRole] PRIMARY KEY CLUSTERED 
(
	[DeploymentStepId] ASC,
	[MachineRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Environment]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Environment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_Environment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EnvironmentProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EnvironmentProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EnvironmentId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_EnvironmentProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exception]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exception](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InnerExceptionId] [int] NULL,
	[Message] [nvarchar](max) NULL,
	[Source] [nvarchar](max) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[TypeName] [nvarchar](max) NULL,
 CONSTRAINT [PK_Exception] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExceptionData]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExceptionData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExceptionId] [int] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[IsProperty] [bit] NOT NULL,
 CONSTRAINT [PK_ExceptionData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Group]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Machine]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Machine](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[URL] [nvarchar](4000) NULL,
	[Login] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_Machine] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachinePublication]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachinePublication](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PublicationId] [int] NOT NULL,
	[MachineId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[StateId] [int] NOT NULL,
	[CompletedDate] [datetime] NULL,
 CONSTRAINT [PK_MachinePublication] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachinePublicationLog]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachinePublicationLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachinePublicationId] [int] NOT NULL,
	[EventId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[DeploymentStepId] [int] NULL,
	[ExceptionId] [int] NULL,
 CONSTRAINT [PK_MachinePublicationLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachinePublicationLogProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachinePublicationLogProperty](
	[Id] [int] NOT NULL,
	[MachinePublicationLogId] [int] NOT NULL,
	[Key] [nvarchar](1000) NULL,
	[Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachinePublicationProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachinePublicationProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachinePublicationId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_MachinePublicationProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachineRole]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachineRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_MachineRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachineToEnvironment]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachineToEnvironment](
	[MachineId] [int] NOT NULL,
	[EnvironmentId] [int] NOT NULL,
 CONSTRAINT [PK_MachineToEnvironment] PRIMARY KEY CLUSTERED 
(
	[MachineId] ASC,
	[EnvironmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MachineToMachineRole]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MachineToMachineRole](
	[MachineId] [int] NOT NULL,
	[MachineRoleId] [int] NOT NULL,
 CONSTRAINT [PK_MachineToMachineRole] PRIMARY KEY CLUSTERED 
(
	[MachineId] ASC,
	[MachineRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NextEnvironment]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NextEnvironment](
	[SourceEnvironmentId] [int] NOT NULL,
	[NextEnvironmentId] [int] NOT NULL,
 CONSTRAINT [PK_NextEnvironment] PRIMARY KEY CLUSTERED 
(
	[SourceEnvironmentId] ASC,
	[NextEnvironmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Package]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Package](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BundleVersionId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[PackageDate] [datetime] NULL,
 CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PackageApprovedOnEnvironment]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackageApprovedOnEnvironment](
	[PackageId] [int] NOT NULL,
	[EnvironmentId] [int] NOT NULL,
	[ApprovedByUserId] [int] NOT NULL,
	[ApprovedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PackageApprovedOnEnvironment] PRIMARY KEY CLUSTERED 
(
	[PackageId] ASC,
	[EnvironmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PackageEntry]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackageEntry](
	[PackageId] [int] NOT NULL,
	[ProjectVersionId] [int] NOT NULL,
	[Revision] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_PackageEntry] PRIMARY KEY CLUSTERED 
(
	[PackageId] ASC,
	[ProjectVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Project]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceControlId] [int] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProjectProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectToBundle]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectToBundle](
	[ProjectId] [int] NOT NULL,
	[BundleId] [int] NOT NULL,
 CONSTRAINT [PK_ProjectToBundle] PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC,
	[BundleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectVersion]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[SourceControlVersionId] [int] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[ProjectTypeId] [int] NOT NULL,
	[SolutionFile] [nvarchar](1000) NOT NULL,
	[ProjectFile] [nvarchar](1000) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProjectVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectVersionProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectVersionProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectVersionId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProjectVersionProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectVersionToBundleVersion]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectVersionToBundleVersion](
	[ProjectVersionId] [int] NOT NULL,
	[BundleVersionId] [int] NOT NULL,
 CONSTRAINT [PK_ProjectVersionToBundleVersion] PRIMARY KEY CLUSTERED 
(
	[ProjectVersionId] ASC,
	[BundleVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Publication]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Publication](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PackageId] [int] NOT NULL,
	[EnvironmentId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ResultId] [int] NOT NULL,
	[QueuedByUserId] [int] NULL,
 CONSTRAINT [PK_Publication] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Revision]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Revision](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceControlId] [int] NOT NULL,
	[Number] [nvarchar](150) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Author] [nvarchar](150) NULL,
 CONSTRAINT [PK_Revision] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RevisionEntry]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RevisionEntry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RevisionId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_RevisionEntry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SourceControl]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceControl](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[TypeId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[OrderIndex] [int] NOT NULL,
 CONSTRAINT [PK_SourceControl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SourceControlProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceControlProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceControlId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_SourceControlProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SourceControlToGroup]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceControlToGroup](
	[SourceControlId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
 CONSTRAINT [PK_SourceControlToGroup] PRIMARY KEY CLUSTERED 
(
	[SourceControlId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SourceControlVersion]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceControlVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceControlId] [int] NOT NULL,
	[ParentVersionId] [int] NULL,
	[Name] [nvarchar](1000) NULL,
	[OrderIndex] [int] NOT NULL,
	[IsHead] [bit] NOT NULL,
	[IsArchivedId] [int] NOT NULL,
 CONSTRAINT [PK_SourceControlVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SourceControlVersionProperty]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceControlVersionProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceControlVersionId] [int] NOT NULL,
	[Key] [nvarchar](1000) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_SourceControlVersionProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestStep]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestStep](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BundleVersionId] [int] NOT NULL,
	[ProjectId] [int] NOT NULL,
	[RunTypeId] [int] NOT NULL,
	[ResultJson] [nvarchar](max) NULL,
 CONSTRAINT [PK_TestStep] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsDisabled] [bit] NOT NULL,
	[ApiKey] [nvarchar](50) NULL,
	[IsOnline] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Version]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Version](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Number] [nvarchar](50) NOT NULL,
	[Notes] [nvarchar](1000) NULL,
 CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VersionToVersion]    Script Date: 8 Dec 17 17:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VersionToVersion](
	[PreviousVersionId] [int] NOT NULL,
	[NextVersionId] [int] NOT NULL,
 CONSTRAINT [PK_VersionToVersion] PRIMARY KEY CLUSTERED 
(
	[PreviousVersionId] ASC,
	[NextVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bundle] ADD  CONSTRAINT [DF_Bundle_IsSingleInstance]  DEFAULT ((0)) FOR [IsSingleInstance]
GO
ALTER TABLE [dbo].[Bundle] ADD  CONSTRAINT [DF_Bundle_OrderIndex]  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[BundleVersion] ADD  CONSTRAINT [DF_BundleVersion_OrderIndex]  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[BundleVersion] ADD  CONSTRAINT [DF_BundleVersion_IsHead]  DEFAULT ((0)) FOR [IsHead]
GO
ALTER TABLE [dbo].[BundleVersion] ADD  CONSTRAINT [DF_BundleVersion_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[BundleVersion] ADD  CONSTRAINT [DF_BundleVersion_IsArchived]  DEFAULT ((0)) FOR [IsArchived]
GO
ALTER TABLE [dbo].[DataField] ADD  CONSTRAINT [DF_DataField_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Machine] ADD  CONSTRAINT [DF_Machine_Login]  DEFAULT ('') FOR [Login]
GO
ALTER TABLE [dbo].[Machine] ADD  CONSTRAINT [DF_Machine_Password]  DEFAULT ('') FOR [Password]
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment] ADD  CONSTRAINT [DF_PackageApprovedOnEnvironment_ApprovedById]  DEFAULT ((0)) FOR [ApprovedByUserId]
GO
ALTER TABLE [dbo].[SourceControl] ADD  CONSTRAINT [DF_SourceControl_OrderIndex]  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[SourceControlVersion] ADD  CONSTRAINT [DF_SourceControlVersion_OrderIndex]  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[SourceControlVersion] ADD  CONSTRAINT [DF_SourceControlVersion_IsHead]  DEFAULT ((0)) FOR [IsHead]
GO
ALTER TABLE [dbo].[SourceControlVersion] ADD  CONSTRAINT [DF_SourceControlVersion_IsArchived]  DEFAULT ((0)) FOR [IsArchivedId]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsOnline]  DEFAULT ((0)) FOR [IsOnline]
GO
ALTER TABLE [dbo].[AspNetDeployException]  WITH CHECK ADD  CONSTRAINT [FK_AspNetDeployException_Exception] FOREIGN KEY([ExceptionId])
REFERENCES [dbo].[Exception] ([Id])
GO
ALTER TABLE [dbo].[AspNetDeployException] CHECK CONSTRAINT [FK_AspNetDeployException_Exception]
GO
ALTER TABLE [dbo].[AspNetDeployException]  WITH CHECK ADD  CONSTRAINT [FK_AspNetDeployException_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AspNetDeployException] CHECK CONSTRAINT [FK_AspNetDeployException_User]
GO
ALTER TABLE [dbo].[BundleVersion]  WITH CHECK ADD  CONSTRAINT [FK_BundleVersion_Bundle] FOREIGN KEY([BundleId])
REFERENCES [dbo].[Bundle] ([Id])
GO
ALTER TABLE [dbo].[BundleVersion] CHECK CONSTRAINT [FK_BundleVersion_Bundle]
GO
ALTER TABLE [dbo].[BundleVersion]  WITH CHECK ADD  CONSTRAINT [FK_BundleVersion_BundleVersion] FOREIGN KEY([ParentBundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[BundleVersion] CHECK CONSTRAINT [FK_BundleVersion_BundleVersion]
GO
ALTER TABLE [dbo].[BundleVersionProperty]  WITH CHECK ADD  CONSTRAINT [FK_BundleVersionProperty_BundleVersion] FOREIGN KEY([BundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[BundleVersionProperty] CHECK CONSTRAINT [FK_BundleVersionProperty_BundleVersion]
GO
ALTER TABLE [dbo].[DataFieldToBundleVersion]  WITH CHECK ADD  CONSTRAINT [FK_DataFieldToBundleVersion_BundleVersion] FOREIGN KEY([BundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[DataFieldToBundleVersion] CHECK CONSTRAINT [FK_DataFieldToBundleVersion_BundleVersion]
GO
ALTER TABLE [dbo].[DataFieldToBundleVersion]  WITH CHECK ADD  CONSTRAINT [FK_DataFieldToBundleVersion_DataField] FOREIGN KEY([DataFieldId])
REFERENCES [dbo].[DataField] ([Id])
GO
ALTER TABLE [dbo].[DataFieldToBundleVersion] CHECK CONSTRAINT [FK_DataFieldToBundleVersion_DataField]
GO
ALTER TABLE [dbo].[DataFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_DataFieldValue_DataField] FOREIGN KEY([DataFieldId])
REFERENCES [dbo].[DataField] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DataFieldValue] CHECK CONSTRAINT [FK_DataFieldValue_DataField]
GO
ALTER TABLE [dbo].[DataFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_DataFieldValue_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[DataFieldValue] CHECK CONSTRAINT [FK_DataFieldValue_Environment]
GO
ALTER TABLE [dbo].[DataFieldValue]  WITH CHECK ADD  CONSTRAINT [FK_DataFieldValue_Machine] FOREIGN KEY([MachineId])
REFERENCES [dbo].[Machine] ([Id])
GO
ALTER TABLE [dbo].[DataFieldValue] CHECK CONSTRAINT [FK_DataFieldValue_Machine]
GO
ALTER TABLE [dbo].[DeploymentStep]  WITH NOCHECK ADD  CONSTRAINT [FK_DeploymentStep_BundleVersion] FOREIGN KEY([BundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[DeploymentStep] CHECK CONSTRAINT [FK_DeploymentStep_BundleVersion]
GO
ALTER TABLE [dbo].[DeploymentStep]  WITH CHECK ADD  CONSTRAINT [FK_DeploymentStep_DeploymentStep] FOREIGN KEY([DeploymentStepId])
REFERENCES [dbo].[DeploymentStep] ([Id])
GO
ALTER TABLE [dbo].[DeploymentStep] CHECK CONSTRAINT [FK_DeploymentStep_DeploymentStep]
GO
ALTER TABLE [dbo].[DeploymentStepProperty]  WITH CHECK ADD  CONSTRAINT [FK_DeploymentStepProperty_DeploymentStep] FOREIGN KEY([DeploymentStepId])
REFERENCES [dbo].[DeploymentStep] ([Id])
GO
ALTER TABLE [dbo].[DeploymentStepProperty] CHECK CONSTRAINT [FK_DeploymentStepProperty_DeploymentStep]
GO
ALTER TABLE [dbo].[DeploymentStepToMachineRole]  WITH CHECK ADD  CONSTRAINT [FK_DeploymentStepToMachineRole_DeploymentStep] FOREIGN KEY([DeploymentStepId])
REFERENCES [dbo].[DeploymentStep] ([Id])
GO
ALTER TABLE [dbo].[DeploymentStepToMachineRole] CHECK CONSTRAINT [FK_DeploymentStepToMachineRole_DeploymentStep]
GO
ALTER TABLE [dbo].[DeploymentStepToMachineRole]  WITH CHECK ADD  CONSTRAINT [FK_DeploymentStepToMachineRole_MachineRole] FOREIGN KEY([MachineRoleId])
REFERENCES [dbo].[MachineRole] ([Id])
GO
ALTER TABLE [dbo].[DeploymentStepToMachineRole] CHECK CONSTRAINT [FK_DeploymentStepToMachineRole_MachineRole]
GO
ALTER TABLE [dbo].[EnvironmentProperty]  WITH CHECK ADD  CONSTRAINT [FK_EnvironmentProperty_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[EnvironmentProperty] CHECK CONSTRAINT [FK_EnvironmentProperty_Environment]
GO
ALTER TABLE [dbo].[Exception]  WITH CHECK ADD  CONSTRAINT [FK_Exception_Exception1] FOREIGN KEY([InnerExceptionId])
REFERENCES [dbo].[Exception] ([Id])
GO
ALTER TABLE [dbo].[Exception] CHECK CONSTRAINT [FK_Exception_Exception1]
GO
ALTER TABLE [dbo].[ExceptionData]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionData_Exception] FOREIGN KEY([ExceptionId])
REFERENCES [dbo].[Exception] ([Id])
GO
ALTER TABLE [dbo].[ExceptionData] CHECK CONSTRAINT [FK_ExceptionData_Exception]
GO
ALTER TABLE [dbo].[MachinePublication]  WITH CHECK ADD  CONSTRAINT [FK_MachinePublication_Machine] FOREIGN KEY([MachineId])
REFERENCES [dbo].[Machine] ([Id])
GO
ALTER TABLE [dbo].[MachinePublication] CHECK CONSTRAINT [FK_MachinePublication_Machine]
GO
ALTER TABLE [dbo].[MachinePublication]  WITH CHECK ADD  CONSTRAINT [FK_MachinePublication_Publication] FOREIGN KEY([PublicationId])
REFERENCES [dbo].[Publication] ([Id])
GO
ALTER TABLE [dbo].[MachinePublication] CHECK CONSTRAINT [FK_MachinePublication_Publication]
GO
ALTER TABLE [dbo].[MachinePublicationLog]  WITH CHECK ADD  CONSTRAINT [FK_MachinePublicationLog_Exception] FOREIGN KEY([ExceptionId])
REFERENCES [dbo].[Exception] ([Id])
GO
ALTER TABLE [dbo].[MachinePublicationLog] CHECK CONSTRAINT [FK_MachinePublicationLog_Exception]
GO
ALTER TABLE [dbo].[MachinePublicationLog]  WITH CHECK ADD  CONSTRAINT [FK_MachinePublicationLog_MachinePublication] FOREIGN KEY([MachinePublicationId])
REFERENCES [dbo].[MachinePublication] ([Id])
GO
ALTER TABLE [dbo].[MachinePublicationLog] CHECK CONSTRAINT [FK_MachinePublicationLog_MachinePublication]
GO
ALTER TABLE [dbo].[MachinePublicationLogProperty]  WITH CHECK ADD  CONSTRAINT [FK_MachinePublicationLogProperty_MachinePublicationLog] FOREIGN KEY([MachinePublicationLogId])
REFERENCES [dbo].[MachinePublicationLog] ([Id])
GO
ALTER TABLE [dbo].[MachinePublicationLogProperty] CHECK CONSTRAINT [FK_MachinePublicationLogProperty_MachinePublicationLog]
GO
ALTER TABLE [dbo].[MachinePublicationProperty]  WITH CHECK ADD  CONSTRAINT [FK_MachinePublicationProperty_MachinePublication] FOREIGN KEY([MachinePublicationId])
REFERENCES [dbo].[MachinePublication] ([Id])
GO
ALTER TABLE [dbo].[MachinePublicationProperty] CHECK CONSTRAINT [FK_MachinePublicationProperty_MachinePublication]
GO
ALTER TABLE [dbo].[MachineToEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_MachineToEnvironment_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[MachineToEnvironment] CHECK CONSTRAINT [FK_MachineToEnvironment_Environment]
GO
ALTER TABLE [dbo].[MachineToEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_MachineToEnvironment_Machine] FOREIGN KEY([MachineId])
REFERENCES [dbo].[Machine] ([Id])
GO
ALTER TABLE [dbo].[MachineToEnvironment] CHECK CONSTRAINT [FK_MachineToEnvironment_Machine]
GO
ALTER TABLE [dbo].[MachineToMachineRole]  WITH CHECK ADD  CONSTRAINT [FK_MachineToMachineRole_Machine] FOREIGN KEY([MachineId])
REFERENCES [dbo].[Machine] ([Id])
GO
ALTER TABLE [dbo].[MachineToMachineRole] CHECK CONSTRAINT [FK_MachineToMachineRole_Machine]
GO
ALTER TABLE [dbo].[MachineToMachineRole]  WITH CHECK ADD  CONSTRAINT [FK_MachineToMachineRole_MachineRole] FOREIGN KEY([MachineRoleId])
REFERENCES [dbo].[MachineRole] ([Id])
GO
ALTER TABLE [dbo].[MachineToMachineRole] CHECK CONSTRAINT [FK_MachineToMachineRole_MachineRole]
GO
ALTER TABLE [dbo].[NextEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_NextEnvironment_Environment_NextEnvironment] FOREIGN KEY([NextEnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[NextEnvironment] CHECK CONSTRAINT [FK_NextEnvironment_Environment_NextEnvironment]
GO
ALTER TABLE [dbo].[NextEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_NextEnvironment_Environment_SourceEnvironment] FOREIGN KEY([SourceEnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[NextEnvironment] CHECK CONSTRAINT [FK_NextEnvironment_Environment_SourceEnvironment]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_BundleVersion] FOREIGN KEY([BundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_BundleVersion]
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_PackageApprovedOnEnvironment_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment] CHECK CONSTRAINT [FK_PackageApprovedOnEnvironment_Environment]
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_PackageApprovedOnEnvironment_Package] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Package] ([Id])
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment] CHECK CONSTRAINT [FK_PackageApprovedOnEnvironment_Package]
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment]  WITH CHECK ADD  CONSTRAINT [FK_PackageApprovedOnEnvironment_User] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[PackageApprovedOnEnvironment] CHECK CONSTRAINT [FK_PackageApprovedOnEnvironment_User]
GO
ALTER TABLE [dbo].[PackageEntry]  WITH CHECK ADD  CONSTRAINT [FK_PackageEntry_Package] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Package] ([Id])
GO
ALTER TABLE [dbo].[PackageEntry] CHECK CONSTRAINT [FK_PackageEntry_Package]
GO
ALTER TABLE [dbo].[PackageEntry]  WITH CHECK ADD  CONSTRAINT [FK_PackageEntry_ProjectVersion] FOREIGN KEY([ProjectVersionId])
REFERENCES [dbo].[ProjectVersion] ([Id])
GO
ALTER TABLE [dbo].[PackageEntry] CHECK CONSTRAINT [FK_PackageEntry_ProjectVersion]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_SourceControl] FOREIGN KEY([SourceControlId])
REFERENCES [dbo].[SourceControl] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_SourceControl]
GO
ALTER TABLE [dbo].[ProjectProperty]  WITH CHECK ADD  CONSTRAINT [FK_ProjectProperty_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[ProjectProperty] CHECK CONSTRAINT [FK_ProjectProperty_Project]
GO
ALTER TABLE [dbo].[ProjectToBundle]  WITH CHECK ADD  CONSTRAINT [FK_ProjectToBundle_Bundle1] FOREIGN KEY([BundleId])
REFERENCES [dbo].[Bundle] ([Id])
GO
ALTER TABLE [dbo].[ProjectToBundle] CHECK CONSTRAINT [FK_ProjectToBundle_Bundle1]
GO
ALTER TABLE [dbo].[ProjectToBundle]  WITH CHECK ADD  CONSTRAINT [FK_ProjectToBundle_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[ProjectToBundle] CHECK CONSTRAINT [FK_ProjectToBundle_Project]
GO
ALTER TABLE [dbo].[ProjectVersion]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersion_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
GO
ALTER TABLE [dbo].[ProjectVersion] CHECK CONSTRAINT [FK_ProjectVersion_Project]
GO
ALTER TABLE [dbo].[ProjectVersion]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersion_SourceControlVersion] FOREIGN KEY([SourceControlVersionId])
REFERENCES [dbo].[SourceControlVersion] ([Id])
GO
ALTER TABLE [dbo].[ProjectVersion] CHECK CONSTRAINT [FK_ProjectVersion_SourceControlVersion]
GO
ALTER TABLE [dbo].[ProjectVersionProperty]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersionProperty_ProjectVersion] FOREIGN KEY([ProjectVersionId])
REFERENCES [dbo].[ProjectVersion] ([Id])
GO
ALTER TABLE [dbo].[ProjectVersionProperty] CHECK CONSTRAINT [FK_ProjectVersionProperty_ProjectVersion]
GO
ALTER TABLE [dbo].[ProjectVersionToBundleVersion]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersionToBundleVersion_BundleVersion] FOREIGN KEY([BundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[ProjectVersionToBundleVersion] CHECK CONSTRAINT [FK_ProjectVersionToBundleVersion_BundleVersion]
GO
ALTER TABLE [dbo].[ProjectVersionToBundleVersion]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersionToBundleVersion_ProjectVersion] FOREIGN KEY([ProjectVersionId])
REFERENCES [dbo].[ProjectVersion] ([Id])
GO
ALTER TABLE [dbo].[ProjectVersionToBundleVersion] CHECK CONSTRAINT [FK_ProjectVersionToBundleVersion_ProjectVersion]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([Id])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_Environment]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_Package] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Package] ([Id])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_Package]
GO
ALTER TABLE [dbo].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_User] FOREIGN KEY([QueuedByUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Publication] CHECK CONSTRAINT [FK_Publication_User]
GO
ALTER TABLE [dbo].[Revision]  WITH CHECK ADD  CONSTRAINT [FK_Revision_SourceControl] FOREIGN KEY([SourceControlId])
REFERENCES [dbo].[SourceControl] ([Id])
GO
ALTER TABLE [dbo].[Revision] CHECK CONSTRAINT [FK_Revision_SourceControl]
GO
ALTER TABLE [dbo].[RevisionEntry]  WITH CHECK ADD  CONSTRAINT [FK_RevisionEntry_Revision] FOREIGN KEY([RevisionId])
REFERENCES [dbo].[Revision] ([Id])
GO
ALTER TABLE [dbo].[RevisionEntry] CHECK CONSTRAINT [FK_RevisionEntry_Revision]
GO
ALTER TABLE [dbo].[SourceControlProperty]  WITH CHECK ADD  CONSTRAINT [FK_SourceControlProperty_SourceControl] FOREIGN KEY([SourceControlId])
REFERENCES [dbo].[SourceControl] ([Id])
GO
ALTER TABLE [dbo].[SourceControlProperty] CHECK CONSTRAINT [FK_SourceControlProperty_SourceControl]
GO
ALTER TABLE [dbo].[SourceControlToGroup]  WITH CHECK ADD  CONSTRAINT [FK_SourceControlToGroup_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
GO
ALTER TABLE [dbo].[SourceControlToGroup] CHECK CONSTRAINT [FK_SourceControlToGroup_Group]
GO
ALTER TABLE [dbo].[SourceControlToGroup]  WITH CHECK ADD  CONSTRAINT [FK_SourceControlToGroup_SourceControl] FOREIGN KEY([SourceControlId])
REFERENCES [dbo].[SourceControl] ([Id])
GO
ALTER TABLE [dbo].[SourceControlToGroup] CHECK CONSTRAINT [FK_SourceControlToGroup_SourceControl]
GO
ALTER TABLE [dbo].[SourceControlVersion]  WITH CHECK ADD  CONSTRAINT [FK_SourceControlVersion_SourceControl] FOREIGN KEY([SourceControlId])
REFERENCES [dbo].[SourceControl] ([Id])
GO
ALTER TABLE [dbo].[SourceControlVersion] CHECK CONSTRAINT [FK_SourceControlVersion_SourceControl]
GO
ALTER TABLE [dbo].[SourceControlVersion]  WITH CHECK ADD  CONSTRAINT [FK_SourceControlVersion_SourceControlVersion] FOREIGN KEY([ParentVersionId])
REFERENCES [dbo].[SourceControlVersion] ([Id])
GO
ALTER TABLE [dbo].[SourceControlVersion] CHECK CONSTRAINT [FK_SourceControlVersion_SourceControlVersion]
GO
ALTER TABLE [dbo].[SourceControlVersionProperty]  WITH CHECK ADD  CONSTRAINT [FK_SourceControlVersionProperty_SourceControlVersion] FOREIGN KEY([SourceControlVersionId])
REFERENCES [dbo].[SourceControlVersion] ([Id])
GO
ALTER TABLE [dbo].[SourceControlVersionProperty] CHECK CONSTRAINT [FK_SourceControlVersionProperty_SourceControlVersion]
GO
ALTER TABLE [dbo].[TestStep]  WITH CHECK ADD  CONSTRAINT [FK_TestStep_BundleVersion] FOREIGN KEY([BundleVersionId])
REFERENCES [dbo].[BundleVersion] ([Id])
GO
ALTER TABLE [dbo].[TestStep] CHECK CONSTRAINT [FK_TestStep_BundleVersion]
GO
ALTER TABLE [dbo].[VersionToVersion]  WITH CHECK ADD  CONSTRAINT [FK_VersionToVersion_Version] FOREIGN KEY([PreviousVersionId])
REFERENCES [dbo].[Version] ([Id])
GO
ALTER TABLE [dbo].[VersionToVersion] CHECK CONSTRAINT [FK_VersionToVersion_Version]
GO
ALTER TABLE [dbo].[VersionToVersion]  WITH CHECK ADD  CONSTRAINT [FK_VersionToVersion_Version1] FOREIGN KEY([NextVersionId])
REFERENCES [dbo].[Version] ([Id])
GO
ALTER TABLE [dbo].[VersionToVersion] CHECK CONSTRAINT [FK_VersionToVersion_Version1]
GO
