using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NightShopStockManager
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerOverlay : Grid
    {

        public delegate void FlashButtonClickedDelegate(Button sender, EventArgs e);
        public event FlashButtonClickedDelegate FlashButtonClicked;

        public delegate void ManualButtonClickedDelegate(Button sender, EventArgs e);
        public event ManualButtonClickedDelegate ManualButtonClicked;

        public ScannerOverlay()
        {
            InitializeComponent();
            topText.SetBinding(Label.TextProperty, new Binding(nameof(TopText)));
            botText.SetBinding(Label.TextProperty, new Binding(nameof(BottomText)));
            flash.SetBinding(IsVisibleProperty, new Binding(nameof(ShowFlashButton)));
            flash.Clicked += (sender, e) => {
                FlashButtonClicked?.Invoke(flash, e);
            };
            manual.Clicked += (sender, e) => {
                ManualButtonClicked?.Invoke(manual, e);
            };
        }

        public static readonly BindableProperty TopTextProperty =
            BindableProperty.Create(nameof(TopText), typeof(string), typeof(ScannerOverlay), string.Empty);
        public string TopText
        {
            get { return (string)GetValue(TopTextProperty); }
            set { SetValue(TopTextProperty, value); }
        }

        public static readonly BindableProperty BottomTextProperty =
            BindableProperty.Create(nameof(BottomText), typeof(string), typeof(ScannerOverlay), string.Empty);
        public string BottomText
        {
            get { return (string)GetValue(BottomTextProperty); }
            set { SetValue(BottomTextProperty, value); }
        }

        public static readonly BindableProperty FlashIconProperty =
           BindableProperty.Create(nameof(FlashIcon), typeof(string), typeof(ScannerOverlay), string.Empty);
        public string FlashIcon
        {
            get { return (string)GetValue(FlashIconProperty); }
            set { SetValue(FlashIconProperty, value); }
        }

        public static readonly BindableProperty ShowFlashButtonProperty =
            BindableProperty.Create(nameof(ShowFlashButton), typeof(bool), typeof(ScannerOverlay), false);
        public bool ShowFlashButton
        {
            get { return (bool)GetValue(ShowFlashButtonProperty); }
            set { SetValue(ShowFlashButtonProperty, value); }
        }

        public static readonly BindableProperty ShowManualButtonProperty =
            BindableProperty.Create(nameof(ShowManualButton), typeof(bool), typeof(ScannerOverlay), false);
        public bool ShowManualButton
        {
            get { return (bool)GetValue(ShowManualButtonProperty); }
            set {
                SetValue(ShowManualButtonProperty, value);
                manual.IsVisible = value;
            }
        }

        public static BindableProperty FlashCommandProperty =
            BindableProperty.Create(nameof(FlashCommand), typeof(ICommand), typeof(ScannerOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnFlashCommandChanged);
        public ICommand FlashCommand
        {
            get { return (ICommand)GetValue(FlashCommandProperty); }
            set { SetValue(FlashCommandProperty, value); }
        }

        private static void OnFlashCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.flash == null) return;
            overlay.flash.Command = newValue as Command;
        }
    }
}