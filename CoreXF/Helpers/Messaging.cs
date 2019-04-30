
using Acr.UserDialogs;
using Plugin.Messaging;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{
    public class Messaging
    {

       static public async Task SendEmail(string email,IUserDialogs _dialogs, string subject = null,string message = null)
        {
            var emailMessenger = CrossMessaging.Current.EmailMessenger;

            if (!emailMessenger.CanSendEmail)
            {
                string msg = null;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    msg = Tx.T("CoreXF_messaging_cannotcreateemail_ios");
                }
                else
                {
                    msg = Tx.T("CoreXF_messaging_cannotcreateemail");
                }
                _dialogs.Alert(msg);
                return;
            }

            try
            {
                emailMessenger.SendEmail(email,subject,message);
            }
            catch (Exception ex)
            {
                ExceptionManager.SendErrorAndShowExceptionDialog(ex, Tx.T("CoreXF_messaging_cannotsendemail"));
            }

        }
    }
}
