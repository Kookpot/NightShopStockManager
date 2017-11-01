using Xamarin.Forms;
using Windows.Media.Playback;
using Windows.Media.Core;
using System;
using NightShopStockManager.UWP;

[assembly: Dependency(typeof(MediaService))]
namespace NightShopStockManager.UWP
{
    public class MediaService : IMedia
    {
        private readonly MediaPlayer _player;

        public MediaService() {
            _player = BackgroundMediaPlayer.Current;
        }

        public void PlayOk()
        {
            _player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///beepOk.mp3"));
            _player.Play();
        }

        public void PlayNOk()
        {
            _player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///beepNOk.mp3"));
            _player.Play();
        }
    }
}