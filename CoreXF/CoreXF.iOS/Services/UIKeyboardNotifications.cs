using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreXF;
using Foundation;
using UIKit;
using Xamarin.Forms;

// https://forums.xamarin.com/discussion/12715/how-to-get-size-of-the-keyboard


[assembly: Dependency(typeof(CoreXF.iOS.UIKeyboardNotifications))]
namespace CoreXF.iOS
{
    public class UIKeyboardNotifications : IKeyboardNotifications, IDisposable
    {
        //public event EventHandler KeyboardShowing;

        public Action<double> OnKeyboardNotification { get; set; }

        NSObject notificationWillShow;
        NSObject notificationWillHide;
        private bool isListening;

        [Preserve]
        public static void Init() { }

        //Just in case
        ~UIKeyboardNotifications()
        {
            Dispose();
        }

        void KeyboardWillShow(object sender, UIKeyboardEventArgs args)
        {

            Console.WriteLine("----------------------");
            //Console.WriteLine("Notification: {0}", args.Notification);
            Console.WriteLine("FrameBegin" + args.FrameBegin);
            Console.WriteLine("FrameEnd" + args.FrameEnd);
            //Console.WriteLine("AnimationDuration" + args.AnimationDuration);
            //Console.WriteLine("AnimationCurve" + args.AnimationCurve);


            // Access strongly typed args
            //Console.WriteLine("Notification: {0}", args.Notification);
            //Console.WriteLine("FrameBegin", args.FrameBegin);
            double height = Math.Max(args.FrameBegin.Height,args.FrameEnd.Height);

            OnKeyboardNotification?.Invoke(height);
        }

        void keyboardWillHide(object sender, UIKeyboardEventArgs args)
        {
            Console.WriteLine("----------------------");
            Console.WriteLine(">>>>> keyboardWillHide");

            OnKeyboardNotification?.Invoke(0d);
        }

        public void StartListening()
        {
            if (!isListening)
            {
                //// listening
                //notification = UIKeyboard.Notifications.ObserveWillShow((sender, args) => {
                //    /* Access strongly typed args */
                //    Console.WriteLine("Notification: {0}", args.Notification);
                //    Console.WriteLine("FrameBegin", args.FrameBegin);
                //    Console.WriteLine("FrameEnd", args.FrameEnd);
                //    Console.WriteLine("AnimationDuration", args.AnimationDuration);
                //    Console.WriteLine("AnimationCurve", args.AnimationCurve);
                //});
                notificationWillShow = UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShow);
                notificationWillHide = UIKeyboard.Notifications.ObserveWillHide(keyboardWillHide);
                
                isListening = true;
            }
        }
        void Teardown()
        {
            Dispose();
        }
        public void StopListening()
        {
            if (isListening)
            {
                //isListening = false;
                //notificationWillShow.Dispose();// To stop listening:
            }
        }

        public void Dispose()
        {
            StopListening();
            //OnKeyboardNotification = null;
        }
    }
}
