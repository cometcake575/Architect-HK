using System.Collections.Generic;

namespace Architect.Events.Blocks.Outputs;

public class JournalEntryBlock : ScriptBlock
{
    protected override IEnumerable<string> Inputs => ["Increment", "Complete"];
    protected override IEnumerable<(string, string)> OutputVars => [
        ("Remaining", "Number")
    ];
    
    protected override string Name => "Journal Entry Control";

    public override void Reset()
    {
        EntryName = "";
    }

    public string EntryName;
    
    protected override void Trigger(string trigger)
    {
        var all = trigger == "Complete";
        
        var pd = GameManager.instance.playerData;
        var killedName = "killed" + EntryName;
        var killsName = "kills" + EntryName;
        var newDataName = "newData" + EntryName;
        var newKill = false;
        if (!pd.GetBool(killedName))
        {
            newKill = true;
            pd.SetBool(killedName, true);
            pd.SetBool(newDataName, true);
        }
        var newFill = false;
        var oldVal = pd.GetInt(killsName);
        if (oldVal > 0)
        {
            var newVal = all ? 0 : oldVal - 1;
            pd.SetInt(killsName, newVal);
            if (newVal <= 0) newFill = true;
        }
        if (!pd.GetBool("hasJournal"))
            return;
        var updated = false;
        if (newFill)
        {
            updated = true;
            pd.SetIntSwappedArgs(pd.GetInt("journalEntriesCompleted") + 1, "journalEntriesCompleted");
        }
        else if (newKill)
        {
            updated = true;
            pd.SetIntSwappedArgs(pd.GetInt("journalNotesCompleted") + 1, "journalNotesCompleted");
        }
        if (!updated) return;
        if (EnemyDeathEffects.journalUpdateMessageSpawned)
        {
            if (EnemyDeathEffects.journalUpdateMessageSpawned.activeSelf)
                EnemyDeathEffects.journalUpdateMessageSpawned.SetActive(false);
            EnemyDeathEffects.journalUpdateMessageSpawned.SetActive(true);
            var fsmOnGameObject = PlayMakerFSM.FindFsmOnGameObject(EnemyDeathEffects.journalUpdateMessageSpawned, "Journal Msg");
            if (!fsmOnGameObject) return;
            FSMUtility.SetBool(fsmOnGameObject, "Full", newFill);
            FSMUtility.SetBool(fsmOnGameObject, "Should Recycle", true);
        }
    }

    public override object GetValue(string id)
    {
        return PlayerData.instance.GetInt("kills" + EntryName);
    }
}
