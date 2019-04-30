
using Acr.UserDialogs;
using ReactiveUI;
using Splat;
using System.Linq;

namespace CoreXF
{
    public static class LocalizationCommands
    {
        public static ReactiveCommand ChangeLanguageCommand { get; private set; }

        static LocalizationCommands()
        {
            ChangeLanguageCommand = ReactiveCommand.Create(() =>
            {
                IUserDialogs _userDialogs = Locator.CurrentMutable.GetService<IUserDialogs>();
                INavigationService _navigationService = Locator.CurrentMutable.GetService<INavigationService>();

                ActionSheetConfig cfg = new ActionSheetConfig
                {
                    Options = CoreApp.Current.LanguageList
                        .Select(x => new ActionSheetOption(
                            text: x.NameExt,
                            action: async () =>
                            {
                                string newLanguageCode = x.TwoLetterISOLanguageName;

                                if (string.IsNullOrEmpty(newLanguageCode) || newLanguageCode == CoreApp.Current.CurrentCulture.TwoLetterISOLanguageName)
                                    return;

                                CoreApp.Current.SetLanguage(newLanguageCode);

                                //await _navigationService.OpenFirstPage(AppConfig.);
                                /*
                                var list = _navigationService.NavigationStack.ToList();
                                list.RemoveAt(list.Count - 1);
                                foreach (var pge in list)
                                    _navigationService.RemovePage(pge);
                                    */
                            },
                            icon: null))
                        .ToList()
                };
                _userDialogs.ActionSheet(cfg);
            });
        }

    }
}
