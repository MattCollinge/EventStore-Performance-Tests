using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using EventStore.TestScenarios.Implementations;

namespace EventStore.TestScenarios
{
    public abstract class EventStorePerfTest<T> where T : class, new()
    {

        protected IStoreEvents Store;

        #region EventStore Interaction
        
        protected abstract void DispatchCommit(Commit commit);

        protected void CreateStreamAndAppendEvents(Guid streamId, List<T> events)
        {
            using (var stream = Store.CreateStream(streamId))
            {
                events.ForEach(@event => stream.Add(new EventMessage { Body = @event }));

                stream.CommitChanges(Guid.NewGuid());
            }
        }

        protected void OpenStreamAndAppendEvents(Guid streamId, List<T> events)
        {
            using (var stream = Store.OpenStream(streamId, int.MinValue, int.MaxValue))
            {
                events.ForEach(@event => stream.Add(new EventMessage { Body = @event }));

                stream.CommitChanges(Guid.NewGuid());
            }
        }

        protected void TakeSnapshot(Guid streamId, object memento)
        {
            Store.Advanced.AddSnapshot(new Snapshot(streamId, 2, memento));
        }

        protected void LoadFromSnapshotForwardAndAppend(Guid streamId, List<T> events)
        {
            var latestSnapshot = Store.Advanced.GetSnapshot(streamId, int.MaxValue);

            using (var stream = Store.OpenStream(latestSnapshot, int.MaxValue))
            {
                events.ForEach(@event => stream.Add(new EventMessage { Body = @event }));

                stream.CommitChanges(Guid.NewGuid());
            }
        }
     
        #endregion

        public int InitialCommitsInEventSource { get; private set; }
        public int EventCountPerCommit { get; private set; }
        public int CommitCount { get; private set; }
        public int TotalRuns { get; private set; }
        public int ConcurrentThreads { get; private  set; }
        public int SnapshotThreshold { get; private set; }

        private EventCreator<T> EventGenerator = new EventCreator<T>();
        private List<Guid> eventSourceIds = new List<Guid>();
        private List<Thread> concurrentEventProcessors = new List<Thread>();
        protected Dictionary<string, Implementation> implementations = new Dictionary<string, Implementation>();

        public void Setup(int totalRuns, int commitCount,
            int concurrentThreads, int initialCommitsInEventSource, int snapshotThreshold, int eventCountPerCommit)
        {
            this.EventCountPerCommit = eventCountPerCommit;
            this.TotalRuns = totalRuns;
            this.CommitCount = commitCount;
            this.ConcurrentThreads = concurrentThreads;
            this.InitialCommitsInEventSource = initialCommitsInEventSource;
            this.SnapshotThreshold = snapshotThreshold;

            

            //Set up Threads for concurrent Operation
            //Only support 1 Thread for now
            //Work out how many runs per Thread
                      
           
            RegisterImplementations();
        }

        private void SetupExisitingEventSources(int concurrentThreads, int initialCommitsInEventSource, int eventCountPerCommit)
        {
            //Set up initial commits for each concurrent Thread
            if (initialCommitsInEventSource > 0)
            {
                Guid exisitngStreamID = Guid.Empty;

                for (int i = 0; i < concurrentThreads; i++)
                {
                    exisitngStreamID = Guid.NewGuid();
                    eventSourceIds.Add(exisitngStreamID);
                    for (int j = 0; j < initialCommitsInEventSource; j++)
                        this.CreateStreamAndAppendEvents(exisitngStreamID, EventGenerator.CreateEvents(eventCountPerCommit));
                }
            }
        }

