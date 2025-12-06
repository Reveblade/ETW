public static class DangerousEventFilter
{
  private static readonly string[] DangerousKeywords =
  {
        // 🎥 EKRAN KOPYALAMA
        "DesktopDuplication",
        "GraphicsCapture",
        "Dxgi",
        "D3D11",
        "FrameCapture",

        // 🎤 SES / MİKROFON
        "Wasapi",
        "AudioCapture",
        "Microphone",
        "MMDevice",

        // ⌨️ LOW LEVEL INPUT / HOOK
        "RawInput",
        "Keyboard",
        "LowLevelHook",
        "WH_KEYBOARD"
    };

  public static bool IsDangerous(string? eventName)
  {
    if (string.IsNullOrWhiteSpace(eventName))
      return false;

    foreach (var k in DangerousKeywords)
      if (eventName.Contains(k, StringComparison.OrdinalIgnoreCase))
        return true;

    return false;
  }
}
