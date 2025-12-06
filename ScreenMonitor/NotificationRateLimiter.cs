public static class NotificationRateLimiter
{
  private static readonly Dictionary<string, DateTime> lastNotify = new();
  private static readonly TimeSpan cooldown = TimeSpan.FromSeconds(5);

  public static bool CanNotify(string process)
  {
    lock (lastNotify)
    {
      if (!lastNotify.ContainsKey(process))
      {
        lastNotify[process] = DateTime.Now;
        return true;
      }

      var diff = DateTime.Now - lastNotify[process];
      if (diff > cooldown)
      {
        lastNotify[process] = DateTime.Now;
        return true;
      }

      return false;
    }
  }
}
