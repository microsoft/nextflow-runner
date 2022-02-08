using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public interface ISshClient
    {
        Task ExecutePipelineAsync(Pipeline pipeline);
    }
}
