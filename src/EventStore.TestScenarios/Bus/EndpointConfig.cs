using System;
using NServiceBus;
using System.Reflection;
using System.Collections.Generic;
using EventStore.TestScenarios.Implementations;
using EventStore.Dispatcher;
using NServiceBus.ObjectBuilder;

namespace EventStore.TestScenarios
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public IBus bus { get; set; }

        public void Init()
        {
            NServiceBus.SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);

            var c = Configure.With()
                           // .Log4Net()
                            .DefaultBuilder()
                            .BinarySerializer()
                            .MsmqTransport()
                            .PurgeOnStartup(true)
                            .InMemorySubscriptionStorage()
                            .UnicastBus();
        }

    }
}
