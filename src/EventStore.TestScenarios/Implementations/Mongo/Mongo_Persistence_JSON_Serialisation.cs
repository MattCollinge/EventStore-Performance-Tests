using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Dispatcher;
using EventStore.Serialization;

namespace EventStore.TestScenarios.Implementations
{
    internal class Mongo_Persistence_JSON_Serialisation : Implementation 
    {
        public Mongo_Persistence_JSON_Serialisation()
        { }

        public override IStoreEvents WireupEventStore(Action<Commit> dispatcher)
        {
            return Wireup.Init()
                    .UsingMongoPersistence("Mongo", new DocumentObjectSerializer())
                 .InitializeStorageEngine()
                   .UsingJsonSerialization()
                    .UsingAsynchronousDispatchScheduler()
                   .DispatchTo(new DelegateMessageDispatcher(dispatcher))
               .Build();
        }
    }
}
