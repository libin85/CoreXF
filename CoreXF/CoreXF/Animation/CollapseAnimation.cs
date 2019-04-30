
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace CoreXF
{
    public class CollapseAnimation : BehaviorBase
    {
        double defaultHeight = -1;
        bool stopMeasuring;

        #region Properties

        #endregion

        public CollapseAnimation()
        {
            TwoStateAnimation = true;
        }

        protected override void OnAttachedTo(View view)
        {
            base.OnAttachedTo(view);
            view.MeasureInvalidated += View_MeasureInvalidated;
        }

        private void View_MeasureInvalidated(object sender, EventArgs e)
        {
            if (stopMeasuring)
                return;

            mesure();
        }

        void mesure()
        {
            var size = AssociatedView.Measure(double.PositiveInfinity, double.PositiveInfinity);
            var calcHeight = Math.Max(size.Request.Height, size.Minimum.Height);
            if(calcHeight > defaultHeight)
            {
                defaultHeight = calcHeight;
            }
        }

        protected override void OnDetachingFrom(View view)
        {
            base.OnDetachingFrom(view);
            view.MeasureInvalidated -= View_MeasureInvalidated;
        }

        bool _internalChanges;
        protected override bool InitialSetup()
        {
            if (AssociatedView == null)
                return false;

            // If the view have to be hidden by default
            if (!CurrentState)
            {
                AssociatedView.IsVisible = false;
            }

            // If the View isn't initialized yet
            if (defaultHeight < 1)
            {
                return false;
            }

            _internalChanges = true;
            if (!CurrentState)
            {
                AssociatedView.HeightRequest = 0;
            }
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

            stopMeasuring = true;

            if (CurrentState)
            {
                AssociatedView.IsVisible = true;
            }
            
            var an = new Animation(x =>
            {
                AssociatedView.HeightRequest = x;
            }, CurrentState ? 0 : defaultHeight, CurrentState ? defaultHeight : 0, Easing, null);

            an.Commit(AssociatedView, Guid.NewGuid().ToString(), length: Length, easing: Easing,
                finished: (a, b) =>
                {
                    if (!CurrentState)
                    {
                        AssociatedView.IsVisible = false;
                    }
                });
        }

    }
}
