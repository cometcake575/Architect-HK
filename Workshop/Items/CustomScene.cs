using UnityEngine;

namespace Architect.Workshop.Items;

public class CustomScene : WorkshopItem
{
    private static readonly Sprite Icon = ResourceUtils.LoadSpriteResource("door", FilterMode.Point);
    
    public string Group = "None";
    
    public int TilemapWidth = 500;
    public int TilemapHeight = 500;
    
    public int Environment = 0;

    public override void Register()
    {
        SceneUtils.CustomScenes[Id] = this;
    }

    public override void Unregister()
    {
        SceneUtils.CustomScenes.Remove(Id);
    }

    public override Sprite GetIcon() => Icon;
}