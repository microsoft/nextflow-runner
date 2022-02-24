using Microsoft.EntityFrameworkCore;

namespace NextflowRunner.Models;

public class NextflowRunnerContext : DbContext
{
    public NextflowRunnerContext(DbContextOptions options) : base(options) { }

    public DbSet<Pipeline> Pipelines { get; set; }
    public DbSet<PipelineParam> PipelineParams { get; set; }
    public DbSet<PipelineRun> PipelineRuns { get; set; }
}