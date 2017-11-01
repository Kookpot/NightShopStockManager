using Xamarin.Forms;
using NightShopStockManager;
using AudioToolbox;
using Foundation;

[assembly: Dependency(typeof(IMedia))]
namespace NightShopStockManager
{
    public class MediaService : IMedia
    {
        private NSUrl url;
        private SystemSound mySound;

        public MediaService() { }

        public void PlayOk()
        {
            url = NSUrl.FromFilename("Sounds/beepOk.mp3");
            mySound = new SystemSound(url);
            mySound.PlaySystemSound();
        }

        public void PlayNOk()
        {
            url = NSUrl.FromFilename("Sounds/beepNOk.mp3");
            mySound = new SystemSound(url);
            mySound.PlayAlertSound();
        }
    }
}