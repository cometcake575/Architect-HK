using UnityEngine;

namespace Architect.Behaviour.Fixers;

public static class InteractableFixers
{
    public static void FixSoulTotem(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("soul_totem");
        fsm.fsmTemplate = null;
        fsm.GetState("Hit").AddAction(() => obj.BroadcastEvent("OnHit"), 0);
    }

    public static void FixReusableLever(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Call Lever");

        fsm.FsmVariables.FindFsmGameObject("Lift").value = null;

        fsm.GetState("Check Already Called").AddAction(() =>
        {
            obj.BroadcastEvent("OnActivate");
            fsm.SendEvent("TRUE");
        }, 0);
    }
}