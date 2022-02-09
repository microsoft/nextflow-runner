CREATE TABLE [dbo].[Pipeline]
(
	[PipelineId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineName] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [GitHubURL] NVARCHAR(250) NULL
)
