using System;
using System.Threading;
using DistributedTaskDataContracts.Messages.Reports.FalseCatcher;
using DistributedTaskDataContracts.Messages.Reports.Scanner;
using EasyNetQ;

namespace EasyNetQConsumer2
{
    public class Consumer :
        IConsume<SuccessFalseCatcherReport>,
        IConsume<FailedFalseCatcherReport>,
        IConsume<SuccessSingleFileScannerReport>,
        IConsume<FailedScannerReport>
    {
        static int _totalCount;

        [Consumer(SubscriptionId = "1")]
        public void Consume(SuccessFalseCatcherReport message)
        {
            Log();
        }

        [Consumer(SubscriptionId = "1")]
        public void Consume(FailedFalseCatcherReport message)
        {
            Log();
        }

        [Consumer(SubscriptionId = "1")]
        public void Consume(SuccessSingleFileScannerReport message)
        {
            Log();
        }

        [Consumer(SubscriptionId = "1")]
        public void Consume(FailedScannerReport message)
        {
            Log();
        }

        static void Log()
        {
            var currentCount = Interlocked.Increment(ref _totalCount);

            if (currentCount % 100 == 0)
                Console.WriteLine("Consumed {0} messages.", currentCount);
        }
    }
}
