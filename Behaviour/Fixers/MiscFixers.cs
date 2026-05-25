using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Architect.Behaviour.Custom;
using Architect.Content.Preloads;
using Architect.Events.Blocks.Operators;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace Architect.Behaviour.Fixers;

public static class MiscFixers
{
    public static Material SpriteMaterial;

    // Dive Coffin
    private static GameObject _coffinFloor;

    // Conveyors
    private static GameObject _conveyorBlock;
    private static readonly Dictionary<ConveyorMovement, List<ConveyorBelt>> CurrentBelts = [];
    private static readonly List<ConveyorBelt> CurrentVerticalBelts = [];

    public static void Init()
    {
        PreloadManager.RegisterPreload(new BasicPreload("Tutorial_01", "_Scenery/plat_float_01",
            o => { SpriteMaterial = o.GetComponent<SpriteRenderer>().material; }));

        PreloadManager.RegisterPreload(new BasicPreload("RestingGrounds_05", "Quake Floor",
            o => _coffinFloor = o));

        PreloadManager.RegisterPreload(new BasicPreload("Mines_31", "Conveyor Block",
            o => _conveyorBlock = o));

        ModHooks.LanguageGetHook += (key, title, orig) =>
            title == "ArchitectMod" ? SubstituteVars(key) : orig;

        On.HeroController.SceneInit += (orig, self) =>
        {
            orig(self);
            CurrentVerticalBelts.Clear();
            CurrentBelts.Clear();
            self.GetComponent<ConveyorMovementHero>().StopConveyorMove();
            HeroController.instance.cState.onConveyor = false;
            HeroController.instance.cState.onConveyorV = false;
        };

        On.ConveyorBelt.OnCollisionExit2D += (orig, self, collision) =>
        {
            if (self.vertical)
            {
                if (collision.gameObject.GetComponent<HeroController>())
                {
                    CurrentVerticalBelts.Remove(self);
                    if (CurrentVerticalBelts.Count > 0) return;
                }
            }
            else
            {
                var move = collision.gameObject.GetComponent<ConveyorMovement>();
                if (move)
                    if (CurrentBelts.TryGetValue(move, out var group))
                    {
                        group.Remove(self);
                        if (group.Count > 0) return;
                    }
            }

            orig(self, collision);
        };

        On.ConveyorBelt.OnCollisionEnter2D += (orig, self, collision) =>
        {
            orig(self, collision);
            if (self.vertical)
            {
                if (collision.gameObject.GetComponent<HeroController>()) CurrentVerticalBelts.Add(self);
            }
            else
            {
                var move = collision.gameObject.GetComponent<ConveyorMovement>();
                if (move)
                {
                    if (!CurrentBelts.Keys.Contains(move)) CurrentBelts[move] = [self];
                    else CurrentBelts[move].Add(self);
                }
            }
        };
        
        On.GameManager.FindEntryPoint += (orig, self, name, scene) =>
        {
            var point = orig(self, name, scene);
            if (!point.HasValue)
            {
                var hrm = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
                    .SelectMany(obj => obj.GetComponentsInChildren<HazardRespawnMarker>(true))
                    .First();
                return hrm.transform.position;
            }

            return point;
        };

        On.HeroController.LocateSpawnPoint += (orig, self) =>
        {
            var point = orig(self);
            if (!point)
            {
                var hrm = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
                    .SelectMany(obj => obj.GetComponentsInChildren<HazardRespawnMarker>(true))
                    .First();
                return hrm.transform;
            }

            return point;
        };
    }

    private static string SubstituteVars(string txt)
    {
        return Regex.Replace(txt, @"\[\[(.*?)\]\]", match =>
        {
            var key = match.Groups[1].Value;
            return StringVarBlock.GetVar(key);
        });
    }

    public class ColorLock : MonoBehaviour
    {
        public bool permanent;
    }

    public class TriggerActivator : MonoBehaviour
    {
        public int layer;
    }

