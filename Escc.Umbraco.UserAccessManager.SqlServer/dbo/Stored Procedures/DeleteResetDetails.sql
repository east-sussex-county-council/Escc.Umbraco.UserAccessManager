





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
GRANT EXECUTE
    ON OBJECT::[dbo].[DeleteResetDetails] TO [UserAccessManagerRole]
    AS [dbo];

