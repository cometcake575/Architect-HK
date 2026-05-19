using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using UnityEngine;

namespace Architect.Events.Blocks.Outputs;

public class CharmBlock : ScriptBlock
{
    protected override IEnumerable<(string, string)> OutputVars => [
        ("Unlocked", "Boolean"),
        ("Equipped", "Boolean")
    ];
    
    protected override string Name => "Charm Control";

    public string CharmName;

    public override object GetValue(string id)
    {
        var item = Finder.GetItem(CharmName);
        var tag = id == "Equipped" ? "equipped" : "got";
        return item is ItemChanger.Items.CharmItem charm && PlayerData.instance.GetBool($"{tag}Charm_{charm.charmNum}");
    }
}
