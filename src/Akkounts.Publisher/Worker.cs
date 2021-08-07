using System;
using System.Threading;
using System.Threading.Tasks;
using Akkounts.Publisher.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Akkounts.Publisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private RestClient _restClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _restClient = new RestClient("https://akkounts.azurewebsites.net");
            //_restClient = new RestClient("http://localhost:5000");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var request = new RestRequest("Account");
                var trx = new RandomTransaction();

                request.AddJsonBody(trx);
                _restClient.Post(request);

                _logger.LogInformation("Conta: {account} Valor: {amount} at: {time}", trx.AccountNumber, trx.Amount, DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}