using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace EventStore.TestScenarios
{
    public class EventCreator<T> where T : new()
    {
        public T CreateSingleEvent() 
        {
            return new T();
        }

        public List<T> CreateEvents(int eventCount)
        {
            var events = new List<T>();

            for (int i = 0; i < eventCount; i++)
            {
                events.Add(new T());
            }
            return events;
        }
    }

    [Serializable]
    public class SmallEvent : IMessage
    {
        public Guid Id { get; set; }
        public string SomeProperty { get; set; }

        public SmallEvent()
        {
            Id = Guid.NewGuid();
            SomeProperty = "I am a Small Event";
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Id.ToString(), SomeProperty);
        }
    }

        [Serializable]
    public class ModerateEvent : IMessage
    {
        public Guid Id { get; set; }
        public string SomeProperty { get; set; }
        public int SomeIntProperty { get; set; }
        public string SomeOtherProperty { get; set; }
        public string ANOtherProperty { get; set; }
        public string BlaaaaProperty { get; set; }

        public ModerateEvent()
        {
            Id = Guid.NewGuid();
            SomeProperty = "I am a Moderate Event";
            SomeIntProperty = new Random().Next();
            SomeOtherProperty = "huge range of different cover for different needs. More than one car in the household? Maybe you need car insurance cover for an additional driver or have a young ";
            ANOtherProperty = "car insurance, we constantly compare the best rates and deals from pet insurance and travel insurance to public liability insurance, credit cards, loans and utilities so you can get on with your busy life We compare prices from hundreds";
            BlaaaaProperty = "574754545dfgddtrdrd65ed";
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} : {2}", Id.ToString(), SomeProperty, SomeIntProperty);
        }
    }

    public class SmallAggregateMemento : SmallEvent
    {
      
    }

     public class ModerateAggregateMemento : ModerateEvent
    {
      
    }
}

