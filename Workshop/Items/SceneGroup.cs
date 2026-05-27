using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Architect.Storage;
using HutongGames.PlayMaker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using SaveSlotButton = On.UnityEngine.UI.SaveSlotButton;

namespace Architect.Workshop.Items;

public class SceneGroup : SpriteItem
{
    public string GroupName = string.Empty;

    public Vector3 Pos;
    public Vector3 TextPos;
    public Vector3 ZoomPos;
    public Vector3 CompassPos;
    public Vector3 AreaNamePos;

    public bool HasMap;
    public string Variable = string.Empty;
    
    public string MapUrl = string.Empty;
    public bool MPoint;
    public float MPpu = 100;

    public Sprite MapSprite;
    
    public override (string, string)[] FilesToDownload => [
        (IconUrl, "png"),
        (MapUrl, "png")
    ];
    
    public Color MapColour;
    
    public readonly List<string>[] DirectionalZones = [[], [], [], []];
    public readonly string[][] InverseDirectionalZones = [[], [], [], []];

    private MapUtils.MapZoneInfo _zoneInfo;
    
    public static void Init()
    {
        SaveSlotButton.PresentSaveSlot += (orig, self, stats) =>
        {
            orig(self, stats);
            if (GlobalArchitectData.Instance.SaveSlotGroups.TryGetValue(self.SaveSlotIndex, out var value))
            {
                if (!SceneUtils.SceneGroups.TryGetValue(value, out var group)) return;
                self.background.sprite = group.Sprite;
                self.locationText.text = group.GroupName;
                ArchitectPlugin.Instance.StartCoroutine(SetText(self.locationText, group.GroupName));
            }

            return;

            IEnumerator SetText(Text text, string t)
            {
                yield return null;
                text.text = t;
            }
        };

        ModHooks.SavegameClearHook += i =>
        {
            GlobalArchitectData.Instance.SaveSlotGroups.Remove(i);
        };
        
        ModHooks.SavegameSaveHook += i =>
        {
            if (!SceneUtils.CustomScenes.TryGetValue(PlayerData.instance.respawnScene, out var scene))
            {
                GlobalArchitectData.Instance.SaveSlotGroups.Remove(i);
                return;
            }
            GlobalArchitectData.Instance.SaveSlotGroups[i] = scene.Group;
        };
    }

    public static readonly List<SceneGroup> Groups = [];

    public override void Register()
    {
        SceneUtils.SceneGroups.Add(Id, this);
        base.Register();
        
        Groups.Add(this);
        SetupWidemap();
        SetupZone();
        if (HasMap) RefreshMapSprite();
    }

    private void RefreshMapSprite()
    {
        if (MapUrl.IsNullOrWhiteSpace()) return;
        CustomAssetManager.DoLoadSprite(MapUrl, MPoint, MPpu, 1, 1, sprites =>
        {
            if (sprites.IsNullOrEmpty()) return;
            MapSprite = sprites[0];
            if (_widemapRenderer) _widemapRenderer.sprite = MapSprite;
        });
    }
    
    public override void Unregister()
    {
        SceneUtils.SceneGroups.Remove(Id);
        Groups.Remove(this);
        
        if (_widemapObject) Object.Destroy(_widemapObject);
        MapUtils.UnregisterCustomMap(Id);
    }

    private GameObject _widemapObject;
    private SpriteRenderer _widemapRenderer;
    private GameObject _mapObject;
    
    private void SetupZone()
    {
        if (!HasMap) return;
        if (_mapObject) Object.Destroy(_mapObject);
        
        if (!GameCameras.instance) return;
        var gm = GameCameras.instance.GetComponentInChildren<GameMap>(true);
        if (!gm) return;

        _mapObject = new GameObject(Id)
        {
            transform =
            {
                parent = gm.transform,
                localPosition = ZoomPos,
                localScale = Vector3.one
            }
        };
        _mapObject.SetActive(false);
        
        var name = Object.Instantiate(
            gm.transform.Find("Crossroads").Find("Area Name (2)").gameObject, 
            _mapObject.transform);
        name.transform.localPosition = AreaNamePos - ZoomPos * 0.2307692308f;
        
        var set = name.GetComponent<SetTextMeshProGameText>();
        set.sheetName = "ArchitectMod";
        set.convName = GroupName;
        var tm = set.GetComponent<TextMeshPro>();
        tm.text = GroupName;
        tm.color = MapColour;
        tm.margin = Vector4.zero;
        tm.textContainer.anchorPosition = TextContainerAnchors.Top;

        foreach (var scene in SceneUtils.CustomScenes.Values.Where(s => s.Group == Id)) 
            scene.SetupMap(this);
    }
    
