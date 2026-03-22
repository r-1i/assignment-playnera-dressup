using System;

namespace DressUp.Core
{
public static class AudioEvents
{
    public static event Action OnCreamApplied;
    public static event Action OnBrushPickup;
    public static event Action OnBrushApplied;
    public static event Action OnLipstickApplied;
    public static event Action OnBlushApplied;
    public static event Action OnSpongeReset;

    public static void CreamApplied() => OnCreamApplied?.Invoke();
    public static void BrushPickup() => OnBrushPickup?.Invoke();
    public static void BrushApplied() => OnBrushApplied?.Invoke();
    public static void LipstickApplied() => OnLipstickApplied?.Invoke();
    public static void BlushApplied() => OnBlushApplied?.Invoke();
    public static void SpongeReset() => OnSpongeReset?.Invoke();
}
}
