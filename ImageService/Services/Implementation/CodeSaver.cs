using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ImageService.Services.Interfaces;

namespace ImageService.Services.Implementation
{
  public class CodeSaver : ICodeSaver
  {
    public async Task Save(string code, string path)
    {
      if (string.IsNullOrWhiteSpace(code))
      {
        throw new ArgumentException("Код пользователя пустой.", nameof(code));
      }

      if (string.IsNullOrWhiteSpace(path))
      {
        throw new ArgumentException("Путь для сохранения кода пустой.", nameof(path));
      }

      using (var fileStream = File.Create(path))
      {
        var bytes = Encoding.UTF8.GetBytes(code);

        await fileStream.WriteAsync(bytes);
      }
    }
  }
}