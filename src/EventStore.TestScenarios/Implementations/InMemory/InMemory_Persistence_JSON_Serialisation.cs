using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Dispatcher;
using EventStore.Serialization;

namespace EventStore.TestScenarios.Implementations
{
    internal class InMemory_Persistence_JSON_Serialisation : Implementation 
    {
        public InMemory_Persistence_JSON_Serialisation()
        { }

        public override IStoreEvents WireupEventStore(Action<Commit> dispatcher)
        {
            return Wireup.Init()
               .UsingInMemoryPersistence()
                   .InitializeStorageEngine()
                   .UsingJsonSerialization()
                   .UsingSynchronousDispatchScheduler()// .UsingAsynchronousDispatchScheduler()
                   .DispatchTo(new DelegateMessageDispatcher(dispatcher))
               .Build();
        }
    }
}
