// References
// https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions


using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace TestWorkshop
{
    public class AlertReceiver
    {
        const string ServiceBusConnectionString = "Endpoint=sb://testworkshop2018.servicebus.windows.net/;SharedAccessKeyName=receivingapp;SharedAccessKey=RSfpfFJPicta6Yj7BwY9N86zhZ8rdZ7Q5gFFlJ0uIOY=";
        const string TopicName = "alerts";
        //const string Secret = "RSfpfFJPicta6Yj7BwY9N86zhZ8rdZ7Q5gFFlJ0uIOY=";
        private string SubscriptionName = "generated-GUID-subscription";
        private ISubscriptionClient subscriptionClient;

        public void Initialize()
        {
            //check that we have an individual subscription for this receiver
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

        }
    }
}
