using System.Collections.Generic;
using Architect.Content.Preloads;
using UnityEngine;

namespace Architect.Events.Blocks.Outputs;

public class StatueBlock : ScriptBlock
{
    protected override string Name => "Statue UI";

    protected override IEnumerable<(string, string)> InputVars => [("Radiant", "Boolean")];

    public string Title = string.Empty;
    public string Desc = string.Empty;

    private static GameObject _statuePrefab;
    private static GameObject _prefab;

    public static void Init()
    {
        PreloadManager.RegisterPreload(new BasicPreload("GG_Workshop", "GG_Statue_Vengefly",
            o =>
            {
                _statuePrefab = o;
                _prefab = ((ShowBossChallengeUI)o.transform.Find("Inspect").gameObject.LocateMyFSM("GG Boss UI")
                    .GetState("Open UI").Actions[1]).prefab.Value;
            }));
    }

    private BossStatue _statue;

    public override void SetupReference()
    {
        var s = Object.Instantiate(_statuePrefab);
        s.SetActive(false);
        _statue = s.GetComponent<BossStatue>();
    }

    protected override void Trigger(string trigger)
    {
        if (!ShowBossChallengeUI.spawnedUI)
        {
            ShowBossChallengeUI.spawnedUI = Object.Instantiate(_prefab);
            ShowBossChallengeUI.spawnedUI.SetActive(false);
        }

        if (ShowBossChallengeUI.spawnedUI)
        {
            var spawnedUi = ShowBossChallengeUI.spawnedUI;
            spawnedUi.transform.position = _prefab.transform.position;
            spawnedUi.SetActive(true);
            var ui = spawnedUi.GetComponent<BossChallengeUI>();
            if (ui)
            {
                BossChallengeUI.HideEvent temp1 = null;
                temp1 = () => { ui.OnCancel -= temp1; };
                ui.OnCancel += temp1;
                BossChallengeUI.LevelSelectedEvent temp2 = null;
                temp2 = () =>
                {
                    Event("OnSelect");
                    ui.OnLevelSelected -= temp2;
                };
                ui.OnLevelSelected += temp2;
                ui.Setup(_statue, 
                    "ArchitectMod", Title,
                    "ArchitectMod", Desc);
            }
        }
    }
}