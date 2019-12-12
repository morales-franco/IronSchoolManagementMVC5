IF EXISTS(SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[IndexUser]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT', N'P' ))
BEGIN
    DROP PROCEDURE [dbo].[IndexUser];
END 

GO

CREATE PROCEDURE [dbo].[IndexUser]	
	@UserName varchar(255) = null,
	@FirstName varchar(255) = null,
	@LastName varchar(255) = null,
	@Active bit = null, 
	@Email varchar(255) = null
AS

	SELECT UserId, UserName, CASE Active WHEN  1 THEN 'Si' ELSE 'No' END AS Active , FirstName, LastName, Email
	FROM [User]
	WHERE (@FirstName IS NULL OR FirstName LIKE '%'+@FirstName+'%')
		AND (@LastName IS NULL OR LastName LIKE '%'+ @LastName+'%')
		AND (@UserName IS NULL OR UserName LIKE '%'+ @UserName+'%')
		AND (@Email IS NULL OR Email LIKE '%'+ @Email+'%')
		AND (@Active IS NULL OR Active = @Active)  
	order by UserName

GO