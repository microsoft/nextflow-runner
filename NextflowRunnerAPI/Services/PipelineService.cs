using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public class PipelineService : IPipelineService
    {
        public PipelineService() { }

        public async Task<List<Pipeline>> GetPipelinesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Pipeline> GetPipelineAsync(int pipelineId)
        {
            throw new NotImplementedException();
        }

        public async Task<Pipeline> CreatePipelineAsync(Pipeline pipeline)
        {
            throw new NotImplementedException();
        }

        public async Task<Pipeline> UpdatePipelineAsync(int pipelineId)
        {
            throw new NotImplementedException();
        }

        public async Task DeletePipelineAsync(int pipelineId)
        {
            throw new NotImplementedException();
        }
    }
}
