namespace EasyNetQPublisher
open Castle.Windsor.Installer
open Castle.Windsor
open Castle.MicroKernel.Registration
open Castle.MicroKernel.SubSystems.Configuration
open EasyNetQ

//type Installer() =
//    interface IWindsorInstaller with
//        member __.Register(container: IWindsorContainer, store: IConfigurationStore) =
//            container.Register(Component.For<IBus>().UsingFactoryMethod(fun(k, c) -> 
//                let settings = System.Configuration.AppSettingsReader()
//                //let host = settings.GetValue("rabbitmq_host", typeof<string>) |> string
//                let host = "localhost"
//                RabbitHutch.CreateBus(sprintf "host=%s;username=guest;password=guest" host)))



