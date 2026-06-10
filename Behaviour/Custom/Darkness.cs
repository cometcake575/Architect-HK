using System.Collections.Generic;
using System.Linq;
using Architect.Behaviour.Utility;
using Architect.Content.Custom;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Architect.Behaviour.Custom;

public class Darkness : PreviewableBehaviour
{
    private static readonly List<Darkness> DarknessObjects = [];

    public int amount;
    private static FsmInt _value;

    private void OnEnable()
    {
        if (isAPreview) return;
        DarknessObjects.Add(this);
        Refresh();
    }

    private void OnDisable()
    {
        if (isAPreview) return;
        DarknessObjects.Remove(this);
        Refresh();
    }

    public static void Init()
    {
        HookUtils.OnFsmAwake += fsm =>
        {
            if (fsm.FsmName == "Darkness Control")
            {
                _value = fsm.FsmVariables.FindFsmInt("Darkness Level");
            }
        };
    }

    private static void Refresh()
    {
        GameManager.instance.sm.darknessLevel = Mathf.Min(DarknessObjects.Sum(d => d.amount), 2);
        _value.Value = GameManager.instance.sm.darknessLevel;
        AbilityObjects.RefreshLanternBinding();
    }
}
