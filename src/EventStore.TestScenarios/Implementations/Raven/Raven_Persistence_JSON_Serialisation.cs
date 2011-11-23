using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Dispatcher;

namespace EventStore.TestScenarios.Implementations
{
    internal class Raven_Persistence_JSON_Serialisation : Implementation 
    {
        public Raven_Persistence_JSON_Serialisation()
        { }

        public override IStoreEvents WireupEventStore(Action<Commit> dispatcher)
        {
            return Wireup.Init()
               .UsingRavenPersistence("Raven")
                   .InitializeStorageEngine()
                   .UsingJsonSerialization()
                    .UsingAsynchronousDispatchScheduler()
                   .DispatchTo(new DelegateMessageDispatcher(dispatcher))
               .Build();
        }
    }
}
