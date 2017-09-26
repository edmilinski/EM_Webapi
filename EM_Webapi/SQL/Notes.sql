CREATE TABLE [dbo].[Notes] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Modified] DATETIME       DEFAULT (getdate()) NOT NULL,
    [Title]    VARCHAR (100),
    [Content]  VARCHAR (3000) NULL,
    [Tags] VARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
go

CREATE PROCEDURE dbo.Notes_Insert
	@Title VARCHAR(100),      
	@Content VARCHAR(3000)      = NULL  , 
	@Tags VARCHAR(100)      = NULL  
AS
BEGIN 
	SET NOCOUNT ON 
	Insert into Notes(Title, Content,Tags, Modified)
	Values(@Title, @Content, @Tags, GetDate())
END
GO 

CREATE PROCEDURE dbo.Notes_FilterAny
	@filter VARCHAR(100)
AS
BEGIN 
	Select * from Notes
	where title like @filter or content like @filter or tags like @filter
END
GO

CREATE PROCEDURE dbo.Notes_FilterTitle
	@titleFilter VARCHAR(100)
AS
BEGIN 
	Select * from Notes
	where title like @titleFilter
END
GO

CREATE PROCEDURE dbo.Notes_FilterContent
	@contentFilter VARCHAR(100)
AS
BEGIN 
	Select * from Notes
	where content like @contentFilter
END
GO

CREATE PROCEDURE dbo.Notes_FilterTags
	@TagsFilter VARCHAR(100)
AS
BEGIN 
	Select * from Notes
	where Tags like @TagsFilter
END
GO

CREATE PROCEDURE dbo.Notes_GetById
	@ID int
AS
BEGIN 
	Select * from Notes
	where Id = @ID
END
GO

CREATE PROCEDURE dbo.Notes_GetTopX
	@x int
AS
BEGIN 
	Select top(@x) * from Notes
	Order by Modified desc
END
GO

CREATE PROCEDURE dbo.Notes_GetAll
AS
BEGIN 
	Select * from Notes
	Order by Modified desc
END
GO

CREATE PROCEDURE dbo.Notes_Update
	@ID int,
	@Title VARCHAR(100),      
	@content VARCHAR(3000)      = NULL  , 
	@Tags VARCHAR(100)      = NULL  
AS
BEGIN 
	Update Notes 
	SET content = @content, title = @Title, Tags = @Tags, modified = GetDate()
	where Id = @ID
END
GO

CREATE PROCEDURE dbo.Notes_Delete
	@ID int
AS
BEGIN 
	Delete Notes
	where Id = @ID
END
GO

CREATE PROCEDURE dbo.Notes_GetNewer
	@Modified datetime
AS
BEGIN 
	Select * from Notes
	where Modified > @Modified
	Order by Modified desc
END
GO
