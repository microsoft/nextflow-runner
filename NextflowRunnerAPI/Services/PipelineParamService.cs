using Microsoft.EntityFrameworkCore;
using NextflowRunnerAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextflowRunnerAPI.Services
{
    public class PipelineParamService : IPipelineParamService
    {
        private readonly NextflowRunnerContext _db;
        public PipelineParamService(NextflowRunnerContext db)
        {
            _db = db;
        }

        public async Task<List<PipelineParam>> GetPipelineParamsAsync(int pipelineId)
        {
            return await _db.PipelineParams.Where(p => p.PipelineId == pipelineId).ToListAsync();
        }

        public async Task<PipelineParam> GetPipelineParamAsync(int pipelineParamId)
        {
            return await _db.PipelineParams.FindAsync(pipelineParamId);
        }

        public async Task<PipelineParam?> CreatePipelineParamAsync(int pipelineId, PipelineParam pipelineParam)
        {
            var pipeline = await _db.Pipelines.FindAsync(pipelineId);

            if (pipeline == null) return null;

            pipeline.PipelineParams ??= new List<PipelineParam>();

            pipeline.PipelineParams.Add(pipelineParam);

            await _db.SaveChangesAsync();

            return pipelineParam;
        }

        public async Task<PipelineParam?> UpdatePipelineParamAsync(PipelineParam pipelineParam)
        {
            var dbPipelineParam = await _db.PipelineParams.FindAsync(pipelineParam.PipelineParamId);

            if(dbPipelineParam == null) return null;  

            dbPipelineParam = pipelineParam;

            await _db.SaveChangesAsync();

            return pipelineParam;
        }

        public async Task DeletePipelineParamAsync(int pipelineParamId)
        {
            var dbPipelineParam = await _db.PipelineParams.FindAsync(pipelineParamId);

            if (dbPipelineParam == null) return;

            _db.PipelineParams.Remove(dbPipelineParam);
        }
    }
}
