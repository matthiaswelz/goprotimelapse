using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoProTimelapse
{
    public static class TaskFactoryExtensions
    {
        public static Task<T> StartNew<T>(this TaskFactory factory, Func<T> func, TaskScheduler scheduler)
        {
            return factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }
    }
}