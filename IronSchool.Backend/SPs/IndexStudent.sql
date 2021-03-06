IF EXISTS(SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[IndexStudent]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT', N'P' ))
BEGIN
    DROP PROCEDURE [dbo].[IndexStudent];
END 

GO

CREATE PROCEDURE [dbo].[IndexStudent]	
AS

	SELECT [StudentId], [FirstName], [LastName], [BirthDate], [Address], [ContactMail], [ContactPhone], 
	CASE [Sex] WHEN  'M' THEN 'Masculino' ELSE 'Femenino' END AS [Sex] 
	FROM [Student]
	ORDER BY LastName

GO 