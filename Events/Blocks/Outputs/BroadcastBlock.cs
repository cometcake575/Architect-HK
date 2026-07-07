using System;
using System.Collections.Generic;
using System.Linq;
using Architect.Events.Blocks.Events;
using UnityEngine;

namespace Architect.Events.Blocks.Outputs;

public class BroadcastBlock : LocalBlock
{
    protected override IEnumerable<string> Inputs => ["Broadcast", "SetEvent"];
    protected override IEnumerable<(string, string)> InputVars => [("New Event", "Text")];
    
    protected override string Name => "Broadcast";

    public GameObject TargetPrefab;

    public string ActualEventName;
    public string EventName = string.Empty;

    protected override void Trigger(string id)
    {
        if (id == "Broadcast")
        {
            DoBroadcast(EventName);
            if (TargetPrefab) TargetPrefab.BroadcastEvent(ActualEventName);
        }
        else EventName = GetVariable<string>("New Event", "");
    }

    public static void DoBroadcast(string eventName)
    {
        foreach (var e in ReceiveBlock.RcEvent.Events
                     .Where(e => e.Block.EventName.Equals(eventName, StringComparison.InvariantCultureIgnoreCase)))
        {
            e.Block.Event("OnReceive");
        }
    } 
}