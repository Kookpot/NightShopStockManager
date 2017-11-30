using System;
using System.IO;
using Xamarin.Forms;
using NightShopStockManager.Droid;

[assembly: Dependency(typeof(FileHelper))]
namespace NightShopStockManager.Droid
{
	public class FileHelper : IFileHelper
	{
		public string GetLocalFilePath(string filename)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, filename);
		}

        public void SaveFile(string filename, string text)
        {
            var documentsPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            Directory.CreateDirectory(documentsPath + "/DCIM/Data_NightShop");
            var filePath = Path.Combine(documentsPath + "/DCIM/Data_NightShop", filename);
            File.WriteAllText(filePath, text);
        }
    }
}