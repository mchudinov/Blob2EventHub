using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Blob2EventHub.Blob2EventHub))]
namespace Blob2EventHub
{
    public class Blob2EventHub : FunctionsStartup
    {        
        private static IDataReceiver _receiver;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configuration = new Configuration();
            configuration.EventHubName = configRoot.GetValue<string>("EventHubName");
            configuration.EventHubConnectionString = configRoot.GetValue<string>("EventHubConnectionString");            
            _receiver = new EventHubReceiver(configuration);
        }

        [FunctionName("Blob2EventHub")]
        public async Task RunAsync([BlobTrigger("%StorageContainerName%/{filename}", Connection = "StorageConnectionString")]Stream blob, string filename, ILogger log)
        {
            try
            {
                var data = blob.ReadToArray();
                await _receiver.ReceiveAsync(data);
                log.LogTrace($"Processed blob:{filename}");
            }
            catch (Exception ex)
            {
                log.LogWarning($"{ex.Message}");
            }            
        }
    }

    static class StreamHelper
    {
        public static byte[] ReadToArray(this Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public static string ReadToString(this Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
