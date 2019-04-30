using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{
    public static class DeviceHelper
    {

        public static Task<T> RunOnMainThreadAsync<T>(Func<T> func) where T : class
        {
            var tcs = new TaskCompletionSource<T>();
            Device.BeginInvokeOnMainThread(
                () =>
                {
                    try
                    {
                        T res = func();
                        tcs.SetResult(res);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }


        public static Task RunOnMainThreadAsync(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            Device.BeginInvokeOnMainThread(
                () =>
                {
                    try
                    {
                        action();
                        tcs.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }

        public static Task RunOnMainThreadAsync(Task action)
        {
            var tcs = new TaskCompletionSource<object>();
            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    try
                    {
                        await action;
                        tcs.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }
    }
}
