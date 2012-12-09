using System;
using EasyNetQ;
using EasyNetQCommon;
using EasyNetQConsumer2.Properties;

namespace EasyNetQConsumer2
{
    static class Program
    {
        static void Main()
        {
            var host = Settings.Default.RabbitHost;

            using (var bus = RabbitHutch.CreateBus(string.Format("host={0};username=guest;password=guest", host),
                                                  // r => r.Register<ISerializer>(p => new BinarySerializer())
                                                  r => r.Register<IEasyNetQLogger>(p => new NoDebugLogger())))
            {
                //var auto = new AutoSubscriber(bus, "1");
                //auto.Subscribe(Assembly.GetExecutingAssembly());
                new Consumer(bus).Run();
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}