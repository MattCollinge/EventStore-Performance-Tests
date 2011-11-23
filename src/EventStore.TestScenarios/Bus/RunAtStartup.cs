using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using NServiceBus.Unicast;
using EventStore.TestScenarios.Implementations;
using EventStore.Dispatcher;

namespace EventStore.TestScenarios.Bus
{
    internal class RunAtStartup : IWantToRunAtStartup
    {
        public IBus bus { get; set; }
        public static IBus Bus { get; set; }
        public Wireup wireup { get; set; }
       

        #region IWantToRunAtStartup Members

        public void Run()
        {
          Bus = bus;

          //EventStore =  Wireup.Init()
          //      .UsingSqlPersistence("MSSQLJSON")
          //       .InitializeStorageEngine()
          //       .UsingJsonSerialization()
          //       .UsingAsynchronousDispatchScheduler(new DelegateMessageDispatcher(Dispatch))
          //      .Build();

        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion

       


       
    }
}
