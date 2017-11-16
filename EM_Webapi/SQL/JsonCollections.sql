USE [EM_Webapi]
GO

/****** Object: Table [dbo].[JsonCollections] Script Date: 16/11/2017 11:52:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JsonCollections] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [Value]     NTEXT          NULL,
    [Modified]  DATETIME       NULL,
    [GroupName] NVARCHAR (100) NULL
);


