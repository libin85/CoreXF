
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{
    public class PropertyGrid : Grid
    {
        public static readonly BindableProperty PropertyTemplateProperty = BindableProperty.Create(nameof(PropertyTemplate), typeof(DataTemplate), typeof(PropertyGrid), default(DataTemplate));
        public static readonly BindableProperty ValueTemplateProperty = BindableProperty.Create(nameof(ValueTemplate), typeof(DataTemplate), typeof(PropertyGrid), default(DataTemplate));
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(HorizontalCatalog), default(IEnumerable));
        public static readonly BindableProperty SelectedItemCommandProperty = BindableProperty.Create(nameof(SelectedItemCommand), typeof(ICommand), typeof(RepeaterView), default(ICommand));

        #region Properties

        public DataTemplate ValueTemplate
        {
            get { return (DataTemplate)GetValue(ValueTemplateProperty); }
            set { SetValue(ValueTemplateProperty, value); }
        }

        public DataTemplate PropertyTemplate
        {
            get { return (DataTemplate)GetValue(PropertyTemplateProperty); }
            set { SetValue(PropertyTemplateProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ICommand SelectedItemCommand
        {
            get { return (ICommand)GetValue(SelectedItemCommandProperty); }
            set { SetValue(SelectedItemCommandProperty, value); }
        }


        #endregion


        public PropertyGrid() : base()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;
        }

        private View GetItemView(DataTemplate template,object item)
        {
            var content = template.CreateContent();
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
            Children.Clear();
            RowDefinitions.Clear();

            if (ItemsSource == null)
                return;

            if (ColumnDefinitions.Count == 0)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }


            int idx = 0;
            foreach(var elm in ItemsSource)
            {
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Children.Add(GetItemView(PropertyTemplate,elm),0,idx);
                Children.Add(GetItemView(ValueTemplate, elm), 1, idx);

                idx++;
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


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(ItemsSource):
                    Build();
                    break;

                case nameof(SelectedItemCommand):
                    if (SelectedItemCommand == null)
                        return;

                    foreach (var view in Children)
                    {
                        BindSelectedItemCommand(view);
                    }
                    break;

            }
            base.OnPropertyChanged(propertyName);
        }
    }
}
