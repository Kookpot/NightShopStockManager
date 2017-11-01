using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace NightShopStockManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyItemsPage : ContentPage
    {
        public BuyItemsPage()
        {
            InitializeComponent();
            ScannerOverlay.ShowFlashButton = ScannerView.HasTorch;
            ScannerOverlay.FlashIcon = "ic_flash_on_white_48pt.png";
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