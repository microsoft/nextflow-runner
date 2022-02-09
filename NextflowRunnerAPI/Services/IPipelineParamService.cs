using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public interface IPipelineParamService
    {
        Task<List<PipelineParam>> GetPipelineParamsAsync(int pipelineId);
        Task<PipelineParam?> GetPipelineParamAsync(int pipelineParamId);
        Task<PipelineParam?> CreatePipelineParamAsync(int pipelineId, PipelineParam pipelineParam);
        Task<PipelineParam?> UpdatePipelineParamAsync(PipelineParam pipelineParam);
        Task DeletePipelineParamAsync(int pipelineParamId);
    }
}
