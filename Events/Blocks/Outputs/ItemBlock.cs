using System.Collections.Generic;
using Architect.Utils;
using ItemChanger;
using ItemChanger.UIDefs;
using SFCore;
using UnityEngine;

namespace Architect.Events.Blocks.Outputs;

public class ItemBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Give", "GiveSilent"];
    protected override IEnumerable<(string, string)> OutputVars => [
        ("Obtained", "Boolean")
    ];
    
    protected override string Name => "Item Control";

    public override void Reset()
    {
        ItemName = "";
    }

    public string ItemName;

    public override object GetValue(string id)
    {
        var item = Finder.GetItem(ItemName);
        return item != null && item.IsObtained();
    }

    protected override void Trigger(string trigger)
    {
        var item = Finder.GetItem(ItemName);
        item?.Give(null, new GiveInfo
        {
            MessageType = trigger == "Give" ? MessageType.Any : MessageType.None
        });
    }
}