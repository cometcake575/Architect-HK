using System.Collections.Generic;
using ItemChanger;

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
        if (Finder.GetItem(CharmName) is not ItemChanger.Items.CharmItem charm) return;

        var pd = PlayerData.instance;

        if (!pd.GetBool($"gotCharm_{charm.charmNum}")) return;

        var equip = id == "Equip";
        
        pd.SetBool($"equippedCharm_{charm.charmNum}", equip);
        if (equip)
        {
            if (!pd.equippedCharms.Contains(charm.charmNum)) pd.equippedCharms.Add(charm.charmNum);
        }
        else pd.equippedCharms.Remove(charm.charmNum);
        
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
