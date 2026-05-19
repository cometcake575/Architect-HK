using System.Linq;
using Architect.Content.Preloads;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using ApplyMusicCue = On.HutongGames.PlayMaker.Actions.ApplyMusicCue;
using TransitionToAudioSnapshot = On.HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot;

namespace Architect.Behaviour.Fixers;

public static class EnemyFixers
{
    // Gruz Mother
    private static GameObject _gruzSpawns;

    // Aspid Mother
    private static GameObject _aspidCage;

    // Husk Hive
    private static GameObject _huskHiveCage;

    // Carver Hatcher
    private static GameObject _carverCage;

    // Flukemarm
    private static GameObject _flukeCage;

    public static void Init()
    {
        PreloadManager.RegisterPreload(new BasicPreload("Crossroads_04", "_Enemies/Fly Spawn",
            o =>
            {
                _gruzSpawns = o;

                foreach (var hm in _gruzSpawns.GetComponentsInChildren<HealthManager>(true))
                    hm.battleScene = null;
            }));

        PreloadManager.RegisterPreload(new BasicPreload("Crossroads_19", "Hatcher Cage (1)",
            o => _aspidCage = o));

        PreloadManager.RegisterPreload(new BasicPreload("Hive_01", "Hatcher Cage (1)",
            o => _huskHiveCage = o));

        PreloadManager.RegisterPreload(new BasicPreload("Deepnest_26b", "Centipede Cage",
            o => _carverCage = o));

        PreloadManager.RegisterPreload(new BasicPreload("GG_Flukemarm", "Hatcher Cage (2)",
            o => _flukeCage = o));

        ApplyMusicCue.OnEnter += (orig, self) =>
        {
            if (self.fsmComponent && self.fsmComponent.GetComponent<BlockMusic>())
            {
                self.Finish();
                return;
            }

            orig(self);
        };

        TransitionToAudioSnapshot.OnEnter += (orig, self) =>
        {
            if (self.fsmComponent && self.fsmComponent.GetComponent<BlockMusic>())
            {
                self.Finish();
                return;
            }

            orig(self);
        };

        On.CorpseHatcher.Smash += (orig, self) =>
        {
            var hatcher = self.GetComponent<HatcherCorpse>();
            if (hatcher)
            {
                var toEnable = DisableOthers(hatcher);
                orig(self);
                EnableOthers(toEnable);
            }
            else
            {
                orig(self);
            }
        };

        On.CorpseZomHive.LandEffects += (orig, self) =>
        {
            var hatcher = self.GetComponent<HatcherCorpse>();
            if (hatcher)
            {
                var toEnable = DisableOthers(hatcher);
                orig(self);
                EnableOthers(toEnable);
            }
            else
            {
                orig(self);
            }
        };
    }

    public static void RotateShardmite(GameObject obj, float rot)
    {
        if (rot is >= 180 and <= 270)
        {
            rot -= 180;
            obj.transform.SetScaleX(-obj.transform.GetScaleX());
            obj.transform.SetScaleY(-obj.transform.GetScaleY());
        }

        obj.transform.SetRotation2D(rot);
    }

    public static void FixGruzMother(GameObject obj)
    {
        var child = obj.transform.GetChild(2).gameObject;
        child.gameObject.RemoveComponent<PolygonCollider2D>();
        var box = child.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size *= 100;

        obj.AddComponent<GruzMother>();
        BlockMusicOn(obj);
    }

    public static void BlockMusicOn(GameObject obj)
    {
        obj.AddComponent<BlockMusic>();
        var ede = obj.GetComponent<EnemyDeathEffects>();
        if (ede)
        {
            ede.PreInstantiate();
            if (ede.corpse) ede.corpse.AddComponent<BlockMusic>();
        }
    }

    public class BlockMusic : MonoBehaviour;

    public class GruzMother : Wakeable
    {
        public bool spawnGruzzers = true;

        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Big Fly Control");
            fsm.SendEvent("TAKE DAMAGE");
            fsm.GetState("Sleep").AddAction(() => fsm.SendEvent("TAKE DAMAGE"));
        }

