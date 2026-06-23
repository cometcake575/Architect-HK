using System.Collections.Generic;
using UnityEngine;

namespace Architect.Events.Blocks.Objects;

public class ObjectScalerBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => [
        "ScaleTo",
        "ScaleAdd",
        "ScaleMul"
    ];

    protected override IEnumerable<(string, string)> InputVars =>
    [
        Space,
        Space,
        ("Target", "Object"),
        ("X", "Number"),
        ("Y", "Number")
    ];
    
    protected override IEnumerable<(string, string)> OutputVars =>
    [
        ("Scale X", "Number"),
        ("Scale Y", "Number")
    ];
    
    public override Color Color => ObjectBlock.ValidColor;
    protected override string Name => "Scale Object";

    protected override void Trigger(string trigger)
    {
        var obj = GetVariable<GameObject>("Target");
        if (!obj) return;
        
        switch (trigger)
        {
            case "ScaleTo":
                var tx = GetVariable<float>("X", obj.transform.GetScaleX());
                var ty = GetVariable<float>("Y", obj.transform.GetScaleY());

                obj.transform.localScale = new Vector3(tx, ty, obj.transform.GetScaleZ());
                
                break;
            case "ScaleAdd":
                var ax = GetVariable<float>("X", 0);
                var ay = GetVariable<float>("Y", 0);

                obj.transform.localScale += new Vector3(ax, ay);
                
                break;
            case "ScaleMul":
                var mx = GetVariable<float>("X", 1);
                var my = GetVariable<float>("Y", 1);

                obj.transform.localScale = new Vector3(
                    obj.transform.GetScaleX() * mx,
                    obj.transform.GetScaleY() * my,
                    obj.transform.GetScaleZ());
                
                break;
        }
    }

    public override object GetValue(string id)
    {
        var obj = GetVariable<GameObject>("Target");
        if (!obj) return 1;

        return id == "Scale X" ? obj.transform.GetScaleX() : obj.transform.GetScaleY();
    }
}