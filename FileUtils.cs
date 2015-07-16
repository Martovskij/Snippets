using System.Collections.Generic;
using System.IO;
using System.Linq;

<<<<<<< HEAD
// ������ �������� �������
=======
// �������� �������
>>>>>>> 65b989dfc759140fa973655da83371b76326cc8c

namespace ConsoleApplication
{
  /// <summary>
  /// ������� ��� ������ � �������.
  /// </summary>
  public static class FileUtils
  {
    /// <summary>
    /// ����������� �����.
    /// </summary>
    /// <param name="sourceDir">����� ��������.</param>
    /// <param name="targetDir">����� ��������.</param>
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
    /// ��������� ������ �����.
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
    /// �������� ����� � ���������.
    /// </summary>
    /// <param name="folderContainer">���������.</param>
    /// <param name="directories">����������� �����.</param>
    private static void AddFolders(this Stack<DirectoryInfo> folderContainer, IEnumerable<DirectoryInfo> directories)
    {
      foreach (var directoryInfo in directories)
        folderContainer.Push(directoryInfo);
    }
  }
}
