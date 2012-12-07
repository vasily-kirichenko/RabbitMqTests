module Reports
open System
open DistributedTaskDataContracts.Messages.Reports.FalseCatcher
open DistributedTaskDataContracts.Messages.Reports.Scanner
open DistributedTaskDataContracts
open CommonDataStructures.Scanner
open KlSrl.Core.PluginsConfiguration

let lastTaskId = ref 0L

let nextTaskId() = 
    lastTaskId := !lastTaskId + 1L
    !lastTaskId

module Fc = 
    let private pluginGuid = PluginGuids.GetPluginGuid(PluginGuids.FalseCatcherPlugin)
    let success() = SuccessFalseCatcherReport(CompositeTaskId(nextTaskId()), PluginGuid = pluginGuid)
    let failed() = FailedFalseCatcherReport(CompositeTaskId(nextTaskId()), "Error message 1", PluginGuid = pluginGuid)

module ReScan = 
    let private pluginGuid = PluginGuids.GetPluginGuid(PluginGuids.ReScannerPlugin)
    let private scanResult = FileScanResult([| DetectInfo("comment 1", Nullable 1L) |], true, false, "Error reason 1")
    let success() = SuccessSingleFileScannerReport(CompositeTaskId(nextTaskId()), scanResult, PluginGuid = pluginGuid)
    let failed() = FailedScannerReport(CompositeTaskId(nextTaskId()), "Error message 1", PluginGuid = pluginGuid)