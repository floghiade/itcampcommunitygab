using AzureWorkshop.Hubs;
using ITCamp.Gab.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureWorkshop
{
    public class BackgroundService : IHostedService
    {
        private Task backgroundJob;
        private CancellationTokenSource privateToken;
        private CountrySettings settings;

        private AlertsHubManager hub;

        const string ServiceBusConnectionString =
            @"Endpoint=sb://__SB_ENDPOINT__;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=__SB_SAS_KEY__";
        const string TopicName = "livealert";
        const string SubscriptionName = "{0}live";
        private ISubscriptionClient subscriptionClient;

        public BackgroundService(AlertsHubManager hub, IOptions<CountrySettings> settings)
        {
            this.hub = hub;
            this.settings = settings.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            privateToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            //hub = new AlertsHubManager();

            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, string.Format(SubscriptionName, settings.Code.ToLower()));
            subscriptionClient.RegisterMessageHandler(ProcessMessageAsync, new MessageHandlerOptions(ExceptionReceivedHandler)
                { AutoComplete = false, MaxConcurrentCalls = 1 });

            return Task.Delay(TimeSpan.FromMilliseconds(-1), privateToken.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            privateToken.Cancel();

            cancellationToken.ThrowIfCancellationRequested();

            while (!subscriptionClient.IsClosedOrClosing) { };

            return Task.CompletedTask;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken cancellation)
        {
            //BrokeredMessage
            var bytesAsString = Encoding.UTF8.GetString(message.Body);
            //THis is a hack for .Net Core SB library
            bytesAsString = bytesAsString.Substring(bytesAsString.IndexOf('{'));
            //bytesAsString = "{\"CountryCode\":\"RO\",\"AlertType\":11,\"Level\":0,\"ValidFrom\":\"2018 - 04 - 06T03: 27:00.0212835Z\",\"Period\":\"00:04:00\"}";
            //bytesAsString = "{\"AlertType\":5,\"CountryCode\":\"BG\",\"Level\":0,\"Period\":\"PT3M\",\"ValidFrom\":\"2018-04-11T12:02:00Z\"}";
            var alert = JsonConvert.DeserializeObject<WeatherAlert>(bytesAsString);

            await hub.SendAlert(alert);

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }
        
    }
}
