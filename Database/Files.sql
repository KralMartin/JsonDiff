CREATE TABLE [dbo].[Files]
(
	[Id] CHAR(128) NOT NULL , 
    [Side] BIT NOT NULL, 
    [Content] VARBINARY(MAX) NULL, 
    PRIMARY KEY ([Id], [Side])
)
