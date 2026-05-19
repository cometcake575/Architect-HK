using System.Collections;
using UnityEngine.UI;
using SaveSlotButton = On.UnityEngine.UI.SaveSlotButton;

namespace Architect.Workshop.Items;

public class SceneGroup : SpriteItem
{
    public string GroupName = string.Empty;
    
    public static void Init()
    {
        SaveSlotButton.PresentSaveSlot += (orig, self, stats) =>
        {
            orig(self, stats);
            if (GlobalArchitectData.Instance.SaveSlotGroups.TryGetValue(self.SaveSlotIndex, out var value))
            {
                if (!SceneUtils.SceneGroups.TryGetValue(value, out var group)) return;
                self.background.sprite = group.Sprite;
                self.locationText.text = group.GroupName;
                ArchitectPlugin.Instance.StartCoroutine(SetText(self.locationText, group.GroupName));
            }

            return;

            IEnumerator SetText(Text text, string t)
            {
                yield return null;
                text.text = t;
            }
        };

        ModHooks.SavegameClearHook += i =>
        {
            GlobalArchitectData.Instance.SaveSlotGroups.Remove(i);
        };
        
        ModHooks.SavegameSaveHook += i =>
        {
            if (!SceneUtils.CustomScenes.TryGetValue(PlayerData.instance.respawnScene, out var scene))
            {
                GlobalArchitectData.Instance.SaveSlotGroups.Remove(i);
                return;
            }
            GlobalArchitectData.Instance.SaveSlotGroups[i] = scene.Group;
        };
    }
    
    public override void Register()
    {
        SceneUtils.SceneGroups.Add(Id, this);
        base.Register();
    }
    
    public override void Unregister()
    {
        SceneUtils.SceneGroups.Remove(Id);
    }
}