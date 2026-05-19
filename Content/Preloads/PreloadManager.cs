using System.Collections.Generic;

namespace Architect.Content.Preloads;

public static class PreloadManager
{
    public static bool HasPreloaded;
    public static readonly Dictionary<string, List<(string, IPreload)>> ToPreload = [];

    public static void RegisterPreload(IPreload obj)
    {
        if (!ToPreload.ContainsKey(obj.Scene)) ToPreload[obj.Scene] = [];
        ToPreload[obj.Scene].Add((obj.Path, obj));
    }
}