        private void RegisterImplementations() 
        {
            //InMemory
            //implementations.Add("InMemory_Persistence_JSON_Serialisation", new InMemory_Persistence_JSON_Serialisation());

            //MS SQL
            implementations.Add("MSSQL Persistence & JSON Serialisation", new MSSQL_Persistence_JSON_Serialisation());
           // implementations.Add("MSSQL Persistence & BSON Serialisation", new MSSQL_Persistence_BSON_Serialisation());
           // implementations.Add("MSSQL Persistence & Binary Serialisation", new MSSQL_Persistence_Binary_Serialisation());
           // implementations.Add("MSSQL Persistence & ServiceStack Serialisation", new MSSQL_Persistence_ServiceStack_Serialisation());

              //Mongo DB
           implementations.Add("Mongo Persistence & JSON Serialisation", new Mongo_Persistence_JSON_Serialisation());
            //implementations.Add("Mongo Persistence & BSON Serialisation", new Mongo_Persistence_BSON_Serialisation());
            //implementations.Add("Mongo Persistence & Binary Serialisation", new Mongo_Persistence_Binary_Serialisation());
            //implementations.Add("Mongo Persistence & ServiceStack Serialisation", new Mongo_Persistence_ServiceStack_Serialisation());

            ////Raven DB
           // implementations.Add("Raven Persistence & JSON Serialisation", new Raven_Persistence_JSON_Serialisation());
            //implementations.Add("Raven Persistence & BSON Serialisation", new Raven_Persistence_BSON_Serialisation());
            //implementations.Add("Raven Persistence & Binary Serialisation", new Raven_Persistence_Binary_Serialisation());
            //implementations.Add("Raven Persistence & ServiceStack Serialisation", new Raven_Persistence_ServiceStack_Serialisation());

         
        }

        /// <summary>
        /// Execute Scenario for supplied Event Source Id.
        /// </summary>
        /// <param name="currentEventSourceId"></param>
        /// <returns></returns>
        public void RunPass(Guid currentEventSourceId, List<T> eventsToStore)
        {

            for (int j = 0; j < this.CommitCount; j++)
                this.CreateStreamAndAppendEvents(currentEventSourceId, eventsToStore); //this.OpenStreamAndAppendEvents(currentEventSourceId, eventsToStore);

        }

        public long Run(Implementation currentImplementation)
        {
            var sw = new Stopwatch();
            var iterations = this.TotalRuns * this.CommitCount;
            var eventsToStore = EventGenerator.CreateEvents(EventCountPerCommit);
            var runsPerThread = TotalRuns / ConcurrentThreads;

            //Set up EventStore
            this.Store = currentImplementation.WireupEventStore(new Action<Commit>(DispatchCommit));

            SetupExisitingEventSources(ConcurrentThreads, InitialCommitsInEventSource, EventCountPerCommit);
            
            //Set up Run
            concurrentEventProcessors.Clear();

            for (int i = 0; i < ConcurrentThreads; i++)
                concurrentEventProcessors.Add(new Thread(ThreadStart) { Name = string.Format("Event processor {0}", i) });

            //Warm up eventSotre
            //var warmup = new Thread(ThreadStart) { Name = string.Format("Event processor {0}", "warmup") };
            //warmup.Start(new Tuple<List<T>, int>(eventsToStore, runsPerThread));
            //warmup.Join();


            sw.Start();

            concurrentEventProcessors.ForEach(t => t.Start(new Tuple<List<T>, int>(eventsToStore, runsPerThread)));
            concurrentEventProcessors.ForEach(t => t.Join());

            var opsPerSecond = (iterations * 1000L) / sw.ElapsedMilliseconds;
            return opsPerSecond;
        }

        private void ThreadStart(object state)
        {
            var t = state as Tuple<List<T>, int>;
            PerformRun(t.Item1, t.Item2);
        }

        private void PerformRun(List<T> eventsToStore, int runs)
        {
            for (int i = 0; i < runs; i++)
            {
                if (eventSourceIds.Count == 0)
                    RunPass(Guid.NewGuid(), eventsToStore);

                //TODO: Sort out EventSources with existing Commits
                //var opsPerSecond = Thread Strart & join Method.....  ForEach RunPass(eventSourceIds[i]);
            }
        }
    }
}
