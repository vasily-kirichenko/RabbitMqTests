using System;
using System.Threading;
using DistributedTaskDataContracts.Messages.Reports.FalseCatcher;
using DistributedTaskDataContracts.Messages.Reports.Scanner;
using EasyNetQ;

namespace EasyNetQConsumer2
{
    public class AutoConsumer :
        IConsume<SuccessFalseCatcherReport>,
        IConsume<FailedFalseCatcherReport>,
        IConsume<SuccessSingleFileScannerReport>,
        IConsume<FailedScannerReport>
    {
        static int _totalCount;

        [Consumer(SubscriptionId = "1")]
        public void Consume(SuccessFalseCatcherReport message)
        {
            Log(message);
        }

        [Consumer(SubscriptionId = "1")]
        public void Consume(FailedFalseCatcherReport message)
        {
            Log(message);
        }

        [Consumer(SubscriptionId = "1")]
        public void Consume(SuccessSingleFileScannerReport message)
        {
            Log(message);
        }

        [Consumer(SubscriptionId = "1")]
        public void Consume(FailedScannerReport message)
        {
            Log(message);
        }

        static void Log(object message)
        {
            var currentCount = Interlocked.Increment(ref _totalCount);
            Console.WriteLine(message);
            //if (currentCount % 100 == 0)
                Console.WriteLine("Consumed {0} messages.", currentCount);
        }
    }
}
