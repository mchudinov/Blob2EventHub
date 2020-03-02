using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Blob2EventHub
{
    public static class Blob2EventHub
    {
        [FunctionName("Blob2EventHub")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "xxx")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
