
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace CoreXF
{
    public class RotateAnimation : BehaviorBase
    {

        public static readonly BindableProperty StartRotationProperty = BindableProperty.Create(nameof(StartRotation), typeof(double), typeof(RotateAnimation), default(double));
        public static readonly BindableProperty EndRotationProperty = BindableProperty.Create(nameof(EndRotation), typeof(double), typeof(RotateAnimation), 180d);

        #region Properties

        public double EndRotation
        {
            get { return (double)GetValue(EndRotationProperty); }
            set { SetValue(EndRotationProperty, value); }
        }

        public double StartRotation
        {
            get { return (double)GetValue(StartRotationProperty); }
            set { SetValue(StartRotationProperty, value); }
        }

        #endregion

        public RotateAnimation()
        {
            TwoStateAnimation = true;
        }

        bool _internalChanges;
        protected override bool InitialSetup()
        {
            if (AssociatedView == null)
                return false;

            _internalChanges = true;

            AssociatedView.Rotation = CurrentState ? EndRotation : StartRotation;

             _internalChanges = false;

            return true;
        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (_internalChanges)
                return;

            switch (propertyName)
            {
                case nameof(CurrentState):
                    DoAnimation();
                    break;
            }
        }

        void DoAnimation()
        {
            if (AssociatedView == null)
                return;

            var an = new Animation(x =>
            {
                AssociatedView.Rotation = x;
            }, CurrentState ? StartRotation : EndRotation, CurrentState ? EndRotation : StartRotation, Easing, null);

            an.Commit(AssociatedView, Guid.NewGuid().ToString(), length: Length, easing: Easing);

        }
    }
}
