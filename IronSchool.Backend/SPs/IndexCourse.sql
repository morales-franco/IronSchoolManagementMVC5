IF EXISTS(SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[IndexCourse]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT', N'P' ))
BEGIN
    DROP PROCEDURE [dbo].[IndexCourse];
END 

GO

CREATE PROCEDURE [dbo].[IndexCourse]	
AS

	SELECT [CourseId], [Description], I.[InstructorId], U.LastName + ' ' + U.FirstName AS Instructor,  [StartDate], [FinishDate], [StudentCountMax],
		   (SELECT COUNT(S.StudentId) FROM StudentsInCourse S WHERE S.CourseId = C.CourseId) AS StudentCount
	FROM [Course] C INNER JOIN Instructor I ON (C.InstructorId = I.InstructorId)
					INNER JOIN [User] U ON (U.UserId = I.UserId)

	ORDER BY [StartDate]

GO 