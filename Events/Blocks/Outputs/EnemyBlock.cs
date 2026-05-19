using System.Collections.Generic;
using Architect.Utils;

namespace Architect.Events.Blocks.Outputs;

public class EnemyBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Damage", "Heal", "Set"];
    protected override IEnumerable<(string, string)> InputVars => [("Target", "Enemy"), ("Multiplier", "Number")];
    protected override IEnumerable<(string, string)> OutputVars => [("Path", "Text")];
    
    protected override string Name => "Enemy Control";

    public int Health;
    public AttackTypes AttackType;

    public override void Reset()
    {
        Health = 1;
        AttackType = AttackTypes.Generic;
    }

    public override object GetValue(string id)
    {
        var target = GetVariable<HealthManager>("Target");
        return target ? target.transform.GetPath() : "";
    }

    protected override void Trigger(string trigger)
    {
        var target = GetVariable<HealthManager>("Target");
        if (!target) return;
        switch (trigger)
        {
            case "Damage":
                target.Hit(
                    new HitInstance
                    {
                        Source = target.gameObject,
                        AttackType = AttackType,
                        DamageDealt = (int)(Health * GetVariable<float>("Multiplier", 1)),
                        SpecialType = SpecialTypes.None,
                        Multiplier = 1
                    });
                break;
            case "Heal":
                target.hp += (int)(Health * GetVariable<float>("Multiplier", 1));
                break;
            case "Set":
                target.hp = (int)(Health * GetVariable<float>("Multiplier", 1));
                break;
        }
    }
}