using Microsoft.EntityFrameworkCore;
using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public class PipelineRunService : IPipelineRunService
    {
        private readonly NextflowRunnerContext _db;
        public PipelineRunService(NextflowRunnerContext db)
        {
            _db = db;
        }

        public async Task<List<PipelineRun>> GetPipelineRunsAsync(int pipelineId)
        {
            return await _db.PipelineRuns.Where(p => p.PipelineId == pipelineId).ToListAsync();
        }

        public async Task<PipelineRun?> GetPipelineRunAsync(int PipelineRunId)
        {
            return await _db.PipelineRuns.FindAsync(PipelineRunId);
        }

        public async Task<PipelineRun?> CreatePipelineRunAsync(int pipelineId, PipelineRun PipelineRun)
        {
            var pipeline = await _db.Pipelines.FindAsync(pipelineId);
           
            if (pipeline == null) return null;

            pipeline.PipelineRuns ??= new List<PipelineRun>();

            pipeline.PipelineRuns.Add(PipelineRun);

            await _db.SaveChangesAsync();

            return PipelineRun;
        }

        public async Task<PipelineRun?> UpdatePipelineRunAsync(PipelineRun PipelineRun)
        {
            var dbPipelineRun = await _db.PipelineRuns.FindAsync(PipelineRun.PipelineRunId);

            if (dbPipelineRun == null) return null;

            dbPipelineRun = PipelineRun;

            await _db.SaveChangesAsync();

            return PipelineRun;
        }

        public async Task DeletePipelineRunAsync(int PipelineRunId)
        {
            var dbPipelineRun = await _db.PipelineRuns.FindAsync(PipelineRunId);

            if (dbPipelineRun == null) return;

            _db.PipelineRuns.Remove(dbPipelineRun);
        }
    }
}
