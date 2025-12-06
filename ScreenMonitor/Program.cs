using System.Linq;

class Program
{
  static async Task Main()
  {
    Console.Title = "Security Event Monitor";

    // 1️⃣ Çalışan uygulamaları listele
    var apps = ProcessManager.GetRunningApps()
        .GroupBy(p => p.Name)
        .Select(g => g.First())
        .OrderBy(p => p.Name)
        .ToList();

    Console.WriteLine("Running applications:");
    int index = 1;

    foreach (var app in apps)
      Console.WriteLine($"{index++}) {app.Name}");

    // 2️⃣ Kullanıcıdan track edilecekleri al
    Console.WriteLine("\nEnter app names to track (comma separated):");
    string input = Console.ReadLine() ?? "";

    var toTrack = input.Split(',')
        .Select(x => x.Trim())
        .Where(x => x.Length > 0);

    foreach (var name in toTrack)
      TrackingManager.AddApp(name);

    // 3️⃣ ETW Listener başlat
    var listener = new EtwListener();

    listener.OnCapture += ev =>
    {
      Logger.Log(ev);      // ✅ tüm olaylar tek log dosyasında
      Notifier.Notify(ev); // ✅ sadece gerçek tehlikede, 1 kere bildirim

      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"[SECURITY] {ev.Timestamp} :: {ev.Process} (PID {ev.PID}) -> {ev.EventName}");
      Console.ResetColor();
    };

    Console.WriteLine("\n✅ Security Monitor Running...");
    Console.WriteLine("✅ Waiting for Screen / Mic / Input threats...\n");

    listener.Start();

    // Programı sonsuza kadar açık tut
    await Task.Delay(-1);
  }
}
