using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CoreXF.Material;

namespace CoreXF
{
    public class GradientComponent : AbstractMaterialComponent, IBindableGradient
    {

        public static readonly BindableProperty GradientStartColorProperty = BindableProperty.Create(nameof(GradientStartColor), typeof(Color), typeof(GradientComponent), default(Color));
        public static readonly BindableProperty GradientEndColorProperty = BindableProperty.Create(nameof(GradientEndColor), typeof(Color), typeof(GradientComponent), default(Color));
        public static readonly BindableProperty GradientOrientationProperty = BindableProperty.Create(nameof(GradientOrientation), typeof(GradientOrientationEnum), typeof(GradientComponent), GradientOrientationEnum.Vertical);

        #region Properties

        public GradientOrientationEnum GradientOrientation
        {
            get { return (GradientOrientationEnum)GetValue(GradientOrientationProperty); }
            set { SetValue(GradientOrientationProperty, value); }
        }

        public Color GradientEndColor
        {
            get { return (Color)GetValue(GradientEndColorProperty); }
            set { SetValue(GradientEndColorProperty, value); }
        }

        public Color GradientStartColor
        {
            get { return (Color)GetValue(GradientStartColorProperty); }
            set { SetValue(GradientStartColorProperty, value); }
        }

        #endregion

        public override void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
            Material.DrawGradient(e.Surface, targetRect, this);
        }
    }
}
