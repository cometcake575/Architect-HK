using System;
using System.Collections.Generic;
using ItemChanger;
using ItemChanger.UIDefs;

namespace Architect.Workshop.Items;

public class CustomItem : SpriteItem
{
    private static readonly Dictionary<string, CustomItem> Lookup = [];
    
    public string Name = string.Empty;
    public string Desc = string.Empty;
    public bool HasCount;

    public static void Init()
    {
        ModHooks.GetPlayerIntHook += (name, orig) =>
        {
            if (name.StartsWith("ARCHITECT_ITEMCOUNT_"))
            {
                var id = name.Split('_')[2];
                return ArchitectData.Instance.CustomItems.TryGetValue(id, out var value) ? value : 0;
            }
            
            return orig;
        };
        
        ModHooks.SetPlayerIntHook += (name, orig) =>
        {
            if (name.StartsWith("ARCHITECT_ITEMCOUNT_"))
            {
                var id = name.Split('_')[2];
                ArchitectData.Instance.CustomItems[id] = orig;
            }
            
            return orig;
        };
        
        ModHooks.LanguageGetHook += (key, _, orig) =>
        {
            if (key.StartsWith("INV_"))
            {
                var ks = key.Split('_');
                if (Lookup.TryGetValue(ks[2], out var item))
                {
                    return ks[1] == "NAME" ? item.Name : item.Desc;
                }
            }
            
            return orig;
        };
    }
    
    private ItemCreator.CustomItem _icItem;
    private MsgUIDef _uiDef;
    private AbstractItem _item;
    
    public override void Register()
    {
        Lookup.Add(Id, this);

        _icItem = new ItemCreator.CustomItem(
            ArchitectPlugin.BlankSprite,
            HasCount, 
            Id, 
            $"ARCHITECT_ITEMCOUNT_{Id}");
        _icItem.Register();
        
        _uiDef = new MsgUIDef
        {
            name = new BoxedString(Name),
            shopDesc = new BoxedString(Desc),
            sprite = new BoxedSprite(ArchitectPlugin.BlankSprite)
        };
        _item = new IcCustomItem
        {
            name = Id,
            UIDef = _uiDef,
            Item = this
        };
        Finder.DefineCustomItem(_item);
        
        base.Register();
    }

    public override void Unregister()
    {
        Lookup.Remove(Id);

        _icItem.Unregister();
        Finder.UndefineCustomItem(_item.name);
    }

    protected override void OnReadySprite()
    {
        _icItem.SetSprite(Sprite);
        _uiDef.sprite = new BoxedSprite(Sprite);
    }
    
    public class IcCustomItem : AbstractItem
    {
        public CustomItem Item;
        
        public override void GiveImmediate(GiveInfo info)
        {
            ArchitectData.Instance.CustomItems[Item.Id] =
                (ArchitectData.Instance.CustomItems.TryGetValue(Item.Id, out var val) ? val : 0) + 1;
        }

        public void Take()
        {
            ArchitectData.Instance.CustomItems[Item.Id] =
                Math.Max(0, (ArchitectData.Instance.CustomItems.TryGetValue(Item.Id, out var val) ? val : 0) - 1);
        }
    }
}