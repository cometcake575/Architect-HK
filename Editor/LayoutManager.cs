using System.Collections.Generic;
using Satchel.BetterMenus;

namespace Architect.Editor;

public static class LayoutManager
{
    public static bool LegacyMode => GlobalArchitectData.LegacyMode;
    public static int UIMode => GlobalArchitectData.UIMode;
    
    public static MenuScreen GetMenuScreen(MenuScreen returnMenu)
    {
        var elements = new List<Element>
        {
            new HorizontalOption("Legacy Event Mode", "Use the Legacy event system and objects", ["False", "True"],
                a => GlobalArchitectData.LegacyMode = a == 1, () => LegacyMode ? 1 : 0),
            new HorizontalOption("Layout Mode", "How the UI should be structured", ["Modern", "Legacy", "Updated Legacy"],
                a => GlobalArchitectData.UIMode = a, () => GlobalArchitectData.UIMode)
        };

        return new Menu(
            name: "Layout Options",
            elements: elements.ToArray()
        ).GetMenuScreen(returnMenu);
    }
}