using System.Collections.Generic;
using UnityEngine;

namespace Architect.Events.Blocks.Operators;

public class PlayerDataBoolBlock : ScriptBlock
{
    private static readonly Dictionary<string, List<BoolBlockRef>> BlockRefs = [];

    protected override IEnumerable<string> Inputs => ["Set"];
    protected override IEnumerable<(string, string)> OutputVars => [("Value", "Boolean")];

    
    
    protected override string Name => "PlayerData Control (Bool)";

    public string Data = string.Empty;
    public bool Value;
    
    public override void SetupReference()
    {
        var blockRef = new GameObject("[Architect] Bool Block");
        var blockRefInst = blockRef.AddComponent<BoolBlockRef>();
        blockRefInst.Block = this;
        if (!BlockRefs.ContainsKey(Data)) BlockRefs[Data] = [];
        BlockRefs[Data].Add(blockRefInst);
    }

    public class BoolBlockRef : MonoBehaviour
    {
        public PlayerDataBoolBlock Block;

        private void OnDisable()
        {
            BlockRefs[Block.Data].Remove(this);
        }
    }

    protected override void Trigger(string trigger)
    {
        PlayerData.instance.SetBool(Data, Value);
    }

    public override object GetValue(string id)
    {
        return PlayerData.instance.GetBool(Data);
    }
}

public class PlayerDataIntBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Set", "Add", "Subtract"];
    protected override IEnumerable<(string, string)> OutputVars => [("Value", "Number")];

    
    
    protected override string Name => "PlayerData Control (Int)";

    public string Data;
    public int Value;

    protected override void Trigger(string trigger)
    {
        switch (trigger)
        {
            case "Set":
                PlayerData.instance.SetInt(Data, Value);
                break;
            case "Add":
                PlayerData.instance.SetInt(Data, PlayerData.instance.GetInt(Data) + Value);
                break;
            case "Subtract":
                PlayerData.instance.SetInt(Data, PlayerData.instance.GetInt(Data) - Value);
                break;
        }
    }

    public override object GetValue(string id)
    {
        return PlayerData.instance.GetInt(Data);
    }
}

public class PlayerDataFloatBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Set", "Add", "Subtract"];
    protected override IEnumerable<(string, string)> OutputVars => [("Value", "Number")];

    
    
    protected override string Name => "PlayerData Control (Float)";

    public string Data;
    public float Value;

    protected override void Trigger(string trigger)
    {
        switch (trigger)
        {
            case "Set":
                PlayerData.instance.SetFloat(Data, Value);
                break;
            case "Add":
                PlayerData.instance.SetFloat(Data, PlayerData.instance.GetFloat(Data) + Value);
                break;
            case "Subtract":
                PlayerData.instance.SetFloat(Data, PlayerData.instance.GetFloat(Data) - Value);
                break;
        }
    }

    public override object GetValue(string id)
    {
        return PlayerData.instance.GetFloat(Data);
    }
}
