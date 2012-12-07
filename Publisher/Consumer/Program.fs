open System
open System.Text
open System.Threading
open RabbitMQ.Client
open RabbitMqUtils

type InteractiveConsumer() =
    inherit DefaultBasicConsumer()

    override this.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body) =
        printfn "Message received: cid = %s, body = %s" properties.CorrelationId (Encoding.UTF8.GetString(body))
        
        let rec readKey() =
            printfn "Press <A> to ack all received messages so far or press <S> to receive another message"
            match Console.ReadKey().Key with
            | ConsoleKey.A -> this.Model.BasicAck(deliveryTag, true)
            | ConsoleKey.S -> ()
            | c -> eprintfn "Unexpected key '%A'" c
                   readKey()
        readKey()

type FastConsumer() =
    inherit DefaultBasicConsumer()
    let mutable consumedMessagesCount: int = 0
    
    override this.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body) =
        consumedMessagesCount <- consumedMessagesCount + 1
        if consumedMessagesCount % 100 = 0 then 
            printfn "%d messages have been consumed so far" consumedMessagesCount
        
        this.Model.BasicAck(deliveryTag, false)

[<EntryPoint>]
let main argv = 
    let queueName = "test_queue_1"
    let settings = System.Configuration.AppSettingsReader()
    let host = settings.GetValue("rabbitmq_host", typeof<string>) |> string
    use conn = (connectionFactory host).CreateConnection()
    use ch = conn.CreateModel()
    //ch.BasicQos(0u, uint16 1, false)
    ch.QueueDeclare(queueName, true, false, false, null) |> ignore
    ch.ExchangeDeclare("test_exchange", "direct", true, false, null)
    ch.QueueBind(queueName, "test_exchange", queueName)
    ch.BasicConsume(queueName, false, FastConsumer(Model = ch)) |> ignore
    printfn "DONE."
    while true do
        Thread.Sleep(1000000)
    0
