
using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using Splat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{

    public static class ExceptionManager
    {
        static IUserDialogs userDialogs;

        // for deduplication
        static ConcurrentDictionary<string, Exception> _exceptions = new ConcurrentDictionary<string, Exception>();

        static bool IsIgnoredException(Exception exception)
        {
            if (exception == null)
                return true;

            if (exception.Message.Contains("A task was canceled")
                || exception.Message.Contains("NoContentException")
                )
            {
                return true;
            }

            return false;
        }


        public static void SendErrorAndShowExceptionDialog(Exception exception, string message = null, string parameters = null)
        {
            processSendError(exception, message, parameters);
            if(userDialogs == null)
            {
                userDialogs = Locator.CurrentMutable.GetService<IUserDialogs>();
            }
            userDialogs.Alert(message);
        }

        //public static void SendError(Exception exception, string message = null, string parameters = null, bool deduplicate = false)

        #region Deduplicate

        

        #endregion

        public static void SendError(Exception exception, string message = null, string parameters = null, bool deduplicate = false)
        {
            if (exception == null)
                return;

            if (deduplicate)
            {
                bool contains = false;
                _exceptions.AddOrUpdate(exception.Message, exception, updateValueFactory: (key, value) =>
                {
                    contains = true;
                    //Debug.Write("11 UPDATE!!");
                    return value;
                });


                if (!contains)
                {
                    //Debug.Write("11 ADD!!");
                    Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                    {
                        ExceptionManager.processSendError(exception, "AsyncErrorHandler");
                        _exceptions.TryRemove(exception.Message,out Exception ex);
                        return false;
                    });
                }
            }
            else
            {
                processSendError(exception, message, parameters);
            }
        }

        static void processSendError(Exception exception,string message = null, string parameters = null,bool deduplicate = false)
        {

            if (IsIgnoredException(exception))
                return;

            Debug.WriteLine("EXCEPTION MANAGER >>>");
            Debug.WriteLineIf(!string.IsNullOrEmpty(message), $"message: {message}");
            Debug.WriteLine(exception);

            var dict = new Dictionary<string, string>();

            dict.Add("Caught", "True");

            // parameters
            if (!string.IsNullOrEmpty(parameters))
            {
                var list = parameters.Split(';');
                foreach (var elm in list)
                {
                    int poz = elm.IndexOf('=');
                    if (poz > 0)
                    {
                        dict.Add(elm.Substring(0, poz - 1), elm.Substring(poz + 1));
                    }
                    else
                    {
                        dict.Add(elm, "");
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                dict.Add("Message", message);
            }
#if RELEASE
            Crashes.TrackError(exception, dict);
#endif
        }
    }

    /*

    public class ExceptionHelper
    {

        public static Exception getDetailedExeption(Exception exception)
        {
            Exception ex;

            if (exception is AggregateException)
            {
                AggregateException aex = exception as AggregateException;
                if (aex.InnerExceptions.Count > 0)
                {
                    ex = aex.InnerExceptions[0];
                }
                else
                {
                    ex = exception;
                }
            }
            else
            {
                ex = exception;
            }
            return ex;
        }

        static async Task<bool> CheckException(Exception ex, IUserDialogs userdialogs)
        {
            if (ex == null)
                return false;

            switch (ex)
            {
                case OperationCanceledException tcEx1:
                case NoContentException nce:
                    return true;

                case HttpRequestException httpEx:
                    if (userdialogs != null)
                    {
                        await userdialogs.AlertAsync(Tx.T("CommonMessages_CheckInet"));
                    }
                    return true;

                case HttpStatusCodeException hsce:
                    if (hsce.Code == System.Net.HttpStatusCode.InternalServerError && userdialogs != null)
                    {
                        await userdialogs.AlertAsync(Tx.T("CommonMessages_Internet_ServerError"));
                        return true;
                    }
                    break;

            }


            string stype = ex?.GetType()?.ToString();
            if (stype == null)
                return false;

            switch (stype)
            {
                case "Java.Net.UnknownHostException":
                case "System.Net.Http.HttpRequestException":
                case "android.system.GaiException":
                    if (userdialogs != null)
                    {
                        await userdialogs.AlertAsync(Tx.T("CommonMessages_CheckInet"));
                    }
                    return true;
            }

            if (stype.Contains("NoContentException"))
            {
                return true;
            }

            return false;
        }

        
        public static void ExceptionProcessingSilent( 
            Exception exception,
            EventType type)
        {
            ExceptionProcessing(exception, "", silent: true).ConfigureAwait(false);
        }

        public static async Task<bool> ExceptionProcessingWithDialog(
            Exception exception,
            EventType type)
        {
            return false;
        }


        public static async Task<bool> ExceptionProcessing(
            Exception exception, 
            string UnexpectedErrorMessage, 
            bool silent, 
            IUserDialogs userdialogs = null,
            bool doNotShowUknownErrorDialog = false)
        {
            Exception ex = getDetailedExeption(exception);
            if (ex == null)
            {
                Debug.WriteLine("ExceptionHelper : empty exception");
                return false;
            }
            if (ex.Message == null)
            {
                return true;
            }

            Debug.WriteLine("ExceptionHelper");
            Debug.WriteLine(ex);

            bool isProcessed = await CheckException(ex,userdialogs);
            if (isProcessed)
                return true;

            // неожиданная ошибка, нужно выслать уведомление
            Device.BeginInvokeOnMainThread(async () => await SendError(ex, UnexpectedErrorMessage, silent, doNotShowUknownErrorDialog));

            return false;

        }

        static async Task SendError(Exception ex, string UnexpectedErrorMessage, bool silent, bool doNotShowUknownErrorDialog)
        {
            if(ex == null)
            {
                return;
            }

            CaughtExceptionModel exModel = new CaughtExceptionModel
            {
                Id = Guid.NewGuid().ToString(),
                Message = UnexpectedErrorMessage,
                Body = ex.ToString(),
                DateTime = DateTimeOffset.Now,
            };

            CaughtExceptionModel visModel = exModel.Clone();

            if(SystemDB.Connection != null)
            {
                SystemDB.Connection.Insert(exModel);
            }
            else
            {
                await HockeyAppExtension.SendHandledException(visModel);
                return;
            }

            await HockeyAppExtension.Sync();

            if (!silent && !doNotShowUknownErrorDialog)
                CoreApp.NavigationService.ShowExceptionDialog(visModel);

        }
    }

    */
}
