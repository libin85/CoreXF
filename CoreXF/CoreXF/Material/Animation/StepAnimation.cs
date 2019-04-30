
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace CoreXF
{
    public abstract class StepAnimation : BaseSkiaAnimation, IDisposableRegistration
    {
        public static readonly BindableProperty StepsProperty = BindableProperty.Create(nameof(Steps), typeof(int), typeof(StepAnimation), default(int));
        public static readonly BindableProperty DelayProperty = BindableProperty.Create(nameof(Delay), typeof(int), typeof(StepAnimation), default(int));

        #region Properties

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public int Steps
        {
            get { return (int)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        #endregion

        protected int CurrentStep;
        protected float CurrentValue;

        public bool NeedToRegisterForDispose { get; set; }
        public bool RegisteredForDispose { get; set; }

        public override void OnSetParent()
        {
            NeedToRegisterForDispose = true;

            startAnimate();
        }

        void SetCurrentStep(int step)
        {
            CurrentStep = step;
            CurrentValue = (float)(StartValue + (EndValue - StartValue) * CurrentStep / Steps);
            SKParentView?.InvalidateSurface();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Enabled):
                    startAnimate();
                    break;
            }
        }

        void startAnimate()
        {
            if (!Enabled || Delay == 0)
                return;

            CurrentStep = 0;
            Device.StartTimer(TimeSpan.FromMilliseconds(Delay), ()=> aniFunc(Delay));
        }

        bool aniFunc(int delay)
        {
            if (!Enabled)
                return false;
            
            if(delay != Delay)
            {
                startAnimate();
                return false;
            }

            if (CurrentStep + 1 == Steps)
            {
                if (IsInfinity)
                {
                    SetCurrentStep(0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            SetCurrentStep(CurrentStep + 1);
            return true;
        }

        public void Dispose()
        {
            Enabled = false;
        }
    }
}
