using System;

namespace ImageService.Models
{
  public class LanguageConfiguration
  {
    private static readonly string[] AnswerFileFormats = new[]
    {
      "", //Cpp
      "", //Java
      "", //JavaScript
      "", //Python
      "runners/csharp/Answer{0}.cs"  //Csharp
    };

    private static readonly string[][] FilesConfigurations = new[]
    {
      new string[] {"", ""}, //Cpp
      new string[] {"", ""}, //Java
      new string[] {"", ""}, //JavaScript
      new string[] {"", ""}, //Python
      new string[] { "runners/csharp/Dockerfile", "runners/csharp/TestCode.csproj" } //Csharp
    };

    public string BuildNumber { get; }
    public string AnswerFile { get; }
    public string[] OtherFiles { get; }

    public static LanguageConfiguration Build(SupportedLanguages language)
    {
      var buildNumber = new Random().Next().ToString();

      string answerFile = string.Format(AnswerFileFormats[(int) language], buildNumber);

      return new LanguageConfiguration(answerFile, FilesConfigurations[(int) language], buildNumber);
    }

    private LanguageConfiguration(string answerFile, string[] otherFiles, string buildNumber)
    {
      AnswerFile = answerFile;
      OtherFiles = otherFiles;
      BuildNumber = buildNumber;
    }
  }
}