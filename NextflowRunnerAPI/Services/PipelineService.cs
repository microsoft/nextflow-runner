using Microsoft.EntityFrameworkCore;
using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public class PipelineService : IPipelineService
    {
        private readonly NextflowRunnerContext _db;
        public PipelineService(NextflowRunnerContext db)
        {
            _db = db;
        }

        public async Task<List<Pipeline>> GetPipelinesAsync()
        {
            return await _db.Pipelines.ToListAsync();
        }

        public async Task<Pipeline?> GetPipelineAsync(int pipelineId)
        {
            return await _db.Pipelines.FindAsync(pipelineId);
        }

        public async Task<Pipeline> CreatePipelineAsync(Pipeline pipeline)
        {
            _db.Pipelines.Add(pipeline);
            await _db.SaveChangesAsync();
            return pipeline;
        }

        public async Task<Pipeline?> UpdatePipelineAsync(Pipeline pipeline)
        {
            var dbPipeline = await _db.Pipelines.FindAsync(pipeline.PipelineId);

            if (dbPipeline == null) return null;

            dbPipeline = pipeline;

            await _db.SaveChangesAsync();

            return pipeline;
        }

        public async Task DeletePipelineAsync(int pipelineId)
        {
            var dbPipeline = await _db.Pipelines.FindAsync(pipelineId);

            if (dbPipeline == null) return;

            _db.Pipelines.Remove(dbPipeline);
        }
    }
}
