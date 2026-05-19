namespace Architect.Multiplayer.Hkmp;

public static class HkmpManagerInitializer
{
    public static void Init()
    {
        CoopManager.Instance = new HkmpManager();
    }
}