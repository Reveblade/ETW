using System.Diagnostics;

public static class ProcessManager
{
  public static List<TrackedApp> GetRunningApps()
  {
    var list = new List<TrackedApp>();

    foreach (var p in Process.GetProcesses())
    {
      try
      {
        if (!string.IsNullOrWhiteSpace(p.ProcessName))
        {
          list.Add(new TrackedApp
          {
            Name = p.ProcessName,
            PID = p.Id
          });
        }
      }
      catch { }
    }

    return list;
  }
}

public class TrackedApp
{
  public string? Name { get; set; }
  public int PID { get; set; }
}
