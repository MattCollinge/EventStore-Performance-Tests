using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Dispatcher;

namespace EventStore.TestScenarios.Implementations
{
    internal class MSSQL_Persistence_JSON_Serialisation : Implementation 
    {
        public MSSQL_Persistence_JSON_Serialisation()
        { }

        public override IStoreEvents WireupEventStore(Action<Commit> dispatcher)
        {
            return Wireup.Init()
               .UsingSqlPersistence("MSSQLJSON")
                   .InitializeStorageEngine()
                   .UsingJsonSerialization()
                    .UsingSynchronousDispatchScheduler()
                   .DispatchTo(new DelegateMessageDispatcher(dispatcher))
               .Build();
        }
    }
}
