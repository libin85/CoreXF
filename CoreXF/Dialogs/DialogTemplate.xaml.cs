
using CoreXF;
using PropertyChanged;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DialogTemplate : Grid, IDialogXF
    {

        public static readonly BindableProperty VerticalDialogOptionProperty = BindableProperty.Create(nameof(VerticalDialogOption), typeof(LayoutOptions), typeof(Grid), LayoutOptions.Center);

        #region Properties

        public LayoutOptions VerticalDialogOption
        {
            get { return (LayoutOptions)GetValue(VerticalDialogOptionProperty); }
            set { SetValue(VerticalDialogOptionProperty, value); }
        }

        #endregion

        public Parameters Params { get; set; }

        public Command CloseCommand { get; set; }
        public Command OkCommand { get; set; }

        public BoxView HorizontalSeparator => _horizontalSeparator;

        public class Parameters : DialogParameters
        {
            [DependsOn(nameof(IsOkButton), nameof(IsCancelButton))]
            public bool ShowButtonSeparator => IsOkButton && IsCancelButton;

            public bool IsOkButton { get; set; }
            public bool IsCancelButton { get; set; }

            public string OkText { get; set; } = Tx.T("Dialogs_Ok");
            public string CancelText { get; set; } = Tx.T("Dialogs_Cancel");
            
            public string Title { get; set; }

        }

        public DialogTemplate() => InitializeComponent ();

        public void Initialize(DialogParameters param)
        {
            Params = param as Parameters;

            if (string.IsNullOrEmpty(Params.Title))
                _mainGrid.RowDefinitions[0].Height = 0;

            if(!Params.IsCancelButton && !Params.IsOkButton)
            {
                _mainGrid.RowDefinitions[2].Height = 0;
            }
            else if(Params.IsCancelButton && Params.IsOkButton)
            {

            }
            else if(!Params.IsCancelButton && Params.IsOkButton)
            {
                _buttonGrid.ColumnDefinitions[2].Width = 0;
                _buttonGrid.ColumnDefinitions[1].Width = 0;
            }
            else if(Params.IsCancelButton && !Params.IsOkButton)
            {
                _buttonGrid.ColumnDefinitions[0].Width = 0;
                _buttonGrid.ColumnDefinitions[1].Width = 0;
            }

        }
    }
}