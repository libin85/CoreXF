
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{

    public enum WrapAllocateRemainingSpace
    {
        None,
        AllocateProportionally
    }

    public class WrapLayout : AbstractCollectionLayout, IDisposable
    {

        DisposableItemsCollection _disposableItems = new DisposableItemsCollection();

        public bool OptimalDistibution { get; set; }
        public WrapAllocateRemainingSpace AllocateRemainingSpace { get; set; }

        //public int AllowReduceViewsInPercendts { get; set; } = 10;
        public int MaxCalculationTimeInMillis { get; set; } = 200;
        //public int AllowShrinkViewsInPercent { get; set; }

        #region Properties
        #endregion

        class WrapLayoutElement
        {
            public int Index;
            public View View;
            //public SizeRequest SizeRequst;

            public Point Position;

            public int WidthInt;
            public int HeightInt;

            public double WidthDbl;
            public double HeightDbl;
        }

        class MeasureItem
        {
            public double widthConstraint;
            public double heightConstraint;
            public List<WrapLayoutElement> AllElements;

            public SizeRequest SizeRequest;

            public bool Compare(List<WrapLayoutElement> NewAllElements, double newwidthConstraint, double newheightConstraint)
            {

                if ((AllElements?.Count ?? 0) != NewAllElements.Count)
                    return false;

                if (widthConstraint != newwidthConstraint || heightConstraint != newheightConstraint)
                    return false;

                for (int i = 1; i < AllElements.Count; i++)
                {
                    var elm1 = AllElements[i];
                    var elm2 = NewAllElements[i];

                    if (elm1.HeightDbl != elm2.HeightDbl || elm1.WidthDbl != elm2.WidthDbl)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        class CalculationElm
        {
            public WrapLayoutElement Element { get; set; }
            public int X;
            public int Y;
            public int Row;

            public double Width;
            public double Xd;
            public double Yd;
            public Rectangle Rectangle;
        }

        class LayoutCalculation
        {
            // Initial settings
            public List<WrapLayoutElement> AllElements;
            public int PaddingLeft;
            public int PaddingRight;
            public int PaddingTop;
            public int PaddingBottom;
            public int ColumnSpacing;
            public int RowSpacing;
            public int Width;


            // Local variables

            int x;
            int y;
            int rows;
            int elmInRowCounter;
            int width;
            int rowHeight;
            int elmWidth;

            public int columnSpacing;

            //Size size;
            WrapLayoutElement elm;

            // calculation
            public CalculationElm[] elements;
            public int TotalHeight;

            public LayoutCalculation(int size)
            {
                elements = new CalculationElm[size];
                for (int i = 0; i < size; i++)
                    elements[i] = new CalculationElm();
            }

            public int Calculate(int[] Variant)
            {
                x = 0;
                y = PaddingTop;
                rows = 0;
                elmInRowCounter = 0;
                TotalHeight = 0;
                width = 0;
                rowHeight = 0;
                TotalHeight = 0;
                columnSpacing = 0;


                for (int i = 0; i < Variant.Length; i++)
                {
                    /*
                    if ((Variant.Length -1) < i)
                    {
                        int ids = 4;
                    }
                    if ((AllElements.Count -1) < Variant[i])
                    {
                        int ids = 4;
                    }
                    */

                    elm = AllElements[Variant[i]];

                    columnSpacing = elmInRowCounter == 0 ? 0 : ColumnSpacing;

                    elmWidth = elm.WidthInt > Width ? Width : elm.WidthInt;

                    if (elmWidth + columnSpacing > width)
                    {
                        // Add row
                        x = PaddingLeft;
                        y += rowHeight;
                        if (rows > 0)
                        {
                            y += RowSpacing;
                            TotalHeight += RowSpacing;
                        }
                        width = Width - PaddingLeft - PaddingRight;
                        TotalHeight += rowHeight;
                        rowHeight = 0;
                        rows++;
                        elmInRowCounter = 0;
                    }

                    if (elmInRowCounter > 0)
                    {
                        x += ColumnSpacing;
                    }

                    elements[i].X = x;
                    elements[i].Y = y;
                    elements[i].Element = elm;
                    elements[i].Row = rows;

                    width -= elmWidth + columnSpacing;
                    x += elmWidth;

                    // height
                    if (rowHeight < elm.HeightInt)
                    {
                        rowHeight = elm.HeightInt;
                    }

                    elmInRowCounter++;

                }

                TotalHeight = TotalHeight + rowHeight + PaddingBottom;
                if (rows > 1)
                {
                    TotalHeight -= RowSpacing;
                }

                return TotalHeight;

            }

        }

        bool calculationIsExecuting = false;

        MeasureItem _cachedMeasure = new MeasureItem
        {
            SizeRequest = AbstractCollectionLayout.Size0
        };
        MeasureItem _calculatingMeasure = new MeasureItem();
        CancellationTokenSource cts;
        LayoutCalculation bestCalculation;

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {


            if (_disableMeasuring || Children.Count == 0)
            {
                return _cachedMeasure.SizeRequest;
            }

            //Debug.WriteLine($"Calc measuge: {widthConstraint} {heightConstraint} {Children.Count}");

            int idx = 0;
            List<WrapLayoutElement> _allElements = new List<WrapLayoutElement>(Children.Count);
            SizeRequest sizeRequest;
            foreach (var child in Children)
            {
                if (!child.IsVisible)
                    continue;

                sizeRequest = child.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
                if (sizeRequest.Request.Height == 0 || sizeRequest.Request.Width == 0)
                    continue;

                _allElements.Add(new WrapLayoutElement
                {
                    Index = idx++,
                    View = child,
                    HeightDbl = sizeRequest.Request.Height,
                    WidthDbl = sizeRequest.Request.Width,
                    HeightInt = (int)sizeRequest.Request.Height,
                    WidthInt = (int)sizeRequest.Request.Width
                });
            }

            if (_allElements.Count == 0)
                return AbstractCollectionLayout.Size0;

            if (_cachedMeasure.Compare(_allElements, widthConstraint, heightConstraint))
            {
                return _cachedMeasure.SizeRequest;
            }



            if (cts != null && _calculatingMeasure.Compare(_allElements, widthConstraint, heightConstraint))
            {
                // calculating is in progress
                //Debug.WriteLine("Calc: calculation is in progress");
                return _cachedMeasure.SizeRequest;
            }

            // calcuation isn't actual
            if (cts != null)
            {
                //Debug.WriteLine("Calc: Tryinng to cancel");
                cts.Cancel();
                cts = null;
            }

            _disposableItems += cts = new CancellationTokenSource();

            calculationObservable.OnNext(new MeasureItem
            {
                AllElements = _allElements,
                widthConstraint = widthConstraint,
                heightConstraint = heightConstraint
            });

            return _cachedMeasure.SizeRequest;

        }
        //MeasureItem
        //void CalculateOptimal(List<WrapLayoutElement> _allElements,double widthConstraint, double heightConstraint, CancellationToken token)
        void CalculateOptimal(MeasureItem measureItem, CancellationToken token)
        {
            bool isHereException = false;
            try
            {

                //Debug.WriteLine($"Calculation start ThreadId {Thread.CurrentThread.ManagedThreadId}");

                LayoutCalculation calcBest = new LayoutCalculation(measureItem.AllElements.Count)
                {
                    PaddingLeft = (int)Padding.Left,
                    PaddingRight = (int)Padding.Right,
                    PaddingBottom = (int)Padding.Bottom,
                    PaddingTop = (int)Padding.Top,
                    ColumnSpacing = (int)ColumnSpacing,
                    RowSpacing = (int)RowSpacing,
                    Width = (int)measureItem.widthConstraint
                };
                LayoutCalculation calcCurrent = calcBest.Clone();
                calcBest.AllElements = measureItem.AllElements;
                calcBest.TotalHeight = 10000;

                int[] rowArr = new int[measureItem.AllElements.Count];
                for (int i = 0; i < measureItem.AllElements.Count; i++)
                {
                    rowArr[i] = i;
                }

                if (OptimalDistibution)
                {

                    LayoutCalculation calcTemp;

                    calcCurrent.AllElements = measureItem.AllElements;

                    long cnt = 0, cnttotal = 0;
                    long startTicks = DateTime.Now.Ticks;
                    long periodTiks = MaxCalculationTimeInMillis * TimeSpan.TicksPerMillisecond;

                    foreach (var permutation in PermutationsExtensions.GetPermutations(rowArr))
                    {
                        if (token.IsCancellationRequested)
                        {
                            //Debug.Write($"Calculation canceled millis {(DateTime.Now.Ticks - startTicks) / TimeSpan.TicksPerMillisecond} ThreadId {Thread.CurrentThread.ManagedThreadId}");
                            goto End;
                        }
                        cnt++; cnttotal++;
                        calcCurrent.Calculate((int[])permutation);

                        if (calcCurrent.TotalHeight < calcBest.TotalHeight)
                        {
                            //Debug.WriteLine($"Best : {calcCurrent.TotalHeight} millis {(DateTime.Now.Ticks - startTicks) / TimeSpan.TicksPerMillisecond} ThreadId {Thread.CurrentThread.ManagedThreadId}");
                            calcTemp = calcBest;
                            calcBest = calcCurrent;
                            calcCurrent = calcTemp;
                        }
                        if (cnt > 5000)
                        {
                            //Debug.Write($"Total {cnttotal} time {DateTime.Now - startDate}");
                            cnt = 0;
                            if (DateTime.Now.Ticks - startTicks > periodTiks)
                            {
                                //Debug.Write($"Total {cnttotal} millis {(DateTime.Now.Ticks - startTicks) / TimeSpan.TicksPerMillisecond} ThreadId {Thread.CurrentThread.ManagedThreadId}");
                                break;
                            }
                        }
                    }
                    //Debug.Write($"Total {cnttotal}  millis {(DateTime.Now.Ticks - startTicks) / TimeSpan.TicksPerMillisecond} ThreadId {Thread.CurrentThread.ManagedThreadId}");
                    /*
                    foreach (var elm in _allCombinations)
                    {
                        string str = "";
                        foreach (var td in elm)
                        {
                            str += td.Index + $" ";
                        }
                        Debug.WriteLine(str);

                    }
                    */

                }
                else
                {
                    calcBest.Calculate(rowArr);
                }

                switch (AllocateRemainingSpace)
                {
                    case WrapAllocateRemainingSpace.None:
                        for (int i = 0; i < calcBest.elements.Length; i++)
                        {
                            var elm = calcBest.elements[i];
                            elm.Xd = elm.X;
                            elm.Yd = elm.Y;
                            elm.Width = elm.Element.WidthInt;
                            elm.Rectangle = new Rectangle(new Point(elm.Xd, elm.Yd), new Size(elm.Element.WidthDbl, elm.Element.HeightDbl));
                        }
                        break;

                    case WrapAllocateRemainingSpace.AllocateProportionally:
                        foreach (var group in calcBest.elements.GroupBy(x => x.Row))
                        {
                            double delta = measureItem.widthConstraint;
                            double deltaRemain = measureItem.widthConstraint;
                            double totalItemsWidth = 0;
                            //double X = 0;
                            //double Y = 0;
                            int cnt = 0;
                            double currentDelta = 0;

                            double accumulatedOffset = 0;

                            foreach (var elm in group)
                            {
                                delta -= elm.Element.WidthInt;
                                totalItemsWidth += elm.Element.WidthInt;
                                cnt++;
                            }

                            int arrLenght = group.Count();
                            int cntInGroup = 0;
                            foreach (var elm in group)
                            {
                                //var elm = arr[i];
                                elm.Yd = elm.Y;

                                if (cntInGroup != arrLenght - 1)
                                {
                                    currentDelta = delta * elm.Element.WidthInt / totalItemsWidth;
                                    elm.Width = elm.Element.WidthDbl + currentDelta;

                                    deltaRemain -= elm.Width;
                                }
                                else
                                {
                                    elm.Width = measureItem.widthConstraint - accumulatedOffset - elm.X;
                                }

                                elm.Xd = accumulatedOffset + elm.X;
                                elm.Rectangle = new Rectangle(new Point(elm.Xd, elm.Yd), new Size(elm.Width, elm.Element.HeightDbl));
                                accumulatedOffset += currentDelta;

                                cntInGroup++;
                            }


                        }
                        break;
                }

                Size size = new Size(measureItem.widthConstraint, calcBest.TotalHeight);
                measureItem.SizeRequest = new SizeRequest(size, size);
                _cachedMeasure = measureItem;

                bestCalculation = calcBest;

            End:;

            }
            catch (Exception ex)
            {
                isHereException = true;
                ExceptionManager.SendError(ex);
            }
            finally
            {
                calculationIsExecuting = false;
                if (!isHereException && !cts.IsCancellationRequested)
                {
                    Device.BeginInvokeOnMainThread(() => this.InvalidateMeasure());
                }
            }
        }


        ReplaySubject<MeasureItem> calculationObservable;
        public WrapLayout()
        {
            _disposableItems += calculationObservable = new ReplaySubject<MeasureItem>();
            _disposableItems += calculationObservable
                .Throttle(TimeSpan.FromMilliseconds(20))
                .Subscribe(x =>
                {
                    _disposableItems += cts = new CancellationTokenSource();
                    _calculatingMeasure = x;
                    CalculateOptimal(x, cts.Token);
                });



            //var sdf = this.Observable
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (bestCalculation.TotalHeight == 0)
                return;

            foreach (var elm in bestCalculation.elements)
            {
                LayoutChildIntoBoundingRegion(elm.Element.View, elm.Rectangle);
            }
        }

        public void Dispose()
        {
            _disposableItems.Dispose();
        }
    }
}
