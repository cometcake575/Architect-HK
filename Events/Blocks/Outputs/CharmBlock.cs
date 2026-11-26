using System.Collections.Generic;
using ItemChanger;
using ItemChanger.Items;

namespace Architect.Events.Blocks.Outputs;

public class CharmBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Equip", "Unequip"];
    protected override IEnumerable<(string, string)> OutputVars => [
        ("Unlocked", "Boolean"),
        ("Equipped", "Boolean")
    ];
    
    protected override string Name => "Charm Control";

    public string CharmName;

    protected override void Trigger(string id)
    {
        var item = Finder.GetItem(CharmName);
        
        int charmNum;
        switch (item)
        {
            case ItemChanger.Items.CharmItem charm:
                charmNum = charm.charmNum;
                break;
            case WhiteFragmentItem:
                charmNum = 36;
                break;
            default:
                return;
        }

        var pd = PlayerData.instance;

        if (!pd.GetBool($"gotCharm_{charmNum}")) return;

        var equip = id == "Equip";
        
        pd.SetBool($"equippedCharm_{charmNum}", equip);
        if (equip)
        {
            if (!pd.equippedCharms.Contains(charmNum)) pd.equippedCharms.Add(charmNum);
        }
        else pd.equippedCharms.Remove(charmNum);
        
        pd.CalculateNotchesUsed();
        
        pd.SetBool(nameof(PlayerData.overcharmed), 
            pd.GetInt(nameof(PlayerData.charmSlotsFilled)) > pd.GetInt(nameof(PlayerData.charmSlots)));
        
        HeroController.instance.CharmUpdate();
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
    }

    public override object GetValue(string id)
    {
        var item = Finder.GetItem(CharmName);
        var tag = id == "Equipped" ? "equipped" : "got";
        return item is ItemChanger.Items.CharmItem charm && PlayerData.instance.GetBool($"{tag}Charm_{charm.charmNum}");
    }
}
