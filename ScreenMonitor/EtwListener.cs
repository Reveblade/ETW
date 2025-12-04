using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;

public class EtwListener
{
  public event Action<CaptureEvent>? OnCapture;

  // === Spam kontrol ===
  private static Dictionary<int, DateTime> lastEvent = new();
  private const int COOLDOWN_MS = 5000; // Aynı uygulama için 5 saniyeden önce tekrar event verme

  public void Start()
  {
    Task.Run(() =>
    {
      using var session = new TraceEventSession("ScreenCaptureSession");
      session.EnableProvider("Microsoft-Windows-Win32k");

      session.Source.Dynamic.All += evt =>
          {
            string eventName = evt.EventName;

            // === 1) SADECE GERÇEK SCREEN CAPTURE EVENTLERİ ===
            if (!(eventName.Contains("BitBlt") ||
                      eventName.Contains("CopySurface") ||
                      eventName.Contains("PrintWindow")))
            {
              return;
            }

            int pid = evt.ProcessID;
            string proc = GetProcessNameSafe(pid);

            // === 2) Sistem süreçlerini ignore et ===
            if (proc.Equals("dwm", StringComparison.OrdinalIgnoreCase) ||
                    proc.Equals("explorer", StringComparison.OrdinalIgnoreCase) ||
                    proc.Equals("ShellExperienceHost", StringComparison.OrdinalIgnoreCase))
            {
              return;
            }

            // === 3) Track edilmeyen app ise ignore ===
            if (!TrackingManager.IsTracked(proc))
              return;

            // === 4) SPAM ÖNLEME / COOL-DOWN ===
            if (lastEvent.ContainsKey(pid))
            {
              double diff = (DateTime.Now - lastEvent[pid]).TotalMilliseconds;

              if (diff < COOLDOWN_MS)
                return; // Çok sık geliyorsa at
            }

            lastEvent[pid] = DateTime.Now;

            // === 5) ARTIK EVENT GERÇEK ===
            OnCapture?.Invoke(new CaptureEvent
            {
              Process = proc,
              PID = pid,
              Timestamp = DateTime.Now
            });
          };

      session.Source.Process();
    });
  }

  // Güvenli şekilde process adını alma
  private static string GetProcessNameSafe(int pid)
  {
    try
    {
      var p = Process.GetProcessById(pid);
      return p.ProcessName;
    }
    catch
    {
      return "unknown";
    }
  }
}

public class CaptureEvent
{
  public string? Process { get; set; }
  public int PID { get; set; }
  public DateTime Timestamp { get; set; }
}
