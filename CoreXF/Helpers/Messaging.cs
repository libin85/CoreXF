
using Acr.UserDialogs;
using Plugin.Messaging;
using Splat;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{
    public class Messaging
    {

        static public void SendEmailToSupport(string Email,string Subject = null)
        {
            Messaging.SendEmail(
                Email,
                Subject == null ? "[MobileApp]" : Subject, 
                $"\n\n\n App ver {DeviceInfo.AppVersion} {AppConfig.ConfigPrefix} {DeviceInfo.OsBuild} {DeviceInfo.OsVersion} {CoreUserSettings.LastLogin} {DeviceInfo.Manufacturer} {DeviceInfo.DeviceName}"
            );
        }

        static public void SendEmail(string Email, string Subject = null,string Message = null)
        {
            IUserDialogs _dialogs = Locator.CurrentMutable.GetService<IUserDialogs>();

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
                emailMessenger.SendEmail(Email, Subject, Message);
            }
            catch (Exception ex)
            {
                ExceptionManager.SendErrorAndShowExceptionDialog(ex, Tx.T("CoreXF_messaging_cannotsendemail"));
            }

        }
    }
}
