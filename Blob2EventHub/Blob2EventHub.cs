using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Blob2EventHub.Blob2EventHub))]
namespace Blob2EventHub
{
    public class Blob2EventHub : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        [FunctionName("Blob2EventHub")]
        public static void Run([BlobTrigger("import-app-insights/{name}", Connection = "StorageConnection")]Stream blob, string name, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");
            }
            catch (Exception ex)
            {
                log.LogWarning($"{ex.Message}");
            }            
        }
    }
}
