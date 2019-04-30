
using CoreXF;
using Xamarin.Forms.Xaml;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectorPage : CoreXF.NavigationAbstraction.CommonPage
    {
        // Bindable properties
        public string Title { get; set; }
        public bool ShowSearchBar { get; set; }
        public ObservableCollection<SelectorItem> Items { get; set; }
        public string SearchText { get; set; }

        // Commands
        public Command ClearSearchCommand { get; set; }

        [Inject] INavigationService _navigation;

        public class SelectorItem : ObservableObject
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public bool IsSelected { get; set; }

            public string SearchSubsr {
                get => __SearchSubsr;
                set {
                    __SearchSubsr = string.IsNullOrWhiteSpace(value) ? "" : value.ToLower();
                } }
            string __SearchSubsr;

            public object Value { get; set; }

            public override string ToString() => Name;

            // system
            public static bool operator ==(SelectorItem x, SelectorItem y) => x?.Name == y?.Name;
            public static bool operator !=(SelectorItem x, SelectorItem y) => x?.Name != y?.Name;
            public override int GetHashCode() => Name.GetHashCode();
            public override bool Equals(object obj) => this == (obj as SelectorItem);

        }

        // Bindable properties
        public SelectorPage() => InitializeComponent();

        public class Parameters : PageParameters
        {
            public string Title { get; set; }
            public IEnumerable<SelectorItem> Items { get; set; }
            public bool ShowSearchBar { get; set; }
            public bool RemoveSelectorFromNavigationStackInsteadGoBack { get; set; }
            public SelectorItem SelectedItem { get; set; }

            public Parameters(IEnumerable<SelectorItem> items)
            {
                Items = items;
            }
        }
        Parameters Param;

        public override void Initialize()
        {
            base.Initialize();

            if (Param?.Items == null)
                return;

            Items = new ObservableCollection<SelectorItem>(Param.Items);
            Title = Param.Title;
            ShowSearchBar = Param.ShowSearchBar;

            if(Items != null && !Items.Any(x => !string.IsNullOrEmpty(x.SearchSubsr)))
            {
                ShowSearchBar = false;
            }

            _list.ItemTapped += _list_ItemTapped;

            DisposableItems += this.ObservableForProperty(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(x => {
                    string subs = SearchText.Trim().ToLower();
                    Items = new ObservableCollection<SelectorItem>(Param.Items.Where(y => y.SearchSubsr?.Contains(subs) ?? false));
                });

            ClearSearchCommand = new Command(() => { SearchText = ""; });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Param.SelectedItem != null)
            {
                _list.ScrollTo(Param.SelectedItem, ScrollToPosition.MakeVisible, false);
            }

        }

        async Task ScrollToCurrentItem()
        {
            
            await Task.Delay(600);
            _list.ScrollTo(Param.SelectedItem, ScrollToPosition.MakeVisible, animated: true);
        }

        private async void _list_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _list.ItemTapped -= _list_ItemTapped;
            

            if (Param?.OnSuccessfulCloseAsync != null)
                await Param.OnSuccessfulCloseAsync(e.Item);

            if (Param.RemoveSelectorFromNavigationStackInsteadGoBack)
            {
                _navigation.RemovePage(this);
            }
            else
            {
                BackCommand.Execute(null);
            }
            
        }
    }
}