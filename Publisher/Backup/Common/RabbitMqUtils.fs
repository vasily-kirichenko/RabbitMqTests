module RabbitMqUtils
open RabbitMQ.Client

let connectionFactory host = ConnectionFactory(HostName = host, Port = 5672, UserName = "guest", Password = "guest")
