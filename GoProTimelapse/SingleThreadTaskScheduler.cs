using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoProTimelapse
{
    public sealed class SingleThreadTaskScheduler
        : TaskScheduler, IDisposable
    {
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly BlockingCollection<Task> _tasks;
        private readonly Action _initAction;

        public SingleThreadTaskScheduler()
            : this(null)
        {
            
        }

        public SingleThreadTaskScheduler(Action initAction, ApartmentState apartmentState = ApartmentState.STA)
        {
            this._cancellationToken = new CancellationTokenSource();
            this._tasks = new BlockingCollection<Task>();
            this._initAction = initAction ?? (() => { });

            this._thread = new Thread(this.ThreadStart);
            this._thread.IsBackground = true;
            this._thread.TrySetApartmentState(apartmentState);
            this._thread.Start();
        }

        protected override void QueueTask(Task task)
        {
            this._tasks.Add(task, this._cancellationToken.Token);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            this._cancellationToken.Token.ThrowIfCancellationRequested();

            if (this._thread != Thread.CurrentThread)
                return false;

            base.TryExecuteTask(task);
            return true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return this._tasks.ToArray();
        }

        private void ThreadStart()
        {
            try
            {
                var token = this._cancellationToken.Token;

                this._initAction();

                foreach (var task in this._tasks.GetConsumingEnumerable(token))
                    base.TryExecuteTask(task);
            }
            finally
            {
                this._tasks.Dispose();
            }
        }

        public void Dispose()
        {
            this._tasks.CompleteAdding();
            this._cancellationToken.Cancel();
        }
    }
}