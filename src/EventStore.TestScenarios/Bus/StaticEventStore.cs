using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Dispatcher;
using NServiceBus;

namespace EventStore.TestScenarios.Bus
{
    class StaticEventStore
    {

        private static volatile IStoreEvents instance;
        private static object syncRoot = new Object();
        private static IBus bus = null;
        

        private StaticEventStore()
        {

        }

        public static IStoreEvents GetEventStore(IBus bus)
        {           
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            StaticEventStore.bus = bus;
                            instance = Wireup.Init()
                                        .UsingSqlPersistence("MSSQLJSON")
                                         .InitializeStorageEngine()
                                         .UsingJsonSerialization()
                                         .UsingAsynchronousDispatchScheduler(new DelegateMessageDispatcher(Dispatch))
                                        .Build();
                        }
                    }
                }

                return instance;
            
        }

        #region IDispatchCommits Members

        public static void Dispatch(Commit commit)
        {
            for (var i = 0; i < commit.Events.Count; i++)
            {
                //var eventMessage = commit.Events[i];                
                //var busMessage = eventMessage.Body as IMessage;
                RunAtStartup.Bus.Publish(new EventStoreDispatchMessage());
            }
        }

        #endregion
    }
}
