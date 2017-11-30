using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace NightShopStockManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SellItemsPage : ContentPage
    {
        public SellItemsPage()
        {
            InitializeComponent();
            ScannerOverlay.ShowFlashButton = ScannerView.HasTorch;
            ScannerOverlay.ShowManualButton = true;
            ScannerOverlay.ShowSummaryButton = true;
            ScannerOverlay.TopText = "Hold the barcode in front of the camera";
            ScannerOverlay.BottomText = "Scanning will happen automatically";
            ScannerOverlay.FlashIcon = "ic_flash_on_white_48pt.png";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ScannerView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            //ScannerView.IsScanning = false;
            base.OnDisappearing();
        }

        private void ScannerOverlay_FlashButtonClicked(Button sender, EventArgs e)
        {
            ScannerView.IsTorchOn = !ScannerView.IsTorchOn;
            if (ScannerView.IsTorchOn)
                ScannerOverlay.FlashIcon = "ic_flash_off_white_48pt.png";
            else
                ScannerOverlay.FlashIcon = "ic_flash_on_white_48pt.png";
        }
    }
}