using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using EventStore.TestScenarios;
using System.Threading;

namespace EventStore.TestRunner
{
    public class EventStoreDispatchHandler : IMessageHandler<EventStoreDispatchMessage>
    {

        #region IMessageHandler<IMessage> Members

        public void Handle(EventStoreDispatchMessage message)
        {
            Interlocked.Increment(ref Program.EventCount);
        }

        #endregion
    }
}
