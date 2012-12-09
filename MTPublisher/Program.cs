using Common;
using MassTransit;

namespace MTPublisher
{
    static class Program
    {
        static void Main()
        {
            using (var bus = ServiceBusFactory.New(c => c.UseRabbitMq().ReceiveFrom("rabbitmq://localhost/")))
            {
                bus.Publish(Reports.Fc.success());
            }
        }
    }
}