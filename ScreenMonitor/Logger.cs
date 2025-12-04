using System.Text.Json;

public static class Logger
{
    static string logPath = "logs";
    static string file = $"{DateTime.Now:yyyy-MM-dd}.jsonl";

    static Logger()
    {
        Directory.CreateDirectory(logPath);
    }

    public static void Log(CaptureEvent ev)
    {
        string line = JsonSerializer.Serialize(ev);
        File.AppendAllText(Path.Combine(logPath, file), line + Environment.NewLine);
    }
}
