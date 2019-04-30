
using FFImageLoading.Helpers;
using System;

namespace CoreXF
{
    public class NoContentException : Exception
    {
    }

    public class FFImageCustomLogger : IMiniLogger
    {
        public void Debug(string message)
        {
            //System.Diagnostics.Debug.WriteLine($"FF: debug : {message}");
        }

        public void Error(string errorMessage)
        {
            //System.Diagnostics.Debug.WriteLine($"FF: error : {errorMessage}"); 
        }

        public void Error(string errorMessage, Exception ex)
        {
            //System.Diagnostics.Debug.WriteLine($"FF: exc : {errorMessage}");

            /*
            if (
                ex is NoContentException || 
                ((AggregateException)ex).InnerExceptions.Any(
                    x => x is NoContentException || 
                    x is HttpRequestException) 
                )
            {
                // no photo
                return;
            }

            System.Diagnostics.Debug.WriteLine($"FF exc : {errorMessage}");
            ExceptionHelper.ExceptionProcessing(ex, "FFImageCustomLogger", silent: true);
            */
        }
    }

}
