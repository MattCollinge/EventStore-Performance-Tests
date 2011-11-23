using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EventStore.TestScenarios.Implementations;
using System.Diagnostics;

namespace EventStore.TestScenarios.Scenarios
{
    [TestFixture]
   public  class NewEventSourceSmallEvent : EventStorePerfTest<SmallEvent>
    {
        protected override void DispatchCommit(Commit commit)
        {
            // This is where we'd hook into our messaging infrastructure, such as NServiceBus,
            // MassTransit, WCF, or some other communications infrastructure.
            // This can be a class as well--just implement IDispatchCommits.
            //try
            //{
            //    foreach (var @event in commit.Events)
            //        Console.WriteLine("Message Dispatched: " + @event.Body.ToString());
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Dispatch Failure");
            //}
        }


        [SetUp]
        public void SetupUnitTest(int runCount, int commitsPerRun, int concurrentThreads, int initialCommitsInStore, int snapshotThreshold, int eventsPerCommit)
        {
            //1M runs
            //1 Commit per Run
            //10 Concurrent Threads
            //0 Initial Commits in Store
            //0 SnapShotThreshold
            //1 Event Per Commit

            Setup(runCount,
                commitsPerRun,
                concurrentThreads,
                initialCommitsInStore,
                snapshotThreshold,
                eventsPerCommit 
                );
        }

        [Test]
        public void RunAsUnitTest()
        {
            foreach (var kvp in implementations)
            {
                Stopwatch sw = Stopwatch.StartNew();
                var ops = Run(kvp.Value);
                sw.Stop();
                Console.WriteLine("{0} : Ops/s: {1}, Total Ops: {2}, Time taken: {3:# ###.##}", kvp.Key, ops, TotalRuns, sw.ElapsedMilliseconds/1000);
            }
        }

    }
}
