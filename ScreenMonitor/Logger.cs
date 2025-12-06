using System.Text.Json;

public static class Logger
{
    private static readonly string filePath = "security_log.jsonl";

    public static void Log(CaptureEvent ev)
    {
        string line = JsonSerializer.Serialize(ev);
        File.AppendAllText(filePath, line + Environment.NewLine);
    }
}
