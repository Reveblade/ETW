using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;

public class EtwListener
{
  public event Action<CaptureEvent>? OnCapture;

  public void Start()
  {
    Task.Run(() =>
    {
      using var session = new TraceEventSession("SecurityCaptureSession");

      // ===========================
      // 🎥 GERÇEK EKRAN KAYDI (GPU)
      // ===========================
      session.EnableProvider("Microsoft-Windows-DXGI");
      session.EnableProvider("Microsoft-Windows-D3D11");
      session.EnableProvider("Microsoft-Windows-WinRT-Graphics-Capture");

      // ===========================
      // 🎤 GERÇEK SES / MİKROFON
      // ===========================
      session.EnableProvider("Microsoft-Windows-Audio");
      session.EnableProvider("Microsoft-Windows-MMDevice");
      session.EnableProvider("Microsoft-Windows-WASAPI");

      // ===========================
      // ⌨️ GERÇEK INPUT / HOOK
      // ===========================
      session.EnableProvider("Microsoft-Windows-UserInput");
      session.EnableProvider("Microsoft-Windows-Input");

      // ===========================
      // (İsteğe bağlı) UI seviyesi
      // ===========================
      session.EnableProvider("Microsoft-Windows-Win32k");

      session.Source.Dynamic.All += evt =>
          {
            string eventName = evt.EventName;
            int pid = evt.ProcessID;
            string proc = GetProcessNameSafe(pid);

            // ✅ Sadece kullanıcı tarafından track edilenler
            if (!TrackingManager.IsTracked(proc))
              return;

            // ✅ Sadece gerçekten tehlikeli event'ler
            if (!DangerousEventFilter.IsDangerous(eventName))
              return;

            var capture = new CaptureEvent
            {
              Process = proc,
              PID = pid,
              EventName = eventName,
              Timestamp = DateTime.Now
            };

            OnCapture?.Invoke(capture);
          };

      session.Source.Process();
    });
  }

  private static string GetProcessNameSafe(int pid)
  {
    try { return Process.GetProcessById(pid).ProcessName; }
    catch { return "unknown"; }
  }
}
