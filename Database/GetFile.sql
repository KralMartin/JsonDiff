CREATE PROCEDURE [dbo].[GetFile]
	@id char(128),
	@side bit
AS
	SELECT Content 
	FROM Files
	WHERE Id = @id and Side = @side

