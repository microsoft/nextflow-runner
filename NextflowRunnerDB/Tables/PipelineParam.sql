CREATE TABLE [dbo].[PipelineParam]
(
	[PipelineParamId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineId] INT NOT NULL,
    [ParamName] NVARCHAR(50) NOT NULL, 
    [ParamType] NVARCHAR(10) NOT NULL,
    [ParamExample] NVARCHAR(50) NULL, 
    [DefaultValue] NVARCHAR(500) NULL, 
    [ParamIsFile] BIT NULL DEFAULT 1, 
    CONSTRAINT [FK_PipelineParam_Pipeline] FOREIGN KEY ([PipelineId]) REFERENCES [Pipeline]([PipelineId])
)

GO
