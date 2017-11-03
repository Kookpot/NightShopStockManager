namespace NightShopStockManager
{
	public interface IFileHelper
	{
		string GetLocalFilePath(string filename);
        void SaveFile(string filename, string text);
	}
}