    private void SetupWidemap()
    {
        if (!HasMap) return;
        if (_widemapObject) Object.Destroy(_widemapObject);
        MapUtils.UnregisterCustomMap(Id);
        
        if (!GameCameras.instance) return;

        var wide = GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Map")
            .Find("World Map").Find("Wide Map");
        
        _widemapObject = Object.Instantiate(wide.Find("Town").gameObject, wide);
        _widemapObject.RemoveComponent<MapUtils.MapZoneReference>();
        _widemapObject.name = Id;
        _widemapObject.transform.localPosition = Pos + new Vector3(4.2f, -7.61f);
        _widemapObject.transform.GetChild(0).localPosition += TextPos;

        _widemapObject.AddComponent<SceneGroupReference>().Group = this;

        _widemapRenderer = _widemapObject.GetComponent<SpriteRenderer>();
        _widemapRenderer.color = MapColour;
        _widemapRenderer.sprite = MapSprite;
        
        var set = _widemapObject.transform.GetChild(0).GetComponent<SetTextMeshProGameText>();
        set.sheetName = "ArchitectMod";
        set.convName = GroupName;
        set.GetComponent<TextMeshPro>().text = GroupName;

        _zoneInfo = new MapUtils.MapZoneInfo
        {
            Id = Id,
            ZoomToPos = -ZoomPos.Where(z: 22),
            EventTarget = new FsmEventTarget
            {
                gameObject = new FsmOwnerDefault
                {
                    gameObject = _widemapObject,
                    ownerOption = OwnerDefaultOption.SpecifyGameObject
                },
                sendToChildren = true,
                target = FsmEventTarget.EventTarget.GameObject
            },
            DirectionalZones = DirectionalZones,
            InverseDirectionalZones = InverseDirectionalZones,
            CustomGroup = this
        };
        MapUtils.RegisterCustomMap(_zoneInfo);
    }

    public static void RefreshMaps()
    {
        foreach (var group in Groups)
        {
            group.SetupWidemap();
            group.SetupZone();
        }
    }
    
    public bool ShowMap()
    {
        return HasMap && (Variable.IsNullOrWhiteSpace() ||
               ArchitectData.Instance.BoolVariables.GetValueOrDefault(Variable));
    }
    
    public GameObject GetGameMap()
    {
        return _mapObject;
    }

    private class SceneGroupReference : MonoBehaviour
    {
        public SceneGroup Group;

        private void OnEnable()
        {
            if (Group == null || !Group.ShowMap()) gameObject.SetActive(false);
        }
    }

    public static void HideMaps()
    {
        foreach (var group in Groups.Where(g => g.HasMap))
        {
            var gm = group.GetGameMap();
            if (!gm) continue;
            gm.SetActive(false);
        }
    }

    public static void ShowMaps(GameMap map)
    {
        foreach (var group in Groups.Where(g => g.HasMap))
        {
            var gm = group.GetGameMap();
            if (!gm) continue;
            gm.SetActive(group.ShowMap());

            foreach (var scene in SceneUtils.CustomScenes.Values.Where(s => s.Group == group.Id))
            {
                var gmp = scene.GetGameMapPiece();
                map.panMinX = Mathf.Min(-gmp.transform.localPosition.x - gm.transform.localPosition.x - 
                                        group.ZoomPos.x * 0.2307692308f, map.panMinX);
                map.panMaxX = Mathf.Max(-gmp.transform.localPosition.x - gm.transform.localPosition.x + 
                                        group.ZoomPos.x * 0.2307692308f, map.panMaxX);
                map.panMinY = Mathf.Min(-gmp.transform.localPosition.y - gm.transform.localPosition.y - 
                                        group.ZoomPos.y * 0.2307692308f, map.panMinY);
                map.panMaxY = Mathf.Max(-gmp.transform.localPosition.y - gm.transform.localPosition.y + 
                                        group.ZoomPos.y * 0.2307692308f, map.panMaxY);
            }
        }
    }
}