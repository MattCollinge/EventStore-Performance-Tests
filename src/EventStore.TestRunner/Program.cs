using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EventStore.TestScenarios.Scenarios;
using NServiceBus;
using EventStore.TestScenarios;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.Configuration;

namespace EventStore.TestRunner
{
    class Program
    {
        public static int EventCount = 0;
        private static IBus Bus = null;
        private static int lastCount = 0;
        private static int timerInterval = 30000;
        private static int timerElapsedCount = 0;

        static void Main(string[] args)
        {
            //RunNServiceBusTests();
            int iterations = int.Parse(ConfigurationManager.AppSettings["iterations"]);
            int threads = int.Parse(ConfigurationManager.AppSettings["threadCount"]);

            RunInProcPerfTests(iterations, threads);

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();
        }

        private static void RunNServiceBusTests(int iterations)
        {
           
            SetUpNSB();

            var t = new System.Timers.Timer();
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Interval = timerInterval;
            t.Start();

            Thread.Sleep(5000);
            Console.WriteLine("Started Sending Events to Store...");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                Bus.Send("EventStorePerfQueue", new NServiceBusEventMessage());

            var interval = sw.ElapsedMilliseconds;

            var opsPerSecond = (iterations * 1000L) / interval;


            Console.WriteLine("{0} : Ops/s: {1}, Total Ops: {2}, Time taken: {3:# ###.##}", "Sending Events Finished", opsPerSecond, iterations, interval / 1000);
            //string line;
            //while ((line = Console.ReadLine()) != null)
            //{
            //    Bus.Send("EventStorePerfQueue", new NServiceBusEventMessage());
            //}
        }

        static void RunInProcPerfTests(int iterations, int threadCount)
        {
            //Execute Scenarios for each Implementation
            var s = new NewEventSourceSmallEvent();
            s.SetupUnitTest(iterations, 1, threadCount, 0, 0, 1);
            s.RunAsUnitTest();
        }

        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            var sample = EventCount;
            var ops = ((sample - lastCount) * 1000L) / timerInterval;
            
            timerElapsedCount++;

            Console.WriteLine("{0} : Ops/s: {1}, Total Ops: {2}, Time taken: {3:# ###.##}", "Events Dispatched", ops, sample, (timerElapsedCount * timerInterval) / 1000);

            if (ops == 0)
                ((System.Timers.Timer)sender).Stop();

            lastCount = sample;
            
        }
              
        private static void SetUpNSB()
        {
            NServiceBus.SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);

            Bus = Configure.With()
               .DefaultBuilder()
              //.Log4Net()
              .BinarySerializer()
              .MsmqTransport()
              .PurgeOnStartup(true)
              .UnicastBus()
              .LoadMessageHandlers()
              .CreateBus()
              .Start();
          
          
        }
    }
}
