
using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{
    // Inspired by  https://github.com/mattleibow/InfiniteScrolling
    //
    //<ListView.Behaviors>
    //    <core:InfiniteScrollBehavior Command = "{Binding LastItemAppearingCommand}" />
    //</ ListView.Behaviors >
    public class InfiniteScrollBehavior : Behavior<ListView>
    {
        public static readonly BindableProperty  IsLoadingMoreProperty = BindableProperty.Create(nameof(IsLoadingMore),typeof(bool),typeof(InfiniteScrollBehavior),default(bool),BindingMode.OneWayToSource);
        private static readonly BindableProperty ItemsSourceProperty =  BindableProperty.Create(nameof(ItemsSource),typeof(IEnumerable),typeof(InfiniteScrollBehavior),default(IEnumerable),BindingMode.OneWay,propertyChanged: OnItemsSourceChanged);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(InfiniteScrollBehavior), default(ICommand));

        #region Properties

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public bool IsLoadingMore
        {
            get => (bool)GetValue(IsLoadingMoreProperty);
            private set => SetValue(IsLoadingMoreProperty, value);
        }

        #endregion

        private bool isLoadingMoreFromScroll;
        private bool isLoadingMoreFromLoader;
        private ListView associatedListView;


        private IEnumerable ItemsSource => (IEnumerable)GetValue(ItemsSourceProperty);

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);

            associatedListView = bindable;

            SetBinding(ItemsSourceProperty, new Binding(ListView.ItemsSourceProperty.PropertyName, source: associatedListView));

            bindable.BindingContextChanged += OnListViewBindingContextChanged;
            bindable.ItemAppearing += OnListViewItemAppearing;

            BindingContext = associatedListView.BindingContext;
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            RemoveBinding(ItemsSourceProperty);

            bindable.BindingContextChanged -= OnListViewBindingContextChanged;
            bindable.ItemAppearing -= OnListViewItemAppearing;

            base.OnDetachingFrom(bindable);
        }

        private void OnListViewBindingContextChanged(object sender, EventArgs e)
        {
            BindingContext = associatedListView.BindingContext;
        }

        private void OnListViewItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (Command == null || !Command.CanExecute(e.Item) || !ShouldLoadMore(e.Item))
            {
                return;
            }

            UpdateIsLoadingMore(true, null);
            Command.Execute(e.Item);
            UpdateIsLoadingMore(false, null);
        }

        bool ShouldLoadMore(object item)
        {
            if (associatedListView.ItemsSource is IList list)
            {
                if (list.Count == 0)
                    return true;
                var lastItem = list[list.Count - 1];
                if (associatedListView.IsGroupingEnabled && lastItem is IList group)
                    return group.Count == 0 || group[group.Count - 1] == item;
                else
                    return lastItem == item;
            }
            return false;
        }


        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            /*
            if (bindable is InfiniteScrollBehavior behavior)
            {
                if (oldValue is IInfiniteScrollLoading oldLoading)
                {
                    oldLoading.LoadingMore -= behavior.OnLoadingMore;
                    behavior.UpdateIsLoadingMore(null, false);
                }
                if (newValue is IInfiniteScrollLoading newLoading)
                {
                    newLoading.LoadingMore += behavior.OnLoadingMore;
                    behavior.UpdateIsLoadingMore(null, newLoading.IsLoadingMore);
                }
            }*/
        }

        /*
        private void OnLoadingMore(object sender, LoadingMoreEventArgs e)
        {
            UpdateIsLoadingMore(null, e.IsLoadingMore);
        }
        */
        private void UpdateIsLoadingMore(bool? fromScroll, bool? fromLoader)
        {
            isLoadingMoreFromScroll = fromScroll ?? isLoadingMoreFromScroll;
            isLoadingMoreFromLoader = fromLoader ?? isLoadingMoreFromLoader;

            IsLoadingMore = isLoadingMoreFromScroll || isLoadingMoreFromLoader;
        }
    }

}
