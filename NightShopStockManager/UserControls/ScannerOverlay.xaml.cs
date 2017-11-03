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

        public delegate void PlusButtonClickedDelegate(Button sender, EventArgs e);
        public event PlusButtonClickedDelegate PlusButtonClicked;

        public delegate void MinusButtonClickedDelegate(Button sender, EventArgs e);
        public event MinusButtonClickedDelegate MinusButtonClicked;

        public delegate void DoneButtonClickedDelegate(Button sender, EventArgs e);
        public event DoneButtonClickedDelegate DoneButtonClicked;

        public delegate void CancelButtonClickedDelegate(Button sender, EventArgs e);
        public event CancelButtonClickedDelegate CancelButtonClicked;

        public delegate void SummaryButtonClickedDelegate(Button sender, EventArgs e);
        public event SummaryButtonClickedDelegate SummaryButtonClicked;

        public ScannerOverlay()
        {
            InitializeComponent();
            number.Text = "0";
            total.Text = "€ 0.00";
            flash.Clicked += (sender, e) => {
                FlashButtonClicked?.Invoke(flash, e);
            };
            manual.Clicked += (sender, e) => {
                ManualButtonClicked?.Invoke(manual, e);
            };
            plus.Clicked += (sender, e) => {
                PlusButtonClicked?.Invoke(plus, e);
            };
            minus.Clicked += (sender, e) => {
                MinusButtonClicked?.Invoke(minus, e);
            };
            done.Clicked += (sender, e) => {
                DoneButtonClicked?.Invoke(done, e);
            };
            cancel.Clicked += (sender, e) => {
                CancelButtonClicked?.Invoke(cancel, e);
            };
            summary.Clicked += (sender, e) => {
                SummaryButtonClicked?.Invoke(cancel, e);
            };
        }

        public string TopText
        {
            set { topText.Text = value; }
        }

        public string BottomText
        {
            set { botText.Text = value; }
        }

        public bool ShowFlashButton
        {
            set { flash.IsVisible = false; }
        }

        public bool ShowManualButton
        {
            set { manual.IsVisible = value; }
        }

        public static BindableProperty CounterProperty =
            BindableProperty.Create(nameof(Counter), typeof(int), typeof(ScannerOverlay), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: OnCounterChanged);
        public int Counter
        {
            get { return (int)GetValue(CounterProperty); }
            set
            {
                SetValue(CounterProperty, value);
                number.Text = value.ToString();
            }
        }

        private static void OnCounterChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.number == null) return;
            overlay.number.Text = newValue.ToString();
        }

        public static BindableProperty TotalProperty =
    BindableProperty.Create(nameof(Total), typeof(string), typeof(ScannerOverlay), defaultValue: " 0.00", defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: OnTotalChanged);
        public string Total
        {
            get { return (string)GetValue(TotalProperty); }
            set
            {
                SetValue(TotalProperty, value);
                total.Text = value;
            }
        }

        private static void OnTotalChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.total == null) return;
            overlay.total.Text = (string) newValue;
        }

        public string FlashIcon
        {
            set { flash.Image = value; }
        }

        public bool ShowSummaryButton
        {
            set
            {
                plus.IsVisible = value;
                minus.IsVisible = value;
                number.IsVisible = value;
                done.IsVisible = value;
                cancel.IsVisible = value;
                total.IsVisible = value;
                summary.IsVisible = value;
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

        public static BindableProperty PlusCommandProperty =
            BindableProperty.Create(nameof(PlusCommand), typeof(ICommand), typeof(ScannerOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnPlusCommandChanged);
        public ICommand PlusCommand
        {
            get { return (ICommand)GetValue(PlusCommandProperty); }
            set { SetValue(PlusCommandProperty, value); }
        }

        private static void OnPlusCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.plus == null) return;
            overlay.plus.Command = newValue as Command;
        }

        public static BindableProperty MinusCommandProperty =
            BindableProperty.Create(nameof(MinusCommand), typeof(ICommand), typeof(ScannerOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnMinusCommandChanged);
        public ICommand MinusCommand
        {
            get { return (ICommand)GetValue(MinusCommandProperty); }
            set { SetValue(MinusCommandProperty, value); }
        }

        private static void OnMinusCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.minus == null) return;
            overlay.minus.Command = newValue as Command;
        }

        public static BindableProperty DoneCommandProperty =
            BindableProperty.Create(nameof(DoneCommand), typeof(ICommand), typeof(ScannerOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnDoneCommandChanged);
        public ICommand DoneCommand
        {
            get { return (ICommand)GetValue(DoneCommandProperty); }
            set { SetValue(DoneCommandProperty, value); }
        }

        private static void OnDoneCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.done == null) return;
            overlay.done.Command = newValue as Command;
        }

        public static BindableProperty CancelCommandProperty =
            BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(ScannerOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnCancelCommandChanged);
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        private static void OnCancelCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.cancel == null) return;
            overlay.cancel.Command = newValue as Command;
        }

        public static BindableProperty SummaryCommandProperty =
            BindableProperty.Create(nameof(SummaryCommand), typeof(ICommand), typeof(ScannerOverlay),
            defaultValue: default(ICommand),
        propertyChanged: OnSummaryCommandChanged);
        public ICommand SummaryCommand
        {
            get { return (ICommand)GetValue(SummaryCommandProperty); }
            set { SetValue(SummaryCommandProperty, value); }
        }

        private static void OnSummaryCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var overlay = bindable as ScannerOverlay;
            if (overlay?.summary == null) return;
            overlay.summary.Command = newValue as Command;
        }
    }
}