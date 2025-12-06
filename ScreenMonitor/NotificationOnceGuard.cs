public static class NotificationOnceGuard
{
  private static readonly HashSet<string> notified = new();

  public static bool CanNotify(string process)
  {
    lock (notified)
    {
      if (notified.Contains(process))
        return false;

      notified.Add(process);
      return true;
    }
  }
}
