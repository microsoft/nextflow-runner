using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public interface IPipelineService
    {
        Task<List<Pipeline>> GetPipelinesAsync();
        Task<Pipeline?> GetPipelineAsync(int pipelineId);
        Task<Pipeline> CreatePipelineAsync(Pipeline pipeline);
        Task<Pipeline?> UpdatePipelineAsync(Pipeline pipeline);
        Task DeletePipelineAsync(int pipelineId);
    }
}
