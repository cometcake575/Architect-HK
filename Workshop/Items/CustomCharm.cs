using System;
using System.Collections;
using System.Collections.Generic;
using ItemChanger;
using ItemChanger.UIDefs;
using SFCore;
using UnityEngine;

namespace Architect.Workshop.Items;

public class CustomCharm : SpriteItem
{
    public string CharmName = string.Empty;
    public string CharmDesc = string.Empty;
    public int Cost = 0;
    
    private int _charmId;
    private bool _everRegistered;
    private bool _valid;

    private static readonly Dictionary<int, CustomCharm> Charms = [];
    private static bool _storedCharmPositions;

    public static void Init()
    {
        Dictionary<string, (Vector3, Vector3)> info = [];
        
        typeof(CharmHelper).Hook(nameof(CharmHelper.InitUiCharms),
            (Action orig) =>
            {
                if (!_storedCharmPositions)
                {
                    _storedCharmPositions = true;
                    var charms = GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms");
                    foreach (Transform child in charms.Find("Backboards")) 
                        info[child.name] = (child.localScale, child.localPosition);
                    foreach (Transform child in charms.Find("Collected Charms")) 
                        info[child.name] = (child.localScale, child.localPosition);
                }
                orig();
            });
        
        typeof(CharmHelper).Hook(nameof(CharmHelper.ClearModdedCharmsFromUiCharms),
            (Action orig) =>
            {
                if (_storedCharmPositions)
                {
                    var charms = GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms");
                    foreach (Transform child in charms.Find("Backboards"))
                    {
                        if (info.TryGetValue(child.name, out var sp))
                        {
                            var (scale, pos) = sp;
                            child.localScale = scale;
                            child.localPosition = pos;
                        }
                    }
                    foreach (Transform child in charms.Find("Collected Charms"))
                    {
                        if (info.TryGetValue(child.name, out var sp))
                        {
                            var (scale, pos) = sp;
                            child.localScale = scale;
                            child.localPosition = pos;
                        }
                    }
                }
                orig();
            });
        
        ModHooks.LanguageGetHook += (key, _, orig) =>
        {
            if (key.StartsWith("CHARM_NAME_"))
            {
                var charmNum = int.Parse(key.Split('_')[2]);
                if (Charms.TryGetValue(charmNum, out var value))
                {
                    return value._valid ? value.CharmName : "Deleted Charm";
                }
            }
            else if (key.StartsWith("CHARM_DESC_"))
            {
                var charmNum = int.Parse(key.Split('_')[2]);
                if (Charms.TryGetValue(charmNum, out var value))
                {
                    return value._valid ? value.CharmDesc : "This charm has been deleted.\n\n" +
                                                            "It will no longer appear once the game is restarted.";
                }
            }
            return orig;
        };

        ModHooks.GetPlayerBoolHook += (target, orig) =>
        {
            if (target.StartsWith("gotCharm_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm))
                {
                    return !charm._valid || ArchitectData.Instance.GotCharms.Contains(charm.Id);
                }
            }
            if (target.StartsWith("newCharm_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm))
                {
                    return ArchitectData.Instance.NewCharms.Contains(charm.Id);
                }
            }
            if (target.StartsWith("equippedCharm_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm))
                {
                    return ArchitectData.Instance.EquippedCharms.Contains(charm.Id);
                }
            }
            return orig;
        };

        ModHooks.SetPlayerBoolHook += (target, orig) =>
        {
            if (target.StartsWith("gotCharm_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm))
                {
                    if (orig) ArchitectData.Instance.GotCharms.Add(charm.Id);
                    else ArchitectData.Instance.GotCharms.Remove(charm.Id);
                    return orig;
                }
            }
            if (target.StartsWith("newCharm_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm))
                {
                    if (orig) ArchitectData.Instance.NewCharms.Add(charm.Id);
                    else ArchitectData.Instance.NewCharms.Remove(charm.Id);
                    return orig;
                }
            }
            if (target.StartsWith("equippedCharm_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm))
                {
                    if (orig) ArchitectData.Instance.EquippedCharms.Add(charm.Id);
                    else ArchitectData.Instance.EquippedCharms.Remove(charm.Id);
                    return orig;
                }
            }
            return orig;
        };

        ModHooks.GetPlayerIntHook += (target, orig) =>
        {
            if (target.StartsWith("charmCost_"))
            {
                var charmNum = int.Parse(target.Split('_')[1]);
                if (Charms.TryGetValue(charmNum, out var charm)) return charm._valid ? charm.Cost : 0;
            }
            return orig;
        };
    }

    private ItemChanger.Items.CharmItem _charmItem;
    private MsgUIDef _uiDef;
    
    public override void Register()
    {
        _valid = true;
        if (!_everRegistered)
        {
            _everRegistered = true;
            _charmId = CharmHelper.AddSprites(ArchitectPlugin.BlankSprite)[0];
            Charms[_charmId] = this;
        }

        _uiDef = new MsgUIDef
        {
            name = new BoxedString(CharmName),
            shopDesc = new BoxedString(CharmDesc),
            sprite = new BoxedSprite(ArchitectPlugin.BlankSprite)
        };
        _charmItem = new ItemChanger.Items.CharmItem
        {
            charmNum = _charmId,
            name = Id,
            UIDef = _uiDef
        };
        Finder.DefineCustomItem(_charmItem);

        base.Register();

        if (!_storedCharmPositions) return;
        if (!GameCameras.instance || !GameCameras.instance.hudCamera) return;
        var charms = GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms");
        ArchitectPlugin.Instance.StopCoroutine(_refreshRoutine);
        _refreshRoutine = DoRefresh(charms.gameObject);
        ArchitectPlugin.Instance.StartCoroutine(_refreshRoutine);
    }

    private static IEnumerator _refreshRoutine;

    private static IEnumerator DoRefresh(GameObject charms)
    {
        yield return new WaitForSeconds(2);
        if (!charms) yield break;
        CharmHelper.ClearModdedCharmsFromUiCharms();
        yield return new WaitUntil(() => !charms || charms.activeInHierarchy);
        if (!charms) yield break;
        CharmHelper.InitUiCharms();
    }

    public override void Unregister()
    {
        _valid = false;
        Finder.UndefineCustomItem(Id);
    }
    
    protected override void OnReadySprite()
    {
        _uiDef.sprite = new BoxedSprite(Sprite);
        CharmHelper.CustomSprites[_charmId - 41] = Sprite;
    }
}