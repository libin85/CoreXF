
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{

    public class HorizontalCatalog : ScrollView, IDisposable
    {
        public static readonly BindableProperty RowsProperty = BindableProperty.Create(nameof(Rows), typeof(int), typeof(HorizontalCatalog), 1);
        public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource), typeof(IEnumerable<object>), typeof(HorizontalCatalog), default(IEnumerable<object>));
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(HorizontalCatalog), default(DataTemplate));
        public static readonly BindableProperty SelectedItemCommandProperty = BindableProperty.Create(nameof(SelectedItemCommand), typeof(ICommand), typeof(HorizontalCatalog), default(ICommand));
        public static readonly BindableProperty UnevenColumnsProperty = BindableProperty.Create(nameof(UnevenColumns), typeof(bool), typeof(HorizontalCatalog), default(bool));

        #region Properties

        public bool UnevenColumns
        {
            get { return (bool)GetValue(UnevenColumnsProperty); }
            set { SetValue(UnevenColumnsProperty, value); }
        }

        public ICommand SelectedItemCommand
        {
            get { return (ICommand)GetValue(SelectedItemCommandProperty); }
            set { SetValue(SelectedItemCommandProperty, value); }
        }

        public IEnumerable<object> ItemSource
        {
            get { return (IEnumerable<object>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        #endregion

        Grid _grid;

        public HorizontalCatalog()
        {
            Orientation = ScrollOrientation.Horizontal;
            Content = _grid = new Grid();
            _grid.RowSpacing = 0;
            _grid.ColumnSpacing = 0;
        }

        private View GetItemView(object item)
        {
            object content;

            if (ItemTemplate is DataTemplateSelector selector)
            {
                var template = selector.SelectTemplate(item, this);

                if (template == null)
                    throw new InvalidOperationException(nameof(template));

                content = template.CreateContent();
            }
            else
            {
                content = ItemTemplate.CreateContent();
            }

            if (!(content is View) && !(content is ViewCell))
            {
                throw new RepeaterView.InvalidViewException("Templated control must be a View or a ViewCell");
            }

            var view = content is View ? content as View : ((ViewCell)content).View;

            view.BindingContext = item;

            if (SelectedItemCommand != null && SelectedItemCommand.CanExecute(item))
                BindSelectedItemCommand(view);

            /*
            if (ShowSeparator && ItemsSource.Cast<object>().Last() != item)
            {
                var container = new StackLayout { Spacing = 0 };

                container.Children.Add(view);
                container.Children.Add(BuildSeparator());

                return container;
            }
            */
            return view;
        }

        void Build()
        {
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            _grid.ColumnDefinitions.Clear();

            if (ItemSource == null || ItemTemplate == null)
                return;

            IEnumerator<object> enumerator = ItemSource.GetEnumerator();
            bool isThereNext = false;
            for (int col = 0; col < 1000; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    isThereNext = enumerator.MoveNext();
                    if (!isThereNext)
                        break;

                    _grid.Children.Add(GetItemView(enumerator.Current), col, row);
                }
                if (!isThereNext)
                    break;
            }

            foreach (RowDefinition row in _grid.RowDefinitions)
            {
                row.Height = GridLength.Auto;
            }
            foreach (ColumnDefinition col in _grid.ColumnDefinitions)
            {
                col.Width = UnevenColumns ? GridLength.Auto : GridLength.Star;
            }
        }

        private void BindSelectedItemCommand(View view)
        {
            if (!SelectedItemCommand.CanExecute(view.BindingContext))
                return;

            var tapGestureRecognizer = new TapGestureRecognizer { Command = SelectedItemCommand, CommandParameter = view.BindingContext };

            if (view.GestureRecognizers.Any())
                view.GestureRecognizers.Clear();

            view.GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            switch (propertyName)
            {
                case nameof(ItemSource):
                    if (ItemSource != null)
                    {
                        INotifyCollectionChanged incc = (ItemSource as INotifyCollectionChanged);
                        if (incc != null)
                        {
                            incc.CollectionChanged -= HorizontalCatalog_CollectionChanged;
                        }
                    }
                    break;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(ItemSource):
                    Build();
                    INotifyCollectionChanged incc = (ItemSource as INotifyCollectionChanged);
                    if (incc != null)
                    {
                        (ItemSource as INotifyCollectionChanged).CollectionChanged += HorizontalCatalog_CollectionChanged;
                    }
                    break;

                case nameof(ItemTemplate):
                    Build();
                    break;

                case nameof(SelectedItemCommand):
                    if (SelectedItemCommand == null)
                        return;

                    foreach (var _view in _grid.Children)
                    {
                        BindSelectedItemCommand(_view);
                    }
                    break;

            }
            base.OnPropertyChanged(propertyName);
        }

        private void HorizontalCatalog_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    Build();
                    break;
            }

        }

        public void Dispose()
        {
            INotifyCollectionChanged incc = (ItemSource as INotifyCollectionChanged);
            if (incc != null)
            {
                incc.CollectionChanged -= HorizontalCatalog_CollectionChanged;
            }
        }
    }


}
