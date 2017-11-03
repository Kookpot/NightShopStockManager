using System.IO;
using Xamarin.Forms;
using System;
using NightShopStockManager.UWP;
using Windows.Storage;
using System.Threading.Tasks;

[assembly: Dependency(typeof(FileHelper))]
namespace NightShopStockManager.UWP
{
	public class FileHelper : IFileHelper_WinApp
	{
		public string GetLocalFilePath(string filename)
		{
			return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
		}

        public async Task SaveFileAsync(string filename, string text)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, text);
        }
    }
}