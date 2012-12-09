using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using EasyNetQ;
using EasyNetQ.Topology;

namespace EasyNetQConsumer2
{
    internal class Consumer
    {
        private readonly IBus _bus;
        int _totalCount;

        public Consumer(IBus bus)
        {
            _bus = bus;
        }

        public void Run()
        {
            var exchange = Exchange.DeclareTopic("reports", false, null);
            
            var fcQueue = Queue.DeclareTransient("reports.false_catcher.all");
            fcQueue.BindTo(exchange, "SuccessFc", "FailedFc");
            
            var rescanQueue = Queue.DeclareTransient("reports.rescanner.all");
            rescanQueue.BindTo(exchange, "SuccessReScan", "FailedReScan");

            _bus.Advanced.Subscribe<Envelop>(fcQueue, (msg, info) =>
                                                       Task.Factory.StartNew(
                                                           () =>
                                                               {
                                                                   //Console.WriteLine("FC: {0} received.", msg);
                                                                   Log();
                                                               }));
            
            _bus.Advanced.Subscribe<Envelop>(rescanQueue, (msg, info) =>
                                                       Task.Factory.StartNew(
                                                           () =>
                                                               {
                                                                   //Console.WriteLine("ReScan: {0} received.", msg);
                                                                   Log();
                                                               }));
        }

        private void Log()
        {
            var count = Interlocked.Increment(ref _totalCount);

            if (count % 100 == 0)
                Console.WriteLine("{0} messages consumed.", count);
        }
    }
}