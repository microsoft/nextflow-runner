using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextflowRunner.Models;
using System;

[assembly: FunctionsStartup(typeof(NextflowRunner.Serverless.Startup))]
namespace NextflowRunner.Serverless;

class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var sqlConnection = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");

        builder.Services.AddDbContext<NextflowRunnerContext>(
            options => options.UseSqlServer(sqlConnection));
    }
}
