using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using EventStore.Dispatcher;

namespace EventStore.TestScenarios
{
    public sealed class NServiceBusCommitDispatcher : IDispatchCommits
    {
        private const string AggregateIdKey = "AggregateId";
        private const string CommitVersionKey = "CommitVersion";
        private const string EventVersionKey = "EventVersion";
        private const string BusPrefixKey = "Bus.";
        public IBus bus { 
            get; 
            set;
        }


        public NServiceBusCommitDispatcher(IBus bus)
        {
            this.bus = bus;
        }

        public NServiceBusCommitDispatcher()
        {

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        private static void AppendHeaders(IMessage message, IEnumerable<KeyValuePair<string, object>> headers)
        {
            headers = headers.Where(x => x.Key.StartsWith(BusPrefixKey));
            foreach (var header in headers)
            {
                var key = header.Key.Substring(BusPrefixKey.Length);
                var value = (header.Value ?? string.Empty).ToString();
                message.SetHeader(key, value);
            }
        }
        private static void AppendVersion(Commit commit, int index)
        {
            var busMessage = commit.Events[index].Body as IMessage;
            busMessage.SetHeader(AggregateIdKey, commit.StreamId.ToString());
            busMessage.SetHeader(CommitVersionKey, commit.StreamRevision.ToString());
            busMessage.SetHeader(EventVersionKey, GetSpecificEventVersion(commit, index).ToString());
        }
        private static int GetSpecificEventVersion(Commit commit, int index)
        {
            // e.g. (StreamRevision: 120) - (5 events) + 1 + (index @ 4: the last index) = event version: 120
            return commit.StreamRevision - commit.Events.Count + 1 + index;
        }

        #region IDispatchCommits Members

        public void Dispatch(Commit commit)
        {
            for (var i = 0; i < commit.Events.Count; i++)
            {
                var eventMessage = commit.Events[i];
                var busMessage = eventMessage.Body as IMessage;
                //AppendHeaders(busMessage, commit.Headers);
                //AppendHeaders(busMessage, eventMessage.Headers);
                //AppendVersion(commit, i);
                this.bus.Publish(busMessage);
            }
        }

        #endregion
    }
}