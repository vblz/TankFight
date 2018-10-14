using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ImageService.Models;
using ImageService.Services.Interfaces;

namespace ImageService.Services.Implementation
{
  public class CodeArchiver : ICodeArchiver
  {
    private const string TgzFilenameFormat = "{0}.tar.gz";

    private readonly List<string> files = new List<string>();

    public string CreateArchive(LanguageConfiguration configuration)
    {
      var tgzFilename = string.Format(TgzFilenameFormat, configuration.BuildNumber);

      this.files.Clear();
      this.files.Add(configuration.AnswerFile);
      this.files.AddRange(configuration.OtherFiles);

      using (var outStream = File.Create(tgzFilename))
      using (var gzoStream = new GZipOutputStream(outStream))
      using (var tarArchive = TarArchive.CreateOutputTarArchive(gzoStream))
      {
        tarArchive.RootPath = Path.GetDirectoryName(configuration.AnswerFile);

        foreach (var file in this.files)
        {
          var tarEntry = TarEntry.CreateEntryFromFile(file);
          tarEntry.Name = Path.GetFileName(file);

          tarArchive.WriteEntry(tarEntry, true);
        }
      }

      return tgzFilename;
    }
  }
}