﻿using DistributedTaskDataContracts;
using DistributedTaskDataContracts.Messages.Reports.FalseCatcher;
using EasyNetQ;
using EasyNetQCommon;
using EasyNetQPublisher2.Properties;

namespace EasyNetQPublisher2
{
    static class Program
    {
        static void Main()
        {
            var host = Settings.Default.RabbitHost;
            var report = new SuccessFalseCatcherReport(new CompositeTaskId(1));

            using (var bus = RabbitHutch.CreateBus(string.Format("host={0};username=guest;password=guest", host),
                                                   r => r.Register<ISerializer>(p => new BinarySerializer())
                                                         .Register<IEasyNetQLogger>(p => new NoDebugLogger())))
            {
                //   new Publisher(bus).RunInfinite();

                using (var ch = bus.OpenPublishChannel(cc => cc.WithPublisherConfirms()))
                {
                    while (true)
                        ch.Publish(report, cc => cc.OnSuccess(() => { }).OnFailure(() => { }));
                }
            }
        }

        //new Publisher(bus).RunManual();
        //new Publisher(bus).RunInfinite();
    }
}