using System;
using System.Threading;
using Common;
using EasyNetQ;
using EasyNetQ.Topology;
using Exchange = EasyNetQ.Topology.Exchange;

namespace EasyNetQPublisher2
{
    class Publisher
    {
        readonly IBus _bus;
        int _totalCount;
        private IExchange _exchange;

        public Publisher(IBus bus)
        {
            _bus = bus;
            _exchange = Exchange.DeclareTopic("reports", false, null);
            //CreateQueues();
        }
        
        public void RunManual()
        {
            using (var channel = _bus.Advanced.OpenPublishChannel(conf => conf.WithPublisherConfirms()))
            {
                while (true)
                {
                    Console.WriteLine("1 - Success FC, 2 - Failed FC, 3 - Success Scan, 4 - Failed Scan");
                    PublishById(channel, int.Parse(new string(new[] {Console.ReadKey().KeyChar})));
                }
            }
        }

        public void RunInfinite()
        {
            var random = new Random();
            
            using (var channel = _bus.Advanced.OpenPublishChannel(conf => conf.WithPublisherConfirms()))
            {
                while (true)
                {
                    PublishById(channel, Math.Max(1, Math.Min(random.Next(1, 5), 4)));
                    Thread.Sleep(0);
                }
            }
        }

        void PublishById(IAdvancedPublishChannel channel, int id)
        {
            switch (id)
            {
                case 1:
                    Publish(channel, new SimpleReports.SuccessFc(1, "p"));
                    break;
                case 2:
                    Publish(channel, new SimpleReports.FailedFc(2, "p1"));
                    break;
                case 3:
                    Publish(channel, new SimpleReports.SuccessReScan(4, "p2"));
                    break;
                case 4:
                    Publish(channel, new SimpleReports.FailedReScan(4, "p3"));
                    break;
                default:
                    {
                        Console.WriteLine("Wrong id");
                        return;
                    }
            }
        }
        
//        void PublishById(IAdvancedPublishChannel channel, int id)
//        {
//            switch (id)
//            {
//                case 1:
//                    Publish(channel, Reports.Fc.success());
//                    break;
//                case 2:
//                    Publish(channel, Reports.Fc.failed());
//                    break;
//                case 3:
//                    Publish(channel, Reports.ReScan.success());
//                    break;
//                case 4:
//                    Publish(channel, Reports.ReScan.failed());
//                    break;
//                default:
//                    {
//                        Console.WriteLine("Wrong id");
//                        return;
//                    }
//            }
//        }

        void Publish<T>(IAdvancedPublishChannel channel, T report)
        {
            var envelop = new Envelop(report);
            var message = new Message<Envelop>(envelop);
            message.Properties.DeliveryMode = 1;
            message.Properties.Headers["content-type"] = envelop.ContentType;
            
            channel.Publish(_exchange, envelop.ContentType, message,
                            conf => conf.OnSuccess(OnSuccess).OnFailure(OnFailure));

            var currentCount = Interlocked.Increment(ref _totalCount);

            if (currentCount % 100 == 0)
                Console.WriteLine("{0} messages published.", currentCount);
        }

//        private static void CreateQueues()
//        {
//            var factory = new ConnectionFactory {HostName = "localhost", UserName = "guest", Password = "guest"};
//
//            using (var conn = factory.CreateConnection())
//            using (var ch = conn.CreateModel())
//            {
//                var queue = ch.QueueDeclare("reports.false_catcher", true, false, false, null);
//                const string exchangeName = "reports";
//                ch.ExchangeDeclare(exchangeName, "topic", true, false, null);
//                ch.QueueBind(queue.QueueName, exchangeName, "#");
//            }
//        }

//        static void CreateQueues()
//        {
//            var conventions = new Conventions();
//            var factory = new ConnectionFactory {HostName = "localhost", UserName = "guest", Password = "guest"};
//
//            using (var conn = factory.CreateConnection())
//            using (var ch = conn.CreateModel())
//            {
//                new[]
//                    {
//                        typeof (SuccessFalseCatcherReport), typeof (FailedFalseCatcherReport),
//                        typeof (SuccessSingleFileScannerReport),
//                        typeof (FailedScannerReport)
//                    }.Each(rt =>
//                        {
//                            var queue = ch.QueueDeclare(conventions.QueueNamingConvention(rt, "1"), true, false, false, null);
//                            var exchangeName = conventions.ExchangeNamingConvention(rt);
//                            ch.ExchangeDeclare(exchangeName, "topic", true, false, null);
//                            ch.QueueBind(queue.QueueName, exchangeName, "#");
//                        });
//            }
//        }

        void OnSuccess(){}
        void OnFailure(){}
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
