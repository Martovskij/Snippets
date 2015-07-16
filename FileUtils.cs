using System.Collections.Generic;
using System.IO;
using System.Linq;

<<<<<<< HEAD
// Другой тестовйы коммент
=======
// Тестовый коммент
>>>>>>> 65b989dfc759140fa973655da83371b76326cc8c

namespace ConsoleApplication
{
  /// <summary>
  /// Утилиты для работы с файлами.
  /// </summary>
  public static class FileUtils
  {
    /// <summary>
    /// Скопировать папку.
    /// </summary>
    /// <param name="sourceDir">Папка источник.</param>
    /// <param name="targetDir">Папка приемник.</param>
    public static void CopyDirectory(string sourceDir, string targetDir, IEnumerable<string> excludedFileNames = null)
    {
      var sourceDirectoryInfo = new DirectoryInfo(sourceDir);
      var subDirectoriesInfo = sourceDirectoryInfo.GetDirectories();
      if (!Directory.Exists(targetDir))
        Directory.CreateDirectory(targetDir);

      var files = sourceDirectoryInfo.GetFiles();
      foreach (var file in files)
      {
        if (excludedFileNames != null && excludedFileNames.Any(f => f == file.Name))
          continue;

        file.CopyTo(Path.Combine(targetDir, file.Name), true);
      }

      foreach (var subdir in subDirectoriesInfo)
        CopyDirectory(subdir.FullName, Path.Combine(targetDir, subdir.Name));
    }

    /// <summary>
    /// Вычислить размер папки.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static long CalculateFolderSize(DirectoryInfo directory)
    {
      var foldersContainer = new Stack<DirectoryInfo>();
      long calculatedSize = 0;
      foldersContainer.AddFolders(new[] { directory });
      while (foldersContainer.Any())
      {
        var currentFolder = foldersContainer.Pop();
        foldersContainer.AddFolders(currentFolder.GetDirectories());
        calculatedSize += currentFolder.GetFiles().Sum(fileInfo => fileInfo.Length);
      }
      return calculatedSize;
    }

    /// <summary>
    /// Добавить папки в контейнер.
    /// </summary>
    /// <param name="folderContainer">Контейнер.</param>
    /// <param name="directories">Добавляемые папки.</param>
    private static void AddFolders(this Stack<DirectoryInfo> folderContainer, IEnumerable<DirectoryInfo> directories)
    {
      foreach (var directoryInfo in directories)
        folderContainer.Push(directoryInfo);
    }
  }
}
