USE [UmbracoUserAdminTest]
GO

/****** Object:  StoredProcedure [dbo].[SetResetDetails]    Script Date: 16/04/2014 14:22:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO











-- =============================================
-- Author:		Daniel Mothersole
-- Create date: 22/07/2013
-- Description:	Set password reset details
-- =============================================
CREATE PROCEDURE [dbo].[SetResetDetails]
	-- Add the parameters for the stored procedure here
	@UniqueResetId nvarchar(50),
	@TimeStamp datetime,
	@UserId Int,
	@EmailAddress varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [passwordReset] ([ResetId],[TimeLimit], [UserId], [EmailAddress]) VALUES (@UniqueResetId, @TimeStamp, @UserId, @EmailAddress)
END












GO

