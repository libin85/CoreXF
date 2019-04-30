
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace CoreXF
{
    public abstract class StepRotateAnimation : StepAnimation
    {
        public StepRotateAnimation() { }

        public override void OnPaintSurfaceBeforeContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
            e.Surface.Canvas.RotateDegrees(CurrentValue, e.Info.Width / 2, e.Info.Height / 2);
        }
    }
}
