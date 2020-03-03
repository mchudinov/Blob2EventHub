using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Blob2EventHub.Blob2EventHub))]
namespace Blob2EventHub
{
    public class Blob2EventHub : FunctionsStartup
    {
        //private static IDataReceiver _receiver;
        //private Configuration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            var _configuration = new Configuration();
            _configuration.EventHubName = configRoot.GetValue<string>("EventHubName");
            _configuration.EventHubConnectionString = configRoot.GetValue<string>("EventHubConnectionString");
            //_receiver = new EventHubReceiver(_configuration);

            builder.Services.AddSingleton(_configuration);
            builder.Services.AddSingleton<IDataReceiver, EventHubReceiver>();
        }

        [FunctionName("Blob2EventHub")]
        public async Task RunAsync([BlobTrigger("%StorageContainerName%/{filename}", Connection = "StorageConnectionString")]Stream blob, string filename, IDataReceiver receiver, ILogger log)
        {
            try
            {
                var data = blob.ReadToArray();
                await receiver.ReceiveAsync(data);
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
