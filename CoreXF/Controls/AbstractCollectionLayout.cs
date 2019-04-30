
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{
    public abstract class AbstractCollectionLayout : Layout<View>
    {
        public static SizeRequest Size0 = new SizeRequest();

        public double RowSpacing { get; set; }
        public double ColumnSpacing { get; set; }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(AbstractCollectionLayout), default(IEnumerable));
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(AbstractCollectionLayout), default(DataTemplate));
        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(AbstractCollectionLayout), default(ICommand));

        protected bool _disableMeasuring;

        #region Properties

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        void CreateViews()
        {
            _disableMeasuring = true;

            int currentCount = Children.Count;

            // remove unnecessery views
            int newCount = ItemsSource?.Count() ?? 0;
            if (currentCount > newCount)
            {
                int lastItme = currentCount - 1;
                for (int i = 0; i < (currentCount - newCount); i++)
                {
                    Children.RemoveAt(lastItme - i);
                }
            }

            // add new views
            if (currentCount < newCount)
            {
                for (int i = 0; i < (newCount - currentCount); i++)
                {
                    View view = CreateView();
                    if (view == null)
                    {
                        throw new Exception("WrapLayout: Cannot create content");
                    }
                    if(ItemSelectedCommand != null)
                    {
                        view.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = ItemSelectedCommand
                        });
                    }
                    Children.Add(view);
                }
            }

            if (newCount == 0)
                return;

            // set context
            int cnt = 0;
            foreach (var item in ItemsSource)
            {

                BindableObject bindableObject = Children[cnt++] as BindableObject;
                if (bindableObject != null)
                {
                    bindableObject.BindingContext = item;
                }
                TapGestureRecognizer gr = (bindableObject as View)?.GestureRecognizers?.FirstOrDefault(x => x is TapGestureRecognizer) as TapGestureRecognizer;
                if(gr != null)
                {
                    gr.CommandParameter = item;
                }

            }

            _disableMeasuring = false;

            InvalidateLayout();
        }

        protected virtual View CreateView()
        {
            return ItemTemplate.CreateContent() as View;
        }

        protected override bool ShouldInvalidateOnChildAdded(View child)
        {
            return false;
        }

        protected override bool ShouldInvalidateOnChildRemoved(View child)
        {
            return false;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(ItemsSource):
                    CreateViews();
                    break;

                case nameof(ItemSelectedCommand):
                    foreach (var view in Children)
                    {
                        TapGestureRecognizer gr = view.GestureRecognizers?.FirstOrDefault(x => x is TapGestureRecognizer) as TapGestureRecognizer;
                        if(gr != null)
                        {
                            view.GestureRecognizers.Remove(gr);
                        }
                        if(ItemSelectedCommand != null)
                        {
                            view.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = ItemSelectedCommand,
                                CommandParameter = view.BindingContext
                            });
                        }
                        
                    }
                    break;
            }

            base.OnPropertyChanged(propertyName);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            CreateViews();
        }

    }

}
