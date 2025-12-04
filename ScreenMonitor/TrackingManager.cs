public static class TrackingManager
{
  private static HashSet<string> trackedApps = new(StringComparer.OrdinalIgnoreCase);

  public static void AddApp(string processName)
  {
    trackedApps.Add(processName.ToLower());
    Console.WriteLine($"[TRACKING] Started tracking {processName}");
  }

  public static void RemoveApp(string processName)
  {
    trackedApps.Remove(processName.ToLower());
    Console.WriteLine($"[TRACKING] Stopped tracking {processName}");
  }

  public static bool IsTracked(string processName)
  {
    return trackedApps.Contains(processName.ToLower());
  }

  public static IEnumerable<string> GetTrackedApps()
  {
    return trackedApps;
  }
}
