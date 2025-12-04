using System;
using System.Diagnostics;

public static class Notifier
{
    public static void Notify(CaptureEvent ev)
    {
        string title = "Screen Capture Detected";
        string message = $"{ev.Process} (PID {ev.PID})";

        string psCommand =
            $"New-BurntToastNotification -Text '{title}','{message}'";

        var psi = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{psCommand}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process.Start(psi);
    }
}
