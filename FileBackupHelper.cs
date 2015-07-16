using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InstallerActions.FileSystem
{
  /// <summary>
  /// Хелпер для бэкапа папок.
  /// </summary>
  public static class FileBackupHelper
  {
    /// <summary>
    /// Токен файлового хранилища.
    /// </summary>
    private const string StorageToken = "7C7B5215-BCC4-43AF-AE72-3A2D8BDAC547";

    /// <summary>
    /// Имя файла с информацией о бэкапе.
    /// </summary>
    private static readonly string BackupInfoFileName = string.Format("@backup_info.{0}", StorageToken);

    /// <summary>
    /// Папка, в которой храниться бэкап.
    /// </summary>
    private static string BackupFolder
    {
      get
      {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StorageToken);
      }
    }

    /// <summary>
    /// Забэкапить папку в хранилище.
    /// </summary>
    /// <param name="folderPath">Путь к сохраняемой папке.</param>
    public static void Backup(string folderPath)
    {
      Backup(new[] { folderPath });
    }

    /// <summary>
    /// Забэкапить папки в хранилище.
    /// </summary>
    /// <param name="folderPaths">Коллекция путей к сохраняемым папкам.</param>
    public static void Backup(IEnumerable<string> folderPaths)
    {
      CheckPossibilityOfBackup(folderPaths);
      foreach (var path in folderPaths)
      {
        var targetFolder = Path.Combine(BackupFolder, path.Split('\\').Last());
        FileUtils.CopyDirectory(path, targetFolder);
        using (var infoFile = File.CreateText(GetBackupInfoFilePath(targetFolder)))
        {
          infoFile.WriteLine(path);
          infoFile.Flush();
        }
      }
    }

    /// <summary>
    /// Восстановить сохраненные папки.
    /// </summary>
    public static void RestoreData()
    {
      foreach (var storedDirectoryPath in Directory.GetDirectories(BackupFolder))
      {
        var storedDirectory = new DirectoryInfo(storedDirectoryPath);
        var backupInfo = storedDirectory.GetFiles().SingleOrDefault(f => f.Name == BackupInfoFileName);
        if (backupInfo == null)
          continue;

        using (var reader = new StreamReader(backupInfo.FullName))
        {
          var restorePath = Regex.Replace(reader.ReadToEnd(), @"\t|\n|\r", "");
          CheckPossibilityOfRestore(storedDirectoryPath, restorePath);
          FileUtils.CopyDirectory(storedDirectoryPath, restorePath, new[] { BackupInfoFileName });
        }
        storedDirectory.Delete(true);
      }
    }

    /// <summary>
    /// Получить путь к файлу с информацией о бэкапе.
    /// </summary>
    /// <param name="targetFolder"></param>
    /// <returns></returns>
    private static string GetBackupInfoFilePath(string targetFolder)
    {
      var fileName = string.Format(BackupInfoFileName, StorageToken);
      return Path.Combine(targetFolder, fileName);
    }

    /// <summary>
    /// Проверить возможность выполнения бэкапа.
    /// </summary>
    /// <param name="folders">Папки, подлежащие бэкапу.</param>
    private static void CheckPossibilityOfBackup(IEnumerable<string> folders)
    {
      var availableFreeSpace = GetDriveFreeSpace(BackupFolder);
      var requiredSpace = folders.Sum(f =>
      {
        var directoryInfo = new DirectoryInfo(f);
        return directoryInfo.Exists ? FileUtils.CalculateFolderSize(directoryInfo) : 0;
      });

      if (availableFreeSpace < requiredSpace)
        throw new InvalidOperationException("Unavailable free space");
    }

    /// <summary>
    /// Проверить возможность восстановления файлов.
    /// </summary>
    /// <param name="restoreFolder">Откуда будут восстановлены файлы.</param>
    /// <param name="restorePath">Куда будут восстановлены файлы.</param>
    private static void CheckPossibilityOfRestore(string restoreFolder, string restorePath)
    {
      var requiredSpace = FileUtils.CalculateFolderSize(new DirectoryInfo(restoreFolder));
      var availableFreeSpace = GetDriveFreeSpace(restorePath);
      if (availableFreeSpace < requiredSpace)
        throw new InvalidOperationException("Unavailable free space");
    }

    /// <summary>
    /// Получить размер свободного пространства на диске.
    /// </summary>
    /// <returns>Размер свободного пространства на диске в байтах.</returns>
    private static long GetDriveFreeSpace(string folder)
    {
      var appDataDriveName = string.Format("{0}\\", folder.Split('\\').First());
      return DriveInfo.GetDrives().Single(d => d.Name == appDataDriveName).TotalFreeSpace;
    }
  }
}
