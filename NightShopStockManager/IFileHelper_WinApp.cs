using System.Threading.Tasks;

namespace NightShopStockManager
{
	public interface IFileHelper_WinApp
	{
		string GetLocalFilePath(string filename);
        Task SaveFileAsync(string filename, string text);
	}
}