
using Android.Content;
using Android.Util;

namespace CoreXF.Droid
{
    public static class IntentExtension
    {
        public static void PrintExtras(this Intent intent,string info)
        {
            System.Diagnostics.Debug.WriteLine("Extras! "+info);
            Log.Info("Extras!", $"Extras! {info}");

            if (intent.Extras != null)
            {
                string msg = $"Extras! {info}";

                foreach (var key in intent.Extras.KeySet())
                {
                    var value = intent.Extras.GetString(key);
                    msg += $"  Key: {key} Value: {value}";
                }
                System.Diagnostics.Debug.WriteLine(msg);
                Log.Info("Extras!", $"Extras! {info}");
                Log.Info("Extras!",msg);

            }

        }

    }
}