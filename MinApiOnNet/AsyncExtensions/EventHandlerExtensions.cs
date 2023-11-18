using System.Reflection;
using System.Runtime.CompilerServices;

namespace MinApiOnNet.AsyncExtensions;

public static class EventHandlerExtensions
{
    public static Task InvokeAsync(this EventHandler? @this, object sender, EventArgs eventArgs)
    {
        // if no event handlers attached complete immediately
        if (@this == null)
        {
            return Task.CompletedTask;
        }

        var delegates = @this.GetInvocationList();
        var count = @this.GetInvocationList().Length;
        var tcs = new TaskCompletionSource<bool>();
        var exception = (Exception)null;

        foreach (var @delegate in delegates)
        {
            var isAsync = @delegate.Method
                .GetCustomAttributes(typeof(AsyncStateMachineAttribute), false)
                .Any();

            var completed = new Action(() =>
            {
                if (Interlocked.Decrement(ref count) == 0)
                {
                    if (exception is null)
                    {
                        tcs.SetResult(true);
                    }
                    else
                    {
                        tcs.SetException(exception);
                    }
                }
            });

            var failed = new Action<Exception>(e =>
                Interlocked.CompareExchange(ref exception, e, null));

            if (isAsync)
            {
                SynchronizationContext.SetSynchronizationContext(new EventHandlerSynchronizationContext(completed, failed));
            }

            try
            {
                @delegate.DynamicInvoke(sender, eventArgs);
            }
            catch (TargetInvocationException e)
                when (e.InnerException != null)
            {
                failed(e.InnerException);
            }
            catch (Exception e)
            {
                failed(e);
            }

            if (!isAsync)
            {
                completed();
            }
        }
        return tcs.Task;
    }
}