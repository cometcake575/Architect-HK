using System;
using HutongGames.PlayMaker;
using Satchel;

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

    public static void AddAction(this FsmState state, Action action, int index = -1)
    {
        if (index == -1) state.AddCustomAction(action);
        else state.InsertCustomAction(action, index);
    }
    
    public static void AddAction(this FsmState state, FsmStateAction customAction, int index = -1)
    {
        var actions = state.Actions;

        if (index == -1) index = actions.Length;
        
        var fsmStateActionArray = new FsmStateAction[actions.Length + 1];
        var index1 = 0;
        var index2 = 0;
        while (index1 < fsmStateActionArray.Length)
        {
            if (index1 == index)
            {
                fsmStateActionArray[index1] = customAction;
                ++index1;
            }
            if (index2 < actions.Length)
                fsmStateActionArray[index1] = actions[index2];
            ++index1;
            ++index2;
        }
        state.Actions = fsmStateActionArray;
        customAction.Init(state);
    }
}