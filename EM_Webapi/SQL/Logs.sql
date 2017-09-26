CREATE TABLE [dbo].[Logs] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Modified] DATE       DEFAULT (getdate()) NOT NULL,
	[Category] VARCHAR (50)  NULL,
    [Content]  VARCHAR (1000),
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
go

CREATE PROCEDURE dbo.Logs_Insert
	@Category VARCHAR(50),
	@Content VARCHAR(1000)
AS
BEGIN 
	SET NOCOUNT ON 
	Insert into Logs(Category,Content,Modified)
	Values(@Category, @Content, GetDate())

	Declare @new_identity int;
	SELECT @new_identity = SCOPE_IDENTITY()
	return @new_identity;
END
GO 

CREATE PROCEDURE dbo.Logs_GetTopX
	@Category VARCHAR(50),
	@x int,
	@skip int
AS
BEGIN 
	Select * from Logs
	where category = @Category
	order by id desc
	offset @skip rows
	fetch next @x rows only
END
GO

CREATE PROCEDURE dbo.Logs_GetAll
	@Category VARCHAR(50)
AS
BEGIN 
	Select * from Logs
	where category = @Category
	Order by id desc
END
GO

CREATE PROCEDURE dbo.Logs_Update
	@ID int,
	@Category VARCHAR(50),
	@content VARCHAR(1000) 
AS
BEGIN 
	Update Logs 
	SET content = @content, category= @Category, modified = GetDate()
	where Id = @ID
END
GO

CREATE PROCEDURE dbo.Logs_Delete
	@ID int
AS
BEGIN 
	Delete Logs
	where Id = @ID
END
GO

CREATE PROCEDURE dbo.Logs_GetById
	@ID int
AS
BEGIN 
	Select * from Logs
	where Id = @ID
END
GO
