using System.Collections.Generic;
using GlobalEnums;

namespace Architect.Events.Blocks.Outputs;

public class UIBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["HudIn", "HudOut", "FadeIn", "FadeOut"];

    protected override string Name => "UI Control";
    
    protected override void Trigger(string id)
    {
        switch (id)
        {
            case "HudIn":
                GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");
                break;
            case "HudOut":
                GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("OUT");
                break;
            case "FadeIn":
                GameCameras.instance.cameraController.FadeSceneIn();
                break;
            case "FadeOut":
                GameCameras.instance.cameraController.FadeOut(CameraFadeType.LEVEL_TRANSITION);
                break;
        }
    }
}
