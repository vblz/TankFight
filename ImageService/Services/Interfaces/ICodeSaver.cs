using System.Threading.Tasks;
using ImageService.Models;

namespace ImageService.Services.Interfaces
{
  public interface ICodeSaver
  {
    Task Save(string code, string path);
  }
}