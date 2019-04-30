
using System;

namespace CoreXF
{
    public interface IKeyboardNotifications : IDisposable
    {
        Action<double> OnKeyboardNotification { get; set; }
        void StartListening();
        void StopListening();
    }
}
