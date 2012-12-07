//using System;
//using System.Threading;
//using CommonDataStructures.Scanner;
//using DistributedTaskDataContracts;
//using DistributedTaskDataContracts.Messages.Reports;
//using DistributedTaskDataContracts.Messages.Reports.FalseCatcher;
//using DistributedTaskDataContracts.Messages.Reports.Scanner;
//using KlSrl.Core.PluginsConfiguration;
//
//namespace EasyNetQPublisher2
//{
//    public static class Reports
//    {
//        static long _lastTaskId;
//
//        static long NextTaskId()
//        {
//            return Interlocked.Increment(ref _lastTaskId);
//        }
//
//        public static class Fc
//        {
//            static readonly Guid PluginGuid = PluginGuids.GetPluginGuid(PluginGuids.FalseCatcherPlugin);
//
//            public static PluginReport Success()
//            {
//                return new SuccessFalseCatcherReport(new CompositeTaskId(NextTaskId())) {PluginGuid = PluginGuid};
//            }
//
//            public static PluginReport Failed()
//            {
//                return new FailedFalseCatcherReport(new CompositeTaskId(NextTaskId()), "Error message 1")
//                    {
//                        PluginGuid = PluginGuid
//                    };
//            }
//        }
//
//        public static class ReScan
//        {
//            static readonly Guid PluginGuid = PluginGuids.GetPluginGuid(PluginGuids.ReScannerPlugin);
//
//            public static PluginReport Success()
//            {
//                var scanResult = new FileScanResult(new[] {new DetectInfo("comment 1", 1)}, true, false,
//                                                    "Error reason 1");
//                return new SuccessSingleFileScannerReport(new CompositeTaskId(NextTaskId()), scanResult)
//                    {
//                        PluginGuid = PluginGuid
//                    };
//            }
//
//            public static PluginReport Failed()
//            {
//                return new FailedScannerReport(new CompositeTaskId(NextTaskId()), "Error message 1")
//                    {
//                        PluginGuid = PluginGuid
//                    };
//            }
//        }
//    }
//}
