using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NextflowRunnerClient.Services;

namespace NextflowRunnerClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddScoped(sp => new NextflowAPI(
            builder.Configuration["NextflowRunnerAPI"],
            new HttpClient()
            ));


        await builder.Build().RunAsync();
    }
}