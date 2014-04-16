USE [UmbracoUserAdminTest]
GO

/****** Object:  StoredProcedure [dbo].[DeleteResetDetails]    Script Date: 16/04/2014 14:22:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






-- =============================================
-- Author:		Daniel Mothersole
-- Create date: 22/07/2013
-- Description:	Delete password reset details
-- =============================================
CREATE PROCEDURE [dbo].[DeleteResetDetails]
	-- Add the parameters for the stored procedure here
	@UniqueResetId nvarchar(50),
	@userId Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [passwordReset] WHERE [ResetId] = @UniqueResetId and [UserId] = @userId
END







GO

