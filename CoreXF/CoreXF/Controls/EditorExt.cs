
using Xamarin.Forms;
using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CoreXF
{
    public class EditorExt : Editor, IDisposable
    {
        public static readonly BindableProperty CompletedCommandProperty = BindableProperty.Create(nameof(CompletedCommand), typeof(ICommand), typeof(EditorExt), default(ICommand));
        public static readonly BindableProperty EnableHeightHackProperty = BindableProperty.Create(nameof(EnableHeightHack), typeof(bool), typeof(EditorExt), default(bool));
        public static readonly BindableProperty IsBorderlessProperty = BindableProperty.Create(nameof(IsBorderless), typeof(bool), typeof(EditorExt), default(bool));
        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(EditorExt), default(ReturnType));

        #region Properties

        public ReturnType ReturnType
        {
            get { return (ReturnType)GetValue(ReturnTypeProperty); }
            set { SetValue(ReturnTypeProperty, value); }
        }

        public bool IsBorderless
        {
            get { return (bool)GetValue(IsBorderlessProperty); }
            set { SetValue(IsBorderlessProperty, value); }
        }

        public bool EnableHeightHack
        {
            get { return (bool)GetValue(EnableHeightHackProperty); }
            set { SetValue(EnableHeightHackProperty, value); }
        }

        public ICommand CompletedCommand
        {
            get { return (ICommand)GetValue(CompletedCommandProperty); }
            set { SetValue(CompletedCommandProperty, value); }
        }

        #endregion

        public EditorExt()
        {
            this.Completed += EditorExt_Completed;
            this.TextChanged += EditorExt_TextChanged;
        }

        private void EditorExt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EnableHeightHack)
            {
                // GetSizeRequest
                SizeRequest size = OnMeasure(this.Width, double.PositiveInfinity);
                //var size = this.GetSizeRequest(this.Width,)
                //var size = ed.Measure(this.Width, double.PositiveInfinity);
                HeightRequest = size.Request.Height;
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            SizeRequest measured = base.OnMeasure(widthConstraint, heightConstraint);
            //Debug.WriteLine($"MTM OnMeasure minimum {measured.Minimum} request {measured.Request}");

            if (MinimumHeightRequest > 0 && measured.Minimum.Height < MinimumHeightRequest)
            {
                measured.Minimum = new Size(measured.Minimum.Width, MinimumHeightRequest);
                measured.Request = new Size(measured.Request.Width, MinimumHeightRequest);
            }

            return measured;
        }

        private void EditorExt_Completed(object sender, EventArgs e)
        {
            CompletedCommand?.Execute(Text);
        }

        public void Dispose()
        {
            this.Completed -= EditorExt_Completed;
            this.TextChanged -= EditorExt_TextChanged;
        }
    }
}
