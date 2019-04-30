
using SkiaSharp;
using System;

namespace CoreXF
{
    public enum DisposeMode { NoDispose, Dispose }

    public class MaterialRawImage : IDisposableRegistration
    {

        public bool HasImage => SkPicture != null || SkBitmap != null;

        public bool NeedToRegisterForDispose { get; private set; }

        public bool RegisteredForDispose { get; set; }

        DisposeMode _disposeMode = DisposeMode.Dispose;

        public SKPicture SkPicture { get; private set; }

        public void Add(SKPicture picture, DisposeMode disposeMode)
        {
            this._disposeMode = disposeMode;
            DisposePreviousResources();
            SkPicture = picture;
            SetNeedToRegisterForDispose(disposeMode);
        }

        public SKBitmap SkBitmap { get; private set; }

        public void Add(SKBitmap bitmap, DisposeMode disposeMode)
        {
            this._disposeMode = disposeMode;
            DisposePreviousResources();
            SkBitmap = bitmap;
            SetNeedToRegisterForDispose(disposeMode);
        }

        public void Reset()
        {
            DisposePreviousResources();
            SetNeedToRegisterForDispose(_disposeMode);
        }

        void SetNeedToRegisterForDispose(DisposeMode disposeMode)
        {
            if (RegisteredForDispose || NeedToRegisterForDispose)
                return;

            if (HasImage && disposeMode == DisposeMode.Dispose)
                NeedToRegisterForDispose = true;
        }

        void DisposePreviousResources()
        {
            if (SkPicture != null && _disposeMode == DisposeMode.Dispose)
            {
                SkPicture.Dispose();
                SkPicture = null;
            }

            if (SkBitmap != null && _disposeMode == DisposeMode.Dispose)
            {
                SkBitmap.Dispose();
                SkBitmap = null;
            }
        }

        public void Dispose()
        {
            DisposePreviousResources();
        }
    }

}
