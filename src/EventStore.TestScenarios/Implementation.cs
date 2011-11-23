using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.TestScenarios
{
   public abstract class Implementation
    {
        public abstract  IStoreEvents WireupEventStore(Action<Commit> dispatcher);
    }
}
