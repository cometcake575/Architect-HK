using System;
using HutongGames.PlayMaker;
using Satchel;
using Satchel.Futils;
using SFCore.Utils;
using FsmUtil = SFCore.Utils.FsmUtil;

namespace Architect.Utils;

public static class FsmUtils
{
    public static FsmState AddState(this PlayMakerFSM fsm, string state)
    {
        return SFCore.Utils.FsmUtil.AddState(fsm, state);
    }
    
    public static FsmState GetState(this PlayMakerFSM fsm, string state)
    {
        return fsm.Fsm.GetState(state);
    }
    
    public static void DisableAction(this FsmState state, int index)
    {
        state.Actions[index].Enabled = false;
    }
    
    public static void DisableActions(this FsmState state, params int[] indexes)
    {
        foreach (var index in indexes) state.Actions[index].Enabled = false;
    }

    public static void AddAction(this FsmState state, Action action, int index = -1)
    {
        if (index == -1) state.AddCustomAction(action);
        else state.InsertCustomAction(action, index);
    }
    
    public static void AddAction(this FsmState state, FsmStateAction customAction, int index = -1)
    {
        if (index == -1) FsmUtil.AddAction(state, customAction);
        else FsmUtil.InsertAction(state, customAction, index);
    }

    public class EveryFrameAction(Action method) : FsmStateAction
    {
        public override void OnEnter() => method();
        public override void OnUpdate() => method();
    }
}