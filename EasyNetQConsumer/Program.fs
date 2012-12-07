open System
open System.Text
open System.Threading
open RabbitMQ.Client
open EasyNetQ
open EasyNetQ.Topology
open DistributedTaskDataContracts.Messages.Reports
open DistributedTaskDataContracts.Messages.Reports.FalseCatcher
open DistributedTaskDataContracts.Messages.Reports.Scanner
open System.Threading.Tasks

[<EntryPoint>]
let main argv = 
    let queueName = "test_queue_1"
//    let settings = System.Configuration.AppSettingsReader()
//    let host = settings.GetValue("rabbitmq_host", typeof<string>) |> string
    let host = "localhost"
    use bus = RabbitHutch.CreateBus(sprintf "host=%s;username=guest;password=guest" host)
    let abus = bus.Advanced

    let consumedMessagesCount = ref 0
    
    let handler msg = 
        consumedMessagesCount := !consumedMessagesCount + 1
        if !consumedMessagesCount % 100 = 0 then 
            printfn "%d messages have been consumed so far" !consumedMessagesCount
    
    use abus = bus.Advanced
    let exchange = Exchange.DeclareTopic("reports")
    let queue = Queue.DeclareDurable("reports.false_catcher")
    queue.BindTo(exchange, "false_catcher")
    //abus.Subscribe<PluginReport>(queue, fun (msg: IMessage<PluginReport>, info: MessageReceivedInfo) -> Task.Factory.StartNew(fun() -> ()))

    //bus.Subscribe<SuccessFalseCatcherReport>("1", Action<SuccessFalseCatcherReport> handler, fun x -> x.WithTopic("false_catcher") |> ignore)
    //bus.Subscribe<SuccessSingleFileScannerReport>("1", Action<SuccessSingleFileScannerReport> handler, fun x -> x.WithTopic("rescanner") |> ignore)

    printfn "DONE."
    while true do
        Thread.Sleep(1000000)
    0
