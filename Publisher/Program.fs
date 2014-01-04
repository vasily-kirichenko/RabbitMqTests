open System
open System.IO
open System.Text
open System.Threading
open RabbitMQ.Client
open RabbitMqUtils
open DistributedTaskDataContracts.Messages.Tasks
open DistributedTaskDataContracts
open Newtonsoft.Json
open FsPickler
open System.Runtime.Serialization.Formatters.Binary

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

let taskAsPicklerBytes =
    let p = FsPickler()
    fun() ->
        use ms = new MemoryStream()
        p.Serialize (ms, task :> obj)
        ms.Position <- 0L
        ms.ToArray()

let taskAsBinaryFormatterBytes =
    let f = BinaryFormatter()
    fun() ->
        use ms = new MemoryStream()
        f.Serialize(ms, task)
        ms.Position <- 0L
        ms.ToArray()

[<EntryPoint>]
let main argv = 
    let queueName = "test_queue_1"
    let settings = System.Configuration.AppSettingsReader()
    let host = settings.GetValue("rabbitmq_host", typeof<string>) |> string
    use conn = (connectionFactory host).CreateConnection()
    use ch = conn.CreateModel()
    //ch.ConfirmSelect()
    ch.QueueDeclare(queueName, true, false, false, null) |> ignore
    ch.ExchangeDeclare("test_exchange", "direct", true, false, null)
    ch.QueueBind(queueName, "test_exchange", queueName)
    let props = ch.CreateBasicProperties()

    let rec loop i =
        //props.SetPersistent(true)
        //props.CorrelationId <- string i
        //let bytes = taskAsBinaryFormatterBytes() 
        let bytes = taskAsPicklerBytes()
        ch.BasicPublish("test_exchange", queueName, props, bytes) 
        if i % 1000 = 0 then printfn "%d messages have been published" i
        Thread.Sleep(0)
        loop (i + 1)

    loop 0
    printfn "DONE."
    Console.ReadLine() |> ignore
    0 
