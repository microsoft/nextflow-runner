using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextflowRunner.Models;
using System;

[assembly: FunctionsStartup(typeof(NextflowRunner.WeblogTracer.Startup))]
namespace NextflowRunner.WeblogTracer;

class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var sqlConnection = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");

        builder.Services.AddDbContext<NextflowRunnerContext>(
            options => options.UseSqlServer(sqlConnection));
    }
}
