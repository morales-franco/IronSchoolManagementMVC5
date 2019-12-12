IF EXISTS(SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[IndexRole]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT', N'P' ))
BEGIN
    DROP PROCEDURE [dbo].[IndexRole];
END 

GO

CREATE PROCEDURE [dbo].[IndexRole]	
	@Name varchar(255) = null,
	@Active bit = null
AS

	SELECT RoleId, Name, CASE Active WHEN  1 THEN 'Si' ELSE 'No' END AS Active
	FROM [Role]
	WHERE (@Name IS NULL OR Name LIKE '%'+@Name+'%')		
		AND (@Active IS NULL OR Active = @Active)  
	order by Name

GO