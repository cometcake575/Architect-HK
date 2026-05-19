using System.Collections.Generic;

namespace Architect.Events.Blocks.Outputs;

public class ShakeCameraBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Shake"];
    
    protected override string Name => "Camera Shake";
    
    public int ShakeType;

    protected override void Trigger(string id)
    {
        GameCameras.instance.cameraShakeFSM.SendEvent(ShakeType switch
        {
            0 => "SmallShake",
            1 => "AverageShake",
            2 => "BigShake",
            _ => "HugeShake"
        });
    }
}
