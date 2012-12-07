open System
open System.Text
open System.Threading
open RabbitMQ.Client
open RabbitMqUtils
open DistributedTaskDataContracts.Messages.Tasks
open DistributedTaskDataContracts
open Newtonsoft.Json

let task = FalseCatcherTaskMessage(CompositeTaskId(33L), 
                                   PluginGuid = Guid.NewGuid(), 
                                   Path = "\\kldfs.avp.ru\vfs\objectsbysha1\0011223344556677889900112233445566778899",
                                   ScanArchived = false,
                                   ScanPacked = false,
                                   ScanSFXArchived = false,
                                   ScanMailBases = false,
                                   ScanMailPlain = false,
                                   SkipEmbedded = true,
                                   ExpirationTime = DateTime.Now)

let taskAsJson = 
    let json = JsonConvert.SerializeObject(task) 
    json

let taskAsBytes = Encoding.UTF8.GetBytes(taskAsJson)

[<EntryPoint>]
let main argv = 
    let queueName = "test_queue_1"
    let settings = System.Configuration.AppSettingsReader()
    let host = settings.GetValue("rabbitmq_host", typeof<string>) |> string
    use conn = (connectionFactory host).CreateConnection()
    use ch = conn.CreateModel()
    ch.ConfirmSelect()
    ch.QueueDeclare(queueName, true, false, false, null) |> ignore
    ch.ExchangeDeclare("test_exchange", "direct", true, false, null)
    ch.QueueBind(queueName, "test_exchange", queueName)

    let rec loop i =
        let props = ch.CreateBasicProperties()
        props.SetPersistent(true)
        props.CorrelationId <- string i
        ch.BasicPublish("test_exchange", queueName, props, taskAsBytes)
        if i % 100 = 0 then printfn "%d messages have been published" i
        Thread.Sleep(0)
        loop (i + 1)

    loop 0
    printfn "DONE."
    Console.ReadLine() |> ignore
    0 
