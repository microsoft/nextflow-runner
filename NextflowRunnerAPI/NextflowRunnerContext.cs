using Microsoft.EntityFrameworkCore;
using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI
{
    public class NextflowRunnerContext : DbContext
    {
        public NextflowRunnerContext(DbContextOptions options) : base(options) { }

        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<PipelineParam> PipelineParams { get; set; }
        public DbSet<PipelineRun> PipelineRuns { get; set; }
    }
}
