using System.ComponentModel.DataAnnotations.Schema;

namespace NextflowRunner.Models;

[Table("PipelineParam")]
public record PipelineParam
{
    public int PipelineParamId { get; set; }
    public int PipelineId { get; set; }
    public string ParamName { get; set; }
    public string ParamType { get; set; }
    public string ParamExample { get; set; }
    public string DefaultValue { get; set; }
}