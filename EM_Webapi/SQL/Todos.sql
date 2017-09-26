CREATE TABLE [dbo].[Todos] (
    [id]       INT        IDENTITY (1, 1) NOT NULL,
    [modified] DATETIME   DEFAULT (getdate()) NOT NULL,
	[userid]  VARCHAR (50)  NULL,
	[title] VARCHAR (100)  NULL,
	[tags]  VARCHAR (100)  NULL,
    [content]  VARCHAR (2000),
	[urgent]  bit DEFAULT 0 NOT NULL ,
	[completed]  bit DEFAULT 0 NOT NULL ,
    PRIMARY KEY CLUSTERED ([id] ASC)
);
go
