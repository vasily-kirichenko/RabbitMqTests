using System;
using System.Threading;
using DistributedTaskDataContracts.Messages.Reports;
using DistributedTaskDataContracts.Messages.Reports.FalseCatcher;
using DistributedTaskDataContracts.Messages.Reports.Scanner;
using EasyNetQ;
using RabbitMQ.Client;
using KlSrl.Core.Extensions;

namespace EasyNetQPublisher2
{
    class Publisher
    {
        readonly IBus _bus;
        int _totalCount;

        public Publisher(IBus bus)
        {
            _bus = bus;
            CreateQueues();
        }
        
        public void RunManual()
        {
            using (var ch = _bus.OpenPublishChannel(cc => cc.WithPublisherConfirms()))
            {
                while (true)
                {
                    Console.WriteLine("1 - Success FC, 2 - Failed FC, 3 - Success Scan, 4 - Failed Scan");
                    PublishById(ch, int.Parse(new string(new[] {Console.ReadKey().KeyChar})));
                }
            }
        }

        public void RunInfinite()
        {
            var random = new Random();
            
            using (var c = _bus.OpenPublishChannel(cc => cc.WithPublisherConfirms()))
            {
                while (true)
                {
                    var id = Math.Max(1, Math.Min(random.Next(1, 5), 4));
                    PublishById(c, id);
                    Thread.Sleep(0);
                }
            }
        }

        void PublishById(IPublishChannel ch, int id)
        {
            switch (id)
            {
                case 1:
                    Publish(ch, Reports.Fc.success());
                    break;
                case 2:
                    Publish(ch, Reports.Fc.failed());
                    break;
                case 3:
                    Publish(ch, Reports.ReScan.success());
                    break;
                case 4:
                    Publish(ch, Reports.ReScan.failed());
                    break;
                default:
                    {
                        Console.WriteLine("Wrong id");
                        return;
                    }
            }
        }

        static void CreateQueues()
        {
            var conventions = new Conventions();
            var factory = new ConnectionFactory {HostName = "localhost", UserName = "guest", Password = "guest"};

            using (var conn = factory.CreateConnection())
            using (var ch = conn.CreateModel())
            {
                new[]
                    {
                        typeof (SuccessFalseCatcherReport), typeof (FailedFalseCatcherReport),
                        typeof (SuccessSingleFileScannerReport),
                        typeof (FailedScannerReport)
                    }.Each(rt =>
                        {
                            var queue = ch.QueueDeclare(conventions.QueueNamingConvention(rt, "1"), true, false, false,
                                                        null);
                            var exchangeName = conventions.ExchangeNamingConvention(rt);
                            ch.ExchangeDeclare(exchangeName, "topic", true, false, null);
                            ch.QueueBind(queue.QueueName, exchangeName, "#");
                        });
            }
        }

        void OnSuccess(){}
        void OnFailure(){}

        void Publish<T>(IPublishChannel ch, T report) where T : PluginReport
        {
            ch.Publish(report, cc => cc.OnSuccess(OnSuccess).OnFailure(OnFailure));
            var currentCount = Interlocked.Increment(ref _totalCount);

            if (currentCount % 100 == 0)
                Console.WriteLine("{0} messages published.", currentCount);
        }
    }
}

//                    var pubConfigType = typeof (IPublishConfiguration<>).MakeGenericType(report.GetType());

//                    var confType = typeof (Action<>).MakeGenericType(pubConfigType);

//

//                    var publishMethod =

//                        c.GetType()

//                         .GetMethods()

//                         .Where(m => m.Name == "Publish")

//                         .First(m => m.GetParameters().Count() == 2);

//

//                    var wrapMethod = typeof (Helper).GetMethod("Wrap");

//                    var gwrapMethod = wrapMethod.MakeGenericMethod(new[] {pubConfigType});

//                    var gAction = gwrapMethod.Invoke(null, new object[0]);

//

//                    var gmethod = publishMethod.MakeGenericMethod(report.GetType());

//                    gmethod.Invoke(c, BindingFlags.Instance, null, new[] {report, gAction}, CultureInfo.CurrentCulture);

//        class Helper
//        {
//            public static Action<T> Wrap<T>()
//            {
//                return c =>
//                    {
//                        var pubConfigType = typeof (IPublishConfiguration<>).MakeGenericType(typeof (T));
//                        var onSuccessMethod = pubConfigType.GetMethod("OnSuccess");
//                        var action = new Action(Foo);
//                        var paramType = onSuccessMethod.GetParameters()[0].ParameterType;
//                        var eq = action.GetType() == paramType;
//                        onSuccessMethod.Invoke(c, new object[]{ action });
//                       // pubConfigType.GetMethod("OnFailure").Invoke(c, new object[] {new Action(() => {})});
//                    };
//            }
//
//            static void Foo(){}
//        }
