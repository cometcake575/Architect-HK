using System;
using System.Collections.Generic;
using Satchel.BetterMenus;

namespace Architect.Storage;

public static class ProjectManager
{
    private static Menu _menuRef;

    private static readonly List<Element> Elements = [];

    public static MenuScreen GetMenuScreen(MenuScreen returnMenu)
    {
        _menuRef = new Menu(
            name: "Architect Projects", 
            elements: Elements.ToArray()
        );
        
        return _menuRef.GetMenuScreen(returnMenu);
    }

    public static void Init()
    {
        var current = "";
        Elements.Add(new InputField("Project Name", s => current = s, () => ""));

        var index = 0;
        var choice = new HorizontalOption("Project To Load", "", ["None"], 
            i => index = i, () => 0);
        choice.OnBuilt += UpdateValues;
        
        Elements.Add(new MenuButton("Save", "", _ =>
        {
            if (current.IsNullOrWhiteSpace() || current == "None") return;
            if (!GlobalArchitectData.Instance.SavedMapNames.Contains(current))
                GlobalArchitectData.Instance.SavedMapNames.Add(current);
            StorageManager.MakeBackup(current);
            StorageManager.MakeBackup(DateTime.Now.ToString("yy-MM-dd-HH-mm-ss"));
            UpdateValues();
        }));
        
        Elements.Add(choice);
        Elements.Add(new MenuButton("Load", "", _ =>
        {
            if (index >= choice.Values.Length) return;
            var v = choice.Values[index];
            if (v == "None" || v.IsNullOrWhiteSpace()) return;
            StorageManager.LoadBackup(v);
        }));
        
        Elements.Add(new MenuButton("Delete", "", _ =>
        {
            if (index >= choice.Values.Length) return;
            var v = choice.Values[index];
            if (v == "None" || v.IsNullOrWhiteSpace()) return;
            StorageManager.DeleteBackup(v);
            UpdateValues();
        }));

        return;

        void UpdateValues()
        {
            choice.Values = GlobalArchitectData.Instance.SavedMapNames.IsNullOrEmpty()
                ? ["None"]
                : GlobalArchitectData.Instance.SavedMapNames.ToArray();
            choice.Update();
        }
    }
}