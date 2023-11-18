using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace MinApiOnNet.AsyncExtensions;

public class EventHandlerSynchronizationContext : SynchronizationContext
{
    private readonly Action _completed;
    private readonly Action<Exception> _failed;

    public EventHandlerSynchronizationContext (Action completed, Action<Exception> failed)
    {
        _completed = completed;
        _failed = failed;
    }

    public override void Post(SendOrPostCallback d, object state)
    {
        if (state is ExceptionDispatchInfo edi)
        {
            Debug.WriteLine("Capturing Exception in Post");
            _failed(edi.SourceException);
        }
        else
        {
            Debug.WriteLine("Posting");
            base.Post(d, state);
        }
    }

    public override void Send(SendOrPostCallback d, object state)
    {
        if (state is ExceptionDispatchInfo edi)
        {
            Debug.WriteLine("Capturing Exception in Send");
            _failed(edi.SourceException);
        }
        else
        {
            Debug.WriteLine("Sending");
            base.Send(d, state);
        }
    }

    public override SynchronizationContext CreateCopy()
    {
        return new EventHandlerSynchronizationContext (_completed, _failed);
    }

    public override void OperationStarted()
        => Debug.WriteLine("SynchronizationContext: Started");

    public override void OperationCompleted()
    {
        Debug.WriteLine("SynchronizationContext: Completed");
        _completed();
    }
}