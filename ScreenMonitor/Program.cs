using System.Linq;

// 1) Açılışta çalışan app'leri göster
var apps = ProcessManager.GetRunningApps()
    .GroupBy(p => p.Name)
    .Select(g => g.First())
    .OrderBy(p => p.Name)
    .ToList();

Console.WriteLine("Running applications:");
int index = 1;

foreach (var app in apps)
{
  Console.WriteLine($"{index++}) {app.Name}");
}

// 2) Kullanıcı seçim yapsın
Console.WriteLine("\nEnter app names to track (comma separated):");
string input = Console.ReadLine() ?? "";

var toTrack = input.Split(',')
    .Select(x => x.Trim())
    .Where(x => x.Length > 0);

foreach (var name in toTrack)
{
  TrackingManager.AddApp(name);
}

// 3) ETW listener başlasın
var listener = new EtwListener();

listener.OnCapture += ev =>
{
  Console.WriteLine($"{ev.Timestamp} :: {ev.Process} (PID {ev.PID})");

  Logger.Log(ev);
  Notifier.Notify(ev);
};

Console.WriteLine("\nScreen Monitor Running... Press CTRL+C to exit.");
listener.Start();

await Task.Delay(-1);