        public void Start()
        {
            var ede = GetComponent<EnemyDeathEffects>();
            ede.PreInstantiate();
            var corpseFsm = ede.corpse.LocateMyFSM("corpse");

            var spawn = spawnGruzzers;
            var flyName = $"{name} Spawns";
            corpseFsm.GetState("Blow").AddAction(() =>
            {
                var bursterFsm = corpseFsm.FsmVariables.FindFsmGameObject("Burster").Value.LocateMyFSM("burster");
                bursterFsm.GetState("Geo").DisableAction(0);

                if (spawn)
                {
                    var flySpawn = Instantiate(_gruzSpawns);
                    flySpawn.name = flyName;
                    flySpawn.SetActive(true);
                    ((FindGameObject)bursterFsm.GetState("Initiate").actions[4]).objectName = flySpawn.name;
                }
                else
                {
                    var check = (GGCheckIfBossScene)bursterFsm.GetState("Stop").actions[1];
                    check.regularSceneEvent = check.bossSceneEvent;
                }
            });
        }
    }

    public abstract class Wakeable : MonoBehaviour
    {
        public abstract void Wake();
    }

    public class MossKnight : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Moss Knight Control");
            fsm.FsmVariables.FindFsmBool("Dormant").Value = false;
            fsm.SendEvent("WAKE");
        }
    }

    public class Mistake : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Blob");
            fsm.FsmVariables.FindFsmBool("Spawns").value = false;
            ((WaitRandom)fsm.GetState("Spawn Pause").actions[0]).timeMax = 0;
            fsm.SendEvent("SPAWN");
        }
    }

    public class Folly : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Control");
            fsm.FsmVariables.FindFsmBool("Spawns").value = false;
            ((WaitRandom)fsm.GetState("Spawn Pause").actions[0]).timeMax = 0;
            fsm.SendEvent("SPAWN");
        }
    }

    public class InfectedBalloon : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Control");
            fsm.GetState("Initiate").AddAction(() => fsm.SetState("Chase - In Sight"));
            
            ((WaitRandom)fsm.GetState("Spawn Pause").actions[0]).timeMax = 0;
            fsm.SendEvent("SPAWN");
        }
    }

    public static void FixElderBaldur(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Blocker Control");
        fsm.GetState("Idle").DisableAction(2);
        fsm.GetState("Close").DisableAction(1);
    }

    public static void FlipElderBaldur(GameObject obj, bool flip)
    {
        if (!flip) return;
        obj.transform.SetScaleX(-obj.transform.GetScaleX());
        obj.LocateMyFSM("Blocker Control").FsmVariables.FindFsmBool("Facing Right").Value = false;
    }

    public static void RotateMawlurk(GameObject obj, float rot)
    {
        obj.transform.SetRotation2D(rot);

        var fsm = obj.LocateMyFSM("Mawlek Turret");

        var spawnPos = fsm.FsmVariables.FindFsmVector3("Spawn Pos");
        spawnPos.Value = Quaternion.Euler(0, 0, rot) * spawnPos.Value;

        var lerpRot = rot / 180;
        if (lerpRot > 1) lerpRot = 1 - lerpRot;
        fsm.FsmVariables.FindFsmFloat("Shot Speed").Value = Mathf.Lerp(25, 10, lerpRot);

        fsm.FsmVariables.FindFsmFloat("Angle Max L").Value = rot + 110 + Mathf.Lerp(0, 80, lerpRot);
        fsm.FsmVariables.FindFsmFloat("Angle Max R").Value = rot + 90;
        fsm.FsmVariables.FindFsmFloat("Angle Min L").Value = rot + 90;
        fsm.FsmVariables.FindFsmFloat("Angle Min R").Value = rot + 70 - Mathf.Lerp(0, 80, lerpRot);
    }

    public class ShrumalOgre : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Shroom Brawler");
            fsm.SendEvent("WAKE");
            fsm.GetState("Sleep").AddAction(() => fsm.SendEvent("WAKE"), 0);
        }
    }

    public static void FlipHopper(GameObject obj, bool flip)
    {
        if (!flip)
        {
            obj.transform.SetScaleX(-obj.transform.GetScaleX());
            obj.LocateMyFSM("Hopper").FsmVariables.FindFsmBool("Moving Right").Value = true;
        }
    }

    public static void FlipGreatHopper(GameObject obj, bool flip)
    {
        if (flip)
        {
            obj.transform.SetScaleX(-obj.transform.GetScaleX());
            obj.LocateMyFSM("Hopper").FsmVariables.FindFsmBool("Moving Right").Value = true;
        }
    }

    public static void FixMenderbug(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Mender Bug Ctrl");
        fsm.GetState("Killed").DisableAction(0);

        var bypass = () => fsm.SendEvent("FINISHED");
        fsm.GetState("Dead?").AddAction(bypass, 0);
        fsm.GetState("Sign Broken?").AddAction(bypass, 0);
        fsm.GetState("Chance").AddAction(bypass, 0);
    }

    public static void FixAspidMother(GameObject obj)
    {
        var cage = Object.Instantiate(_aspidCage);
        cage.SetActive(true);
        cage.name = obj.name + " Cage";

        ((FindGameObject)obj.LocateMyFSM("Hatcher").GetState("Initiate").actions[2]).objectName = cage.name;

        obj.AddComponent<Hatcher>().cage = cage;

        var ede = obj.GetComponent<EnemyDeathEffects>();
        ede.PreInstantiate();
        if (ede.corpse) ede.corpse.AddComponent<HatcherCorpse>().cage = cage;
    }

    public static void FixHuskHive(GameObject obj)
    {
        var cage = Object.Instantiate(_huskHiveCage);
        cage.SetActive(true);
        cage.name = obj.name + " Cage";

        ((FindGameObject)obj.LocateMyFSM("Hive Zombie").GetState("Init").actions[1]).objectName = cage.name;

        obj.AddComponent<Hatcher>().cage = cage;

        var ede = obj.GetComponent<EnemyDeathEffects>();
        ede.PreInstantiate();
        if (ede.corpse) ede.corpse.AddComponent<HatcherCorpse>().cage = cage;
    }

    public class Hatcher : MonoBehaviour
    {
        public GameObject cage;
    }

    public static void ScaleHatcher(GameObject obj, float scale)
    {
        obj.transform.localScale *= scale;
        obj.GetComponent<Hatcher>().cage.transform.localScale *= scale;
    }

    private static void EnableOthers(GameObject[] toEnable)
    {
        foreach (var obj in toEnable) obj.tag = "Extra Tag";
    }

    private static GameObject[] DisableOthers(HatcherCorpse hatcherCorpse)
    {
        var toEnable = GameObject.FindGameObjectsWithTag("Extra Tag")
            .Where(o => o != hatcherCorpse.cage && o.activeInHierarchy).ToArray();

        foreach (var obj in toEnable) obj.tag = "Untagged";

        return toEnable;
    }

    private class HatcherCorpse : MonoBehaviour
    {
        public GameObject cage;
    }

    public static void FixCarverHatcher(GameObject obj)
    {
        var cage = Object.Instantiate(_carverCage);
        cage.SetActive(true);
        cage.name = obj.name + " Cage";

        ((FindGameObject)obj.LocateMyFSM("Centipede Hatcher").GetState("Init").actions[1]).objectName = cage.name;

        obj.AddComponent<Hatcher>().cage = cage;
    }

    public static void FixFlukemarm(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Fluke Mother");
        fsm.GetState("GG?").AddAction(() => fsm.SendEvent("FINISHED"), 0);

        var cage = Object.Instantiate(_flukeCage);
        cage.SetActive(true);
        cage.name = obj.name + " Cage";

        ((FindGameObject)fsm.GetState("Init").actions[2]).objectName = cage.name;

        obj.AddComponent<Flukemarm>().fsm = fsm;
        obj.AddComponent<Hatcher>().cage = cage;
    }

    public class Flukemarm : Wakeable
    {
        public PlayMakerFSM fsm;

        public override void Wake()
        {
            fsm.SendEvent("TOOK DAMAGE");
            fsm.GetState("Idle").AddAction(() => fsm.SendEvent("TOOK DAMAGE"));
            fsm.GetState("Play Idle").AddAction(() => fsm.SendEvent("TOOK DAMAGE"));
        }
    }
    
    public static void FixVoltTwister(GameObject obj)
    {
        FixTwister(obj, "Electric Mage");
    }
    
    public static void FixSoulTwister(GameObject obj)
    {
        FixTwister(obj, "Mage");
    }
    
    private static void FixTwister(GameObject obj, string fsmName)
    {
        var fsm = obj.LocateMyFSM(fsmName);

        var teleplane = new GameObject(obj.name + " Teleplane")
        {
            tag = "Teleplane",
            transform =
            {
                position = obj.transform.position
            }
        };
        var collider = teleplane.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(10, 10);
        collider.isTrigger = true;

        var select = fsm.GetState("Select Target");
        
        select.DisableAction(1);
        select.AddAction(new FindGameObject
        {
            withTag = "Teleplane",
            objectName = teleplane.name,
            store = fsm.FsmVariables.GetFsmGameObject("Teleplane")
        }, 2);

        obj.AddComponent<Teleplane>().collider = collider;
    }
    
    public class Teleplane : MonoBehaviour
    {
        public BoxCollider2D collider;

        private void Start()
        {
            transform.position = collider.gameObject.transform.position;
        }
    }

    public static void FixZoteling(GameObject obj) => FixGenericZoteling(obj);

    private static PlayMakerFSM FixGenericZoteling(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Control");
        var antic = fsm.GetState("Spawn Antic");
        for (var i = 1; i <= 5; i++) antic.DisableAction(i);
        fsm.GetState("Dormant").AddAction(() => fsm.SendEvent("SPAWN"));
        obj.GetComponent<HealthManager>().hasSpecialDeath = false;

        return fsm;
    }

    public static void FixHeavyZoteling(GameObject obj)
    {
        var fsm = FixGenericZoteling(obj);
        fsm.GetState("Land Waves").AddAction(() =>
        {
            fsm.FsmVariables.FindFsmFloat("Shockwave Y").Value = fsm.transform.position.y - 2.3516f;
        }, 2);
    }

    public static void FixLankyZoteling(GameObject obj)
    {
        obj.RemoveComponent<ConstrainPosition>();
        var fsm = FixGenericZoteling(obj);
        fsm.GetState("Grav").DisableAction(1);
    }

    public static void FixWingedZoteling(GameObject obj) => FixBallZoteling(obj, "BUZZER");

    public static void FixHopZoteling(GameObject obj) => FixBallZoteling(obj, "HOPPER");

    private static void FixBallZoteling(GameObject obj, string type)
    {
        var fsm = obj.LocateMyFSM("Control");
        
        var ball = fsm.GetState("Ball");
        ball.DisableAction(2);
        var random = (WaitRandom)ball.actions[6];
        random.timeMin = 0.0001f;
        random.timeMax = 0.001f;

        fsm.GetState("Dormant").AddAction(() => fsm.SendEvent("SPAWN"));
        obj.GetComponent<HealthManager>().hasSpecialDeath = false;

        fsm.GetState("Choice").AddAction(() => fsm.SendEvent(type), 3);
    }
    
    public static void FixVolatileZoteling(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Control");
        fsm.GetState("Dormant").AddAction(() => fsm.SendEvent("BALLOON SPAWN"));
        obj.GetComponent<HealthManager>().hasSpecialDeath = false;

        fsm.GetState("Set Pos").DisableAction(6);

        fsm.GetState("Reset").transitions = [];
    }
    
    public static void FixFlukeZoteling(GameObject obj)
    {
        var fsm = obj.LocateMyFSM("Control");
        fsm.GetState("Dormant").AddAction(() => fsm.SendEvent("GO"));
        obj.GetComponent<HealthManager>().hasSpecialDeath = false;

        fsm.GetState("Pos").DisableAction(3);
        
        fsm.GetState("Climb").DisableAction(3);
    }

    public static void FixVengeflyKing(GameObject obj)
    {
        BlockMusicOn(obj);
        var fsm = obj.LocateMyFSM("Big Buzzer");

        fsm.GetState("Init").DisableAction(3);

        var pos = obj.transform.position;

        fsm.FsmVariables.FindFsmFloat("Swoop Height").Value = pos.y - 3.28f;

        var summon = fsm.GetState("Summon");

        var s1 = (CreateObject)summon.actions[1];
        var s2 = (CreateObject)summon.actions[3];
        
        s2.storeObject = s1.storeObject;

        var bi = fsm.FsmVariables.FindFsmGameObject("Buzzer Instance");
        
        summon.AddAction(() =>
        {
            var bobj = bi.Value;
            var mpos = obj.transform.position;
            if (bobj)
            {
                bobj.transform.position = new Vector3(mpos.x + 20.3926f *
                    (obj.transform.GetScaleX() > 0
                        ? -1
                        : 1), mpos.y + 6.9023f, 17);
            }
        }, 4);

        summon.AddAction(() =>
        {
            var bobj = bi.Value;
            var mpos = obj.transform.position;
            if (bobj)
            {
                bobj.transform.position = new Vector3(mpos.x + 1.3926f *
                    (obj.transform.GetScaleX() > 0
                        ? -1
                        : 1), mpos.y + 6.9023f, 17);
            }
        }, 2);
    }

    public static void FixSoulWarrior(GameObject obj)
    {
        BlockMusicOn(obj);
        var body = obj.GetComponent<Rigidbody2D>();
        var fsm = obj.LocateMyFSM("Mage Knight");

        fsm.GetState("Sleep").AddAction(() => fsm.SendEvent("WAKE"));

        var floorY = fsm.FsmVariables.FindFsmFloat("Floor Y");
        fsm.GetState("Side Tele Aim").AddAction(() => floorY.Value = fsm.gameObject.transform.position.y, 0);
        fsm.GetState("Up Tele Aim").AddAction(() => body.gravityScale = 0, 0);
        fsm.GetState("Idle").AddAction(() => body.gravityScale = 1, 0);
    }

    public static void FixTamerBeast(GameObject obj)
    {
        BlockMusicOn(obj);
        var fsm = obj.LocateMyFSM("Control");
        fsm.GetState("Dormant").AddAction(() => fsm.SendEvent("WAKE"));
    }

    public static void FixWatcherKnight(GameObject obj)
    {
        BlockMusicOn(obj);
        obj.transform.SetPositionZ(0.1102f);
        obj.AddComponent<WatcherKnight>();
        for (var i = 0; i < 4; i++) obj.transform.GetChild(i).gameObject.SetActive(false);
    }

    public class WatcherKnight : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Black Knight");
            fsm.SendEvent("WAKE");
            fsm.GetState("Rest").AddAction(() => fsm.SendEvent("WAKE"));
        }
    }

    public class MassiveMossCharger : Wakeable
    {
        public override void Wake()
        {
            var fsm = gameObject.LocateMyFSM("Mossy Control");
            fsm.SendEvent("WAKE");
            fsm.GetState("Sleep").AddAction(() => fsm.SendEvent("WAKE"));
        }
    }

    public static void FixMassiveMossCharger(GameObject obj)
    {
        obj.LocateMyFSM("Mossy Control").GetState("Init").DisableAction(28);
        obj.AddComponent<MassiveMossCharger>();
    }

    public static void ScaleMassiveMossCharger(GameObject obj, float scale)
    {
        var fsm = obj.LocateMyFSM("Mossy Control");
        ((SetScale)fsm.GetState("Emerge Left").actions[6]).x = scale;
        ((SetScale)fsm.GetState("Emerge Right").actions[6]).x = -scale;

        obj.transform.localScale *= scale;
    }

    private static PlayMakerFSM FixBrokenVessel(GameObject obj)
    {
        BlockMusicOn(obj);
        
        var fsm = obj.LocateMyFSM("IK Control");
        
        fsm.GetState("Roar").DisableAction(5);
        fsm.GetState("Waiting").AddAction(() => fsm.SendEvent("BATTLE START"), 3);
        
        fsm.FsmVariables.FindFsmFloat("Min Dstab Height").Value = -100;

        fsm.GetState("Aim Jump").AddAction(() =>
        {
            var newPos = obj.transform.position;
            fsm.FsmVariables.FindFsmFloat("Left X").Value = newPos.x - 10.235f;
            fsm.FsmVariables.FindFsmFloat("Right X").Value = newPos.x + 10.235f;
        }, 0);

        var aimJump2 = (RandomFloat)fsm.GetState("Aim Jump 2").actions[0];
        aimJump2.min = fsm.FsmVariables.FindFsmFloat("Left X");
        aimJump2.max = fsm.FsmVariables.FindFsmFloat("Right X");

        fsm.GetState("Set Height").DisableAction(0);

        var balloonFsm = obj.LocateMyFSM("Spawn Balloon");
        
        balloonFsm.GetState("Spawn").AddAction(() =>
        {
            var newPos = obj.transform.position;
            balloonFsm.FsmVariables.FindFsmFloat("X Min").Value = newPos.x - 9.55f;
            balloonFsm.FsmVariables.FindFsmFloat("X Max").Value = newPos.x + 9.55f;

            balloonFsm.FsmVariables.FindFsmFloat("Y Min").Value = newPos.y;
            balloonFsm.FsmVariables.FindFsmFloat("Y Max").Value = newPos.y + 5.26f;
        }, 0);
        
        return fsm;
    }

    public static void FixNormalBrokenVessel(GameObject obj)
    {
        var fsm = FixBrokenVessel(obj);
        fsm.GetState("Set Pos").DisableAction(1);
        fsm.GetState("Close Gates").DisableAction(0);
        fsm.GetState("Init").AddAction(() => fsm.SendEvent("ACTIVE"), 2);
    }

    public static void FixLostKin(GameObject obj)
    {
        var fsm = FixBrokenVessel(obj);
        var sx = fsm.GetState("Set X");
        sx.DisableAction(0);
        sx.DisableAction(2);
        fsm.GetState("Intro Fall").DisableAction(2);
        var cg = fsm.GetState("Close Gates");
        cg.DisableAction(3);
        cg.DisableAction(0);
    }
}