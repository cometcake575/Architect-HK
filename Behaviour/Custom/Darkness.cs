using System.Collections.Generic;
using System.Linq;
using Architect.Content.Custom;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Architect.Behaviour.Custom;


public class Darkness : MonoBehaviour
{
    private static readonly List<Darkness> DarknessObjects = [];

    public int amount;
    private static FsmInt _value;

    private void OnEnable()
    {
        DarknessObjects.Add(this);
        Refresh();
    }

    private void OnDisable()
    {
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
