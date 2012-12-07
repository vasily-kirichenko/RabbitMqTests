open System
open System.Text
open System.Threading
open RabbitMQ.Client
open DistributedTaskDataContracts.Messages.Reports.FalseCatcher
open DistributedTaskDataContracts.Messages.Reports
open DistributedTaskDataContracts
open EasyNetQ
open EasyNetQ.Topology
open Castle.Windsor
open Castle.Windsor.Installer

[<EntryPoint>]
let main argv = 
    let settings = System.Configuration.AppSettingsReader()
    //let host = settings.GetValue("rabbitmq_host", typeof<string>) |> string
    let host = "localhost"
    let container = new WindsorContainer()
    container.Install(FromAssembly.This()) |> ignore
    use bus = RabbitHutch.CreateBus(sprintf "host=%s;username=guest;password=guest" host)
    use abus = bus.Advanced
    let exchange = Exchange.DeclareTopic("reports")
    let queue = Queue.DeclareDurable("reports.false_catcher")

    use ch = abus.OpenPublishChannel(fun c -> c.WithPublisherConfirms() |> ignore)
    //let msg = Message(successReport)
//    msg.Properties.DeliveryMode <- byte 2
//    ch.Publish<SuccessFalseCatcherReport>(exchange, "false_catcher", msg, fun c -> c.OnSuccess(fun() -> ())
//                                                                                    .OnFailure(fun() -> ()) |> ignore)

//    use ch = bus.OpenPublishChannel(fun config -> config.WithPublisherConfirms() |> ignore)
//    
//    ch.Publish<PluginReport>(successReport, fun c -> c.WithTopic("false_catcher")
//                                                      .OnSuccess(fun() -> ())
//                                                      .OnFailure(fun() -> ()) |> ignore)

//    let rec loop i =
//        ch.Publish(successReport, fun c -> c.WithTopic("false_catcher") |> ignore)
//        if i % 100 = 0 then printfn "%d messages have been published" i
//        Thread.Sleep(1000)
//        loop (i + 1)
//
//    loop 0
    printfn "DONE."
    Console.ReadLine() |> ignore
    0 
