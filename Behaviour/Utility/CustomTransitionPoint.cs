using System;
using System.Collections;
using Architect.Content.Preloads;
using Architect.Prefabs;
using Architect.Utils;
using GlobalEnums;
using UnityEngine;

namespace Architect.Behaviour.Utility;

public class CustomTransitionPoint : PreviewableBehaviour
{
    public int pointType;
    public bool applyInEditMode = true;

    public static void Init()
    {
        typeof(TransitionPoint).Hook(nameof(TransitionPoint.GetGatePosition),
            (Func<TransitionPoint, GatePosition> orig, TransitionPoint self) =>
            {
                if (!self) return GatePosition.unknown;
                var ctp = self.GetComponentInParent<CustomTransitionPoint>();
                return ctp ? ctp.GetGatePosition() : orig(self);
            });
        
        typeof(HeroController).Hook(nameof(HeroController.EnterScene), EnterSceneHook);
    }

    private static IEnumerator EnterSceneHook(Func<HeroController, TransitionPoint, float, IEnumerator> orig,
        HeroController self, TransitionPoint point, float delay)
    {
        if (point)
        {
            var ctp = point.GetComponentInParent<CustomTransitionPoint>();
            if (ctp) ctp.gameObject.BroadcastEvent("OnExit");
        }
        yield return orig(self, point, delay);
    }

    private void Start()
    {
        if (isAPreview && (PrefabManager.InPrefabScene || !applyInEditMode))
        {
            gameObject.SetActive(false);
        }
    }

    public GatePosition GetGatePosition()
    {
        return pointType switch
        {
            1 => GatePosition.left,
            2 => GatePosition.right,
            3 => GatePosition.top,
            4 => GatePosition.bottom,
            _ => GatePosition.door
        };
    }
}