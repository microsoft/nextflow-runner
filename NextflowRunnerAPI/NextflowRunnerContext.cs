using Microsoft.EntityFrameworkCore;
using NextflowRunner.API.Models;

namespace NextflowRunner.API
{
    public class NextflowRunnerContext : DbContext
    {
        public NextflowRunnerContext(DbContextOptions options) : base(options) { }

        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<PipelineParam> PipelineParams { get; set; }
        public DbSet<PipelineRun> PipelineRuns { get; set; }
    }
}
