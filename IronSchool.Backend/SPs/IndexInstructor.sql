IF EXISTS(SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[IndexInstructor]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT', N'P' ))
BEGIN
    DROP PROCEDURE [dbo].[IndexInstructor];
END 

GO

CREATE PROCEDURE [dbo].[IndexInstructor]	
AS

	SELECT [InstructorId], U.[FirstName], U.[LastName], [BirthDate], [Address], U.Email AS [ContactMail], [ContactPhone],  [Salary],
	CASE [Sex] WHEN  'M' THEN 'Masculino' ELSE 'Femenino' END AS [Sex] 
	FROM [Instructor] I INNER JOIN [User] U ON ( I.UserId = U.UserId )
	ORDER BY LastName

GO 