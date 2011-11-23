using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using EventStore.TestScenarios.Bus;

namespace EventStore.TestScenarios
{
    public class EventToStoreMessageHandler : IMessageHandler<NServiceBusEventMessage>
    {
        public IBus bus { get; set; }

        #region IMessageHandler<NServiceBusEventMessage> Members

        public void Handle(NServiceBusEventMessage message)
        {
            using (var stream = StaticEventStore.GetEventStore(bus).CreateStream(message.Id))
            {
                stream.Add(new EventMessage { Body = message });

                stream.CommitChanges(Guid.NewGuid());
            }
        }

        #endregion
    }
}