    public static void KeepActive(GameObject obj)
    {
        obj.RemoveComponent<DeactivateIfPlayerdataFalse>();
        obj.RemoveComponent<DeactivateIfPlayerdataTrue>();
    }

    public static void AddComponent<T>(GameObject obj) where T : MonoBehaviour
    {
        obj.AddComponent<T>();
    }

    public class Npc : MonoBehaviour
    {
        public string dialogue;
    }

    public static void FixTiso(GameObject obj)
    {
        obj.AddComponent<Tiso>();
        KeepActive(obj);
    }

    public class Tiso : Npc
    {
        private void Start()
        {
            var cc = gameObject.LocateMyFSM("Conversation Control");
            cc.GetState("Convo Choice").AddAction(() => cc.SendEvent("REPEAT"), 0);
            var p = ((CallMethodProper)cc.GetState("Repeat").Actions[1]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            cc.GetState("Talk Finish").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public class Elderbug : Npc
    {
        private void Start()
        {
            var cc = gameObject.LocateMyFSM("Conversation Control");
            cc.GetState("Grimm?").DisableAction(0);
            cc.GetState("Convo Choice").AddAction(() => cc.SendEvent("GENERIC"), 0);
            cc.GetState("Generic Choice").AddAction(() => cc.SendEvent("FINISHED"), 0);
            var p = ((CallMethodProper)cc.GetState("Normal").Actions[0]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            cc.GetState("Talk Finish").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public class Quirrel : Npc
    {
        private void Start()
        {
            var cc = gameObject.LocateMyFSM("Conversation Control");

            cc.GetState("Convo Choice").AddAction(() => cc.SendEvent("TEMPLE"), 0);
            cc.GetState("Egg Temple").AddAction(() => cc.SendEvent("EGG 1"), 0);

            var et1 = cc.GetState("Egg Temple 1");
            et1.DisableAction(0);

            var p = ((CallMethodProper)et1.Actions[4]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            cc.GetState("End").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public class Hornet : Npc
    {
        private void Start()
        {
            var cc = gameObject.LocateMyFSM("Conversation Control");

            cc.GetState("Choice").AddAction(() => cc.SendEvent("REPEAT"), 1);

            var p = ((CallMethodProper)cc.GetState("Repeat").Actions[1]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            cc.GetState("End").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public class Midwife : Npc
    {
        private void Start()
        {
            var go = transform.GetChild(1).gameObject;

            var cc = go.LocateMyFSM("Conversation Control");

            cc.GetState("Convo Choice").AddAction(() => cc.SendEvent("FINISHED"), 1);

            var p = ((CallMethodProper)cc.GetState("Repeat").Actions[0]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            cc.GetState("End").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public static void FixZote(GameObject obj)
    {
        obj.AddComponent<Zote>();
        obj.RemoveComponent<DeactivateIfPlayerdataTrue>();
    }

    public class Zote : Npc
    {
        private PlayMakerFSM _cc;

        private void Awake()
        {
            _cc = gameObject.LocateMyFSM("Conversation Control");
            _cc.GetState("Check Active").AddAction(() => _cc.SendEvent("FINISHED"), 0);
        }

        private void Start()
        {
            _cc.GetState("Convo Choice").AddAction(() => _cc.SendEvent("REPEAT"), 6);
            var p = ((CallMethodProper)_cc.GetState("Repeat").actions[1]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            _cc.GetState("Talk Finish").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public class Godseeker : Npc
    {
        private void Start()
        {
            var cc = gameObject.LocateMyFSM("Conversation Control");

            var revisitCell = cc.FsmVariables.FindFsmString("Revisit Cell");
            var gts = cc.FsmVariables.FindFsmString("GameText Sheet");
            cc.GetState("Revisit?").AddAction(() =>
            {
                revisitCell.value = dialogue;
                gts.value = "ArchitectMod";
                cc.SendEvent("REVISIT");
            }, 2);

            cc.GetState("End").AddAction(() => gameObject.BroadcastEvent("OnFinish"), 0);
        }
    }

    public static void RotateBench(GameObject obj, float rot)
    {
        obj.transform.SetRotation2D(rot);
        if (rot == 0) return;
        var fsm = obj.GetComponentsInChildren<PlayMakerFSM>()
            .First(fsm => fsm.FsmName == "Bench Control");
        fsm.FsmVariables.FindFsmBool("Tilter").Value = true;
        obj.AddComponent<RestBenchTilt>().tilt = rot;
    }

    public class Tablet : Npc
    {
        private void Start()
        {
            var fsm = gameObject.LocateMyFSM("Inspection");

            fsm.FsmVariables.GetFsmString("Convo Name").Value = dialogue;
            fsm.FsmVariables.GetFsmString("Sheet Name").Value = "ArchitectMod";
        }
    }

    public static void FixRotation(GameObject obj)
    {
        obj.transform.SetRotation2D(0);
    }

    public static void BreakableZ(GameObject obj)
    {
        obj.transform.SetPositionZ(0.015f);
    }

    public static void FixGrubBottle(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Bottle Control");

        fsm.GetState("Destroy Self").AddAction(() =>
        {
            obj.BroadcastEvent("OnBreak");
            obj.BroadcastEvent("FirstBreak");
        }, 0);
        fsm.GetState("Activate").AddAction(() =>
        {
            obj.BroadcastEvent("OnBreak");
            obj.BroadcastEvent("LoadedBroken");
        }, 0);
    }

    public static void FixLever(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Switch Control");
        fsm.GetState("Open").AddAction(() =>
        {
            obj.BroadcastEvent("OnPull");
            obj.BroadcastEvent("FirstPull");
        });
        var activated = fsm.GetState("Activated");
        activated.transitions = [];
        activated.AddAction(() =>
        {
            obj.BroadcastEvent("OnPull");
            obj.BroadcastEvent("LoadedPulled");
        });
        (fsm.FsmVariables.FindFsmGameObject("Target") ?? fsm.FsmVariables.FindFsmGameObject("Target 1"))
            .value = null;
        fsm.FsmVariables.FindFsmString("Player Data").value = "";
    }

    public static void FixChest(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Chest Control");
        fsm.GetState("Opened").AddAction(() =>
        {
            obj.BroadcastEvent("OnOpen");
            obj.BroadcastEvent("FirstOpen");
        });
        fsm.GetState("Activated").AddAction(() =>
        {
            obj.BroadcastEvent("OnOpen");
            obj.BroadcastEvent("LoadedOpen");
        });
    }

    public static void FixToll(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Toll Machine");
        fsm.fsm.globalTransitions = [];
        fsm.GetState("Activated").AddAction(() =>
        {
            obj.BroadcastEvent("OnPay");
            obj.BroadcastEvent("LoadedPaid");
        }, 0);
        var og = fsm.GetState("Open Gates");
        og.DisableAction(1);
        og.AddAction(() =>
        {
            obj.BroadcastEvent("OnPay");
            obj.BroadcastEvent("FirstPay");
        }, 0);

        obj.AddComponent<Toll>().fsm = fsm;
    }

    public class Toll : Npc
    {
        public PlayMakerFSM fsm;

        public void Start()
        {
            var p = ((CallMethodProper)fsm.GetState("Send Text").actions[2]).parameters;
            p[0].stringValue = dialogue;
            p[1].stringValue = "ArchitectMod";

            fsm.FsmVariables.FindFsmString("Prompt Convo").value = dialogue;
        }
    }

    public static void FixNpc<T>(GameObject obj) where T : Npc
    {
        obj.AddComponent<T>();

        foreach (var fsm in obj.GetComponents<PlayMakerFSM>())
            if (fsm.FsmName == "FSM")
                fsm.enabled = false;
    }

    public static void FlipMidwife(GameObject obj, bool flip)
    {
        if (!flip) return;
        obj.transform.SetScaleX(-obj.transform.GetScaleX());

        obj.transform.GetChild(1).gameObject.LocateMyFSM("npc_control").FsmVariables
            .FindFsmFloat("Move To Offset")
            .Value = -3;
    }

    public static void FixDiveCoffin(GameObject obj)
    {
        var floor = Object.Instantiate(_coffinFloor, obj.transform);
        floor.SetActive(true);
        floor.transform.localPosition = Vector3.zero;
        floor.name = obj.name + " Floor";
    }

    public static void FixBreakableWall(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            if (child.name == "Masks") obj.SetActive(false);
        }
    }

    public class WpLift : MonoBehaviour
    {
        public Vector2 move;

        public void DoRise(PlayMakerFSM fsm, Vector2 end)
        {
            StartCoroutine(Rise(fsm, end));
        }

        private IEnumerator Rise(PlayMakerFSM fsm, Vector2 end)
        {
            var yMoved = false;
            var xMoved = false;

            while (!xMoved || !yMoved)
            {
                var pos = fsm.transform.position;

                yMoved = move.y == 0 ||
                         (move.y > 0 && pos.y >= end.y) ||
                         (move.y < 0 && pos.y <= end.y);

                xMoved = move.x == 0 ||
                         (move.x > 0 && pos.x >= end.x) ||
                         (move.x < 0 && pos.x <= end.x);

                yield return null;
            }

            fsm.SendEvent("TOP");
        }

        private void Start()
        {
            var fsm = gameObject.LocateMyFSM("Control");

            var rise = fsm.GetState("Rise");
            var translate = (Translate)rise.actions[4];
            var setVel = (SetVelocity2d)rise.actions[5];

            var moveDir = move.normalized * 30;

            translate.x = moveDir.x;
            translate.y = moveDir.y;
            setVel.vector = moveDir;

            var end = transform.position + (Vector3)move;

            rise.DisableAction(7);

            rise.AddAction(() => DoRise(fsm, end), 7);

            var sp = (SetPosition)fsm.GetState("Hit Top").actions[0];
            sp.x = end.x;
            sp.y = end.y;

            ((SetPosition)fsm.GetState("Return").actions[1]).x = transform.position.x;
        }
    }

    public static void FixMillibelle(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Hit Around");
        var init = fsm.GetState("Init");
        init.DisableAction(1);
        init.DisableAction(2);

        fsm.GetState("Cancel NPC").AddAction(() => fsm.SendEvent("OnHit"), 0);

        for (var i = 1; i < 4; i++) obj.transform.GetChild(i).gameObject.SetActive(false);
        obj.AddComponent<TriggerActivator>();
    }

    public static void FixZoteHead(GameObject obj)
    {
        obj.transform.SetRotation2D(0);
        obj.AddComponent<PngObject>();
        obj.AddComponent<ZoteHead>();
    }

    public class ZoteHead : TriggerActivator
    {
        private Collider2D _col2d;
        private bool _ground;

        private void Start()
        {
            _col2d = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D _)
        {
            if (CheckTouchingGround())
            {
                _ground = true;
                gameObject.BroadcastEvent("OnLand");
            }
        }

        private void OnCollisionExit2D(Collision2D _)
        {
            if (_ground && !CheckTouchingGround())
            {
                _ground = false;
                gameObject.BroadcastEvent("OnAir");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponentInChildren<NailSlash>())
                gameObject.BroadcastEvent("OnHit");
        }

        public bool CheckTouchingGround()
        {
            var bounds1 = _col2d.bounds;
            double x1 = bounds1.min.x;
            bounds1 = _col2d.bounds;
            double y1 = bounds1.center.y;
            var vector21 = new Vector2((float)x1, (float)y1);
            Vector2 center = _col2d.bounds.center;
            var bounds2 = _col2d.bounds;
            double x2 = bounds2.max.x;
            bounds2 = _col2d.bounds;
            double y2 = bounds2.center.y;
            var vector22 = new Vector2((float)x2, (float)y2);
            bounds2 = _col2d.bounds;
            var distance = bounds2.extents.y + 0.16f;
            Debug.DrawRay(vector21, Vector2.down, Color.yellow);
            Debug.DrawRay(center, Vector2.down, Color.yellow);
            Debug.DrawRay(vector22, Vector2.down, Color.yellow);
            var raycastHit2D1 = Physics2D.Raycast(vector21, Vector2.down, distance, 256);
            var raycastHit2D2 = Physics2D.Raycast(center, Vector2.down, distance, 256);
            var raycastHit2D3 = Physics2D.Raycast(vector22, Vector2.down, distance, 256);
            return raycastHit2D1.collider || raycastHit2D2.collider || raycastHit2D3.collider;
        }
    }

    public static void FixColoWall(GameObject obj)
    {
        var child = obj.transform.GetChild(0);

        child.GetChild(0).gameObject.SetActive(false);
        foreach (var i in new[] { 0, 2, 4, 5, 6 }) child.GetChild(1).GetChild(i).gameObject.SetActive(false);

        var col = child.GetComponent<BoxCollider2D>();
        col.size = new Vector2(1.4581f, 6.4249f);
        col.offset = Vector2.zero;

        obj.AddComponent<ColoWall>();
    }

    public class ColoWall : MonoBehaviour
    {
        public float moveDistance;
        public float moveSpeed = 28;
        public bool flipped;
        public float rotation;

        private void Start()
        {
            var fsm = gameObject.LocateMyFSM("Control");

            var val = new Vector2(moveSpeed, 0);
            if (!flipped) val.x = -val.x;
            val = Quaternion.Euler(0, 0, rotation) * val;

            fsm.FsmVariables.FindFsmVector2("In Speed").Value = val;
            fsm.FsmVariables.FindFsmVector2("Out Speed").Value = -val;
        }
    }

    public static void FlipColoWall(GameObject obj, bool flip)
    {
        if (!flip) return;
        obj.transform.SetScaleX(-obj.transform.GetScaleX());
        obj.GetComponent<ColoWall>().flipped = true;
    }

    public static void RotateColoWall(GameObject obj, float rot)
    {
        obj.transform.SetRotation2D(rot);
        obj.GetComponent<ColoWall>().rotation = rot;
    }

    public static void FixConveyor(GameObject obj)
    {
        var block = Object.Instantiate(_conveyorBlock, obj.transform);

        block.transform.localScale = new Vector3(5.1f, 0.5f, 1);
        block.transform.localPosition = new Vector3(0, -0.0703f, 0);

        block.SetActive(true);
    }

    public static void FlipConveyor(GameObject obj, bool flip)
    {
        if (flip)
        {
            obj.transform.SetScaleX(-obj.transform.GetScaleX());
            return;
        }
        
        var belt = obj.GetComponentInChildren<ConveyorBelt>();
        belt.speed = -belt.speed;
    }
    
    public static void RotateConveyor(GameObject obj, float rot)
    {
        obj.transform.SetRotation2D(rot);
        
        if (rot is 90 or 270)
        {
            var belt = obj.GetComponentInChildren<ConveyorBelt>();
            if (rot is 270) belt.speed = -belt.speed;
            belt.vertical = true;
        }
    }
    
    private static readonly int EnemyLayer = LayerMask.NameToLayer("Enemies");

    public static void FixCloth(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Control");
        
        fsm.GetState("Wait").AddAction(() => fsm.SendEvent("CLOTH ENTER"));

        var gt = fsm.GetState("Get Target");
        gt.DisableAction(0);
        var target = fsm.FsmVariables.FindFsmGameObject("Target");
        gt.AddAction(() =>
        {
            var enemy = Object
                .FindObjectsOfType<Collider2D>()
                .Where(h => h.gameObject != obj && h.gameObject.layer == EnemyLayer)
                .OrderBy(h => (h.transform.position - obj.transform.position).sqrMagnitude)
                .FirstOrDefault();
            if (enemy && (enemy.transform.position - obj.transform.position).sqrMagnitude < 512) 
                target.value = enemy.gameObject;
        }, 0);
    }
}
