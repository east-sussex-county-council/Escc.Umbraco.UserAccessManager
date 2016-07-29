CREATE TABLE [dbo].[passwordReset] (
    [RestNo]       INT          IDENTITY (1, 1) NOT NULL,
    [ResetId]      VARCHAR (50) NULL,
    [TimeLimit]    DATETIME     NULL,
    [UserId]       INT          NULL,
    [EmailAddress] VARCHAR (50) NULL,
    CONSTRAINT [PK_passwordReset] PRIMARY KEY CLUSTERED ([RestNo] ASC)
);


GO
GRANT SELECT
    ON OBJECT::[dbo].[passwordReset] TO [UserAccessManagerRole]
    AS [dbo];

