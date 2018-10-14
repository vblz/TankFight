using System.Threading.Tasks;
using ImageService.Models;

namespace ImageService.Services.Interfaces
{
  public interface ICodeArchiver
  {
    string CreateArchive(LanguageConfiguration configuration);
  }
}