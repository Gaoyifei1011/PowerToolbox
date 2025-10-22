using Microsoft.UI.Dispatching;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace PowerToolbox.Extensions.Threading
{
    public class DispatcherQueueSynchronizationContext(DispatcherQueue dispatcherQueue) : SynchronizationContext
    {
        private readonly DispatcherQueue dispatcherQueue = dispatcherQueue;

        public override void Post(SendOrPostCallback callback, object state)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            dispatcherQueue.TryEnqueue(() =>
            {
                callback(state);
            });
        }

        public override void Send(SendOrPostCallback callback, object state)
        {
            if (dispatcherQueue.HasThreadAccess)
            {
                callback(state);
            }
            else
            {
                ManualResetEvent manualResetEvent = new(false);
                ExceptionDispatchInfo exceptionDispatchInfo = null;

                dispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        callback(state);
                    }
                    catch (Exception ex)
                    {
                        exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    }
                    finally
                    {
                        manualResetEvent.Set();
                    }
                });
                manualResetEvent.WaitOne();
                exceptionDispatchInfo?.Throw();
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return new DispatcherQueueSynchronizationContext(dispatcherQueue);
        }
    }
}
