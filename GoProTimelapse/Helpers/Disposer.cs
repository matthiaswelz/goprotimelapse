using System;

namespace GoProTimelapse.Helpers
{
    internal sealed class Disposer 
        : IDisposable
    {
        private readonly Action _action;

        public Disposer(Action action)
        {
            this._action = action;
        }

        public void Dispose()
        {
            this._action();
        }
    }
}