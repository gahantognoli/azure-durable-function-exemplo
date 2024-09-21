using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Company.AzureDurableFunction
{
    public static class DurableFunctionExample
    {
        [FunctionName("DurableFunctionExample")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            var produto = "Bicicleta";

            outputs.Add(await context.CallActivityAsync<string>(nameof(BuscarProduto), produto));
            outputs.Add(await context.CallActivityAsync<string>(nameof(EfetuarPagamento), produto));
            outputs.Add(await context.CallActivityAsync<string>(nameof(Entrega), produto));

            return outputs;
        }

        [FunctionName("BuscarProduto")]
        public static string BuscarProduto([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Bsucando produto {name}...";
        }

        [FunctionName("EfetuarPagamento")]
        public static string EfetuarPagamento([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Realizando pagamento {name}...";
        }

        [FunctionName("Entrega")]
        public static string Entrega([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Realizando entrega {name}...";
        }

        [FunctionName("DurableFunctionExample_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunctionExample", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}