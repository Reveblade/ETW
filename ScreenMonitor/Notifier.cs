using System.Diagnostics;

public static class Notifier
{
    public static void Notify(CaptureEvent ev)
    {
        if (!NotificationOnceGuard.CanNotify(ev.Process ?? "unknown"))
            return;

        string title = "⚠️ Screen / Mic / Input Detected";
        string message = $"{ev.Process} → {ev.EventName}";

        string psCommand =
            $"New-BurntToastNotification -Text '{title}','{message}'";

        var psi = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try { Process.Start(psi); } catch { }
    }
}
