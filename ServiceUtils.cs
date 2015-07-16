using System;
using System.Linq;
using System.ServiceProcess;

namespace InstallerUtils
{
  /// <summary>
  /// Методы для взаимодействия со службами Windows.
  /// </summary>
  public static class ServiceUtils
  {
    /// <summary>
    /// Получить отображаемое имя службы.
    /// </summary>
    /// <param name="serviceName">Системное имя службы.</param>
    /// <returns>Отображаемое имя службы.</returns>
    public static string GetServiceDisplayName(string serviceName)
    {
      var service = ServiceController.GetServices()
        .SingleOrDefault(s => string.Compare(s.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase) == 0);

      return service != null ? service.DisplayName : string.Empty;
    }
  }
}
