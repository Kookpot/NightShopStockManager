using Xamarin.Forms;
using Android.Media;
using NightShopStockManager;

[assembly: Dependency(typeof(MediaService))]
namespace NightShopStockManager
{
    public class MediaService : IMedia
    {
        public MediaService() { }

        private MediaPlayer _mediaPlayer;

        public void PlayOk()
        {
            _mediaPlayer = MediaPlayer.Create(Android.App.Application.Context, Resource.Raw.beepOk);
            _mediaPlayer.Start();
        }

        public void PlayNOk()
        {
            _mediaPlayer = MediaPlayer.Create(Android.App.Application.Context, Resource.Raw.beepNOk);
            _mediaPlayer.Start();
        }
    }
}