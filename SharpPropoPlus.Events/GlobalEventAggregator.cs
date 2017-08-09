using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharpPropoPlus.Events.Events;

namespace SharpPropoPlus.Events
{
  public class GlobalEventAggregator: EventAggregator
  {
    private static volatile GlobalEventAggregator _instance;
    private static readonly object Sync = new object();

    public static GlobalEventAggregator Instance
    {
      get
      {
        if (_instance != null)
          return _instance;

        lock (Sync)
        {
          if (_instance == null)
            _instance = new GlobalEventAggregator();
        }

        return _instance;
      }
    }

    private GlobalEventAggregator()
    {

      //var eventAggregationManager = new EventAggregator();

      //eventAggregationManager.AddListener(new BasicListener());
      ////eventAggregationManager.AddListener<SampleEventMessage>(Handle);


      //eventAggregationManager.SendMessage(new SampleEventMessage(DateTime.Now, "Message"));

    }

    //public void Handle(SampleEventMessage message)
    //{
    //  //"BasicHandler - Received event".Log();
    //}


    public class MessagePublisher
    {
      private readonly IEventPublisher _eventPublisher;

      public MessagePublisher(IEventPublisher eventPublisher)
      {
        _eventPublisher = eventPublisher;
      }

      public void DoSomeWork()
      {
        _eventPublisher.SendMessage<SampleEventMessage>();
      }
    }


    public class BasicListener : IListener<SampleEventMessage>
    {
      public void Handle(SampleEventMessage message)
      {
        //"BasicHandler - Received event".Log();
      }
    }

    public class SampleEventMessage
    {
      public SampleEventMessage()
        : this(DateTime.Now, string.Empty)
      {
      }

      public SampleEventMessage(DateTime eventPublished, string message)
      {
        EventPublished = eventPublished;
        Message = message;
      }

      public DateTime EventPublished { get; set; }
      public string Message { get; set; }
    }
  }
}
