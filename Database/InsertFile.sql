CREATE PROCEDURE [dbo].[InsertFile]
	@id char(128),
	@side bit,
	@content varbinary(MAX)
AS
	INSERT INTO Files
	VALUES (@id, @side, @content)
RETURN 1
