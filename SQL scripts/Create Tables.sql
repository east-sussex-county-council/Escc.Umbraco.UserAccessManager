

CREATE TABLE [dbo].[editors] (
    [UserId]   INT           NOT NULL,
    [FullName] NVARCHAR (50) NOT NULL
);

CREATE TABLE [dbo].[passwordReset] (
    [RestNo]       INT          IDENTITY (1, 1) NOT NULL,
    [ResetId]      VARCHAR (50) NULL,
    [TimeLimit]    DATETIME     NULL,
    [UserId]       INT          NULL,
    [EmailAddress] VARCHAR (50) NULL,
    CONSTRAINT [PK_passwordReset] PRIMARY KEY CLUSTERED ([RestNo] ASC)
);

CREATE TABLE [dbo].[permissions] (
    [PermissionId] INT           IDENTITY (1, 1) NOT NULL,
    [PageId]       INT           NOT NULL,
    [PageName]     NVARCHAR (50) NOT NULL,
    [UserId]       INT           NOT NULL,
    [FullName]     NVARCHAR (50) NOT NULL,
    [EmailAddress] NVARCHAR (50) NOT NULL,
    [Created]      DATETIME      NULL
);

