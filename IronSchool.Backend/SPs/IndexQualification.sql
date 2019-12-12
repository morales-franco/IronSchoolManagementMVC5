IF EXISTS(SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[IndexStudentsInCourse]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT', N'P' ))
BEGIN
    DROP PROCEDURE [dbo].[IndexStudentsInCourse];
END 

GO

CREATE PROCEDURE [dbo].[IndexStudentsInCourse](@InstructorUserId BIGINT)
AS

	SELECT CONVERT(VARCHAR, SC.CourseId) + '-' + CONVERT(VARCHAR, SC.StudentId) AS QualificationId, S.LastName + ' ' + S.FirstName AS Student, C.Description AS Course, ISNULL(CONVERT(VARCHAR,SC.ExamQualification), 'No Informado') AS Qualification
	FROM StudentsInCourse SC INNER JOIN Student S ON ( SC.StudentId = S.StudentId )
							 INNER JOIN Course C ON ( SC.CourseId = C.CourseId )
							 INNER JOIN Instructor I ON ( C.InstructorId = I.InstructorId )
	WHERE @InstructorUserId IS NULL OR I.UserId = @InstructorUserId
	ORDER BY SC.CourseId, S.LastName


GO 

