using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Dispatcher;

namespace EventStore.TestScenarios.Implementations
{
    internal class MSSQL_Persistence_BSON_Serialisation : Implementation 
    {
        public MSSQL_Persistence_BSON_Serialisation()
        { }

        public override IStoreEvents WireupEventStore(Action<Commit> dispatcher)
        {
            return Wireup.Init()
               .UsingSqlPersistence("MSSQLBSON")
                   .InitializeStorageEngine()
                   .UsingBsonSerialization()
                    .UsingAsynchronousDispatchScheduler()
                   .DispatchTo(new DelegateMessageDispatcher(dispatcher))
               .Build();
        }
    }
}
