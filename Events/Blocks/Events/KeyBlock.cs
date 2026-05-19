using System.Collections.Generic;
using Architect.Utils;
using Architect.Workshop.Items;
using InControl;
using UnityEngine;

namespace Architect.Events.Blocks.Events;

public class KeyBlock : ToggleableBlock
{
    protected override IEnumerable<string> Outputs => ["OnPress", "OnRelease"];
    protected override IEnumerable<string> Inputs => ["Disable", "Enable"];
    protected override IEnumerable<(string, string)> OutputVars => [("Held", "Boolean")];
    protected override string Name => "Key Listener";

    public KeyCode Key = KeyCode.None;
    public PlayerAction PlayerAction = null;

    public override void SetupReference()
    {
        var te = new GameObject("[Architect] Key Listener Block").AddComponent<KeyEvent>();
        te.Block = this;
    }
    
    public override object GetValue(string id)
    {
        return Input.GetKey(Key) || (PlayerAction?.IsPressed ?? false);
    }

    public class KeyEvent : MonoBehaviour
    {
        public KeyBlock Block;
        
        private void Update()
        {
            if (Input.GetKeyDown(Block.Key) || (Block.PlayerAction?.WasPressed ?? false)) Block.Event("OnPress");
            if (Input.GetKeyUp(Block.Key) || (Block.PlayerAction?.WasReleased ?? false)) Block.Event("OnRelease");
        }
    }
}