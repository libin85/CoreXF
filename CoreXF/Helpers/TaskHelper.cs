using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreXF.Helpers
{
    public static class TaskHelper
    {
        public static void RunSequential(Action onComplete, Action<Exception> errorHandler,
                          params Func<Task>[] actions)
        {
            RunSequential(onComplete, errorHandler, actions.AsEnumerable().GetEnumerator());
        }

        public static void RunSequential(Action onComplete, Action<Exception> errorHandler,
                                  IEnumerator<Func<Task>> actions)
        {
            if (!actions.MoveNext())
            {
                onComplete?.Invoke();
                return;
            }

            if (actions.Current != null)
            {
                var task = actions.Current();

                task.ContinueWith(t => errorHandler?.Invoke(t.Exception),
                    TaskContinuationOptions.OnlyOnFaulted);

                task.ContinueWith(t => RunSequential(onComplete, errorHandler, actions),
                    TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }
    }
}
