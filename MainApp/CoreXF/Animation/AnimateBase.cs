
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace CoreXF
{
    public abstract class BehaviorBase : Behavior<View> 
    {
        public View AssociatedView { get; private set; }

        public static readonly BindableProperty LengthProperty = BindableProperty.Create(nameof(Length), typeof(uint), typeof(BehaviorBase), 250U);
        public static readonly BindableProperty EasingProperty = BindableProperty.Create(nameof(Easing), typeof(Easing), typeof(BehaviorBase), Easing.Linear);

        public static readonly BindableProperty TwoStateAnimationProperty = BindableProperty.Create(nameof(TwoStateAnimation), typeof(bool), typeof(BehaviorBase), default(bool));
        public static readonly BindableProperty CurrentStateProperty = BindableProperty.Create(nameof(CurrentState), typeof(bool), typeof(BehaviorBase), default(bool));

        #region Properties

        public bool CurrentState
        {
            get { return (bool)GetValue(CurrentStateProperty); }
            set { SetValue(CurrentStateProperty, value); }
        }

        public bool TwoStateAnimation
        {
            get { return (bool)GetValue(TwoStateAnimationProperty); }
            set { SetValue(TwoStateAnimationProperty, value); }
        }


        public Easing Easing
        {
            get { return (Easing)GetValue(EasingProperty); }
            set { SetValue(EasingProperty, value); }
        }

        public uint Length
        {
            get { return (uint)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        #endregion

        protected override void OnAttachedTo(View view)
        {
            base.OnAttachedTo(view);
            AssociatedView = view;
            view.BindingContextChanged += OnBindingContextChanged;
        }

        protected override void OnDetachingFrom(View view)
        {
            base.OnDetachingFrom(view);
            view.BindingContextChanged -= OnBindingContextChanged;
        }

        bool _isInitialized;
        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedView.BindingContext;

            if (!_isInitialized && AssociatedView != null)
            {
                _isInitialized = InitialSetup();
            }
        }

        protected virtual bool InitialSetup() => true;


    }

}
