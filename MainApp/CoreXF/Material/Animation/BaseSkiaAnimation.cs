
using System;
using Xamarin.Forms;

namespace CoreXF
{
    public abstract class BaseSkiaAnimation : AbstractMaterialComponent
    {
        public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(BaseSkiaAnimation), default(bool));
        public static readonly BindableProperty IsInfinityProperty = BindableProperty.Create(nameof(IsInfinity), typeof(bool), typeof(BaseSkiaAnimation), default(bool));
        public static readonly BindableProperty StartValueProperty = BindableProperty.Create(nameof(StartValue), typeof(double), typeof(BaseSkiaAnimation), default(double));
        public static readonly BindableProperty EndValueProperty = BindableProperty.Create(nameof(EndValue), typeof(double), typeof(BaseSkiaAnimation), default(double));


        #region Properties

        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        public bool IsInfinity
        {
            get { return (bool)GetValue(IsInfinityProperty); }
            set { SetValue(IsInfinityProperty, value); }
        }

        #endregion

    }
}
