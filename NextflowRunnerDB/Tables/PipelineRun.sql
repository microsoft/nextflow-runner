CREATE TABLE [dbo].[PipelineRun]
(
	[PipelineRunId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineId] INT NOT NULL, 
    [PipelineRunName] NVARCHAR(50) NULL,
    [NextflowRunCommand] NVARCHAR(MAX) NOT NULL, 
    [RunDateTime] DATETIME NOT NULL, 
    [Status] NVARCHAR(50) NULL, 
    CONSTRAINT [FK_PipelineRun_Pipeline] FOREIGN KEY ([PipelineId]) REFERENCES [Pipeline]([PipelineId])
)
