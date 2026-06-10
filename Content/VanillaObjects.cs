using System;
using Architect.Behaviour.Fixers;
using Architect.Objects.Categories;
using Architect.Objects.Groups;
using Architect.Objects.Placeable;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architect.Content;

public static class VanillaObjects
{
    public static void Init()
    {
        AddCrossroadsObjects();
        AddGreenObjects();
        AddWastesObjects();
        AddDeepnestObjects();
        AddCanyonObjects();
        AddCityObjects();
        AddWaterwaysObjects();
        AddPeakObjects();
        AddGardensObjects();
        AddEdgeObjects();
        AddColosseumObjects();
        AddHiveObjects();
        AddGroundsObjects();
        AddBasinObjects();
        AddDreamObjects();
        AddWpObjects();
        AddAbyssObjects();
        AddSoulObjects();
        AddNpcObjects();
        AddGhostObjects();
        AddOrdealObjects();
    }

    private static void AddCrossroadsObjects()
    {
        Categories.Misc.Add(new PreloadObject("Bench", "common_bench", 
                ("Crossroads_47", "RestBench")))
            .WithConfigGroup(ConfigGroup.Benches)
            .WithRotateAction(MiscFixers.RotateBench);
        
        Categories.Misc.Add(new PreloadObject("Toll Bench", "toll_bench", 
                ("Fungus3_50", "Toll Machine Bench")))
            .WithConfigGroup(ConfigGroup.TollBench)
            .WithRotateAction(MiscFixers.RotateBench);
        
        Categories.Interactable.Add(new PreloadObject("Lever", "basic_lever", 
                ("Room_Town_Stag_Station", "Gate Switch"),
                postSpawnAction: MiscFixers.FixLever))
            .WithConfigGroup(ConfigGroup.Levers)
            .WithBroadcasterGroup(BroadcasterGroup.Levers)
            .WithRotationGroup(RotationGroup.Eight);

        Categories.Misc.Add(new PreloadObject("Grub Bottle", "grub_bottle",
            ("Crossroads_31", "Grub Bottle"),
            postSpawnAction: MiscFixers.FixGrubBottle)
            .WithConfigGroup(ConfigGroup.GrubBottle)
            .WithBroadcasterGroup(BroadcasterGroup.PersistentBreakable));

        Categories.Misc.Add(new PreloadObject("Infected Blob S", "i_blob_1",
            ("Crossroads_07", "Infected Parent/infected_large_blob_010000"),
            preloadAction: MiscFixers.BreakableZ));
        
        Categories.Misc.Add(new PreloadObject("Infected Blob L", "i_blob_2",
            ("Crossroads_07", "Infected Parent/infected_large_blob_030000 (1)"),
            preloadAction: MiscFixers.BreakableZ));
        
        Categories.Misc.Add(new PreloadObject("Infected Drip", "i_drip",
            ("Crossroads_07", "Infected Parent/infected_orange_drip_020000 (3)"),
            preloadAction: MiscFixers.BreakableZ));
        
        Categories.Misc.Add(new PreloadObject("Infected Vine", "i_vine",
            ("Crossroads_07", "Infected Parent/infected_vine_01"),
            preloadAction: MiscFixers.BreakableZ));

        Categories.Hazards.Add(new PreloadObject("Falling Stalactite", "falling_stalactite",
            ("Tutorial_01", "_Props/Stalactite Hazard")));
        Categories.Hazards.Add(new PreloadObject("Cave Spikes", "cave_spikes",
            ("Mines_31", "Cave Spikes (4)"), preloadAction: MiscFixers.FixRotation)
            .WithRotationGroup(RotationGroup.Eight));
        
        AddSolid("Crossroads Platform 1", "plat_generic_1",
            ("Tutorial_01", "_Scenery/plat_float_01"));
        AddSolid("Crossroads Platform 2", "plat_generic_2",
            ("Tutorial_01", "_Scenery/plat_float_07"));
        AddSolid("Crossroads Platform 3", "plat_generic_3",
            ("Tutorial_01", "_Scenery/plat_float_08"));
        AddSolid("Crossroads Platform 4", "plat_generic_4",
            ("Tutorial_01", "_Scenery/plat_float_10"));
        
        AddSolid("Lift Platform 1", "plat_lift_1",
            ("Crossroads_07", "_Scenery/plat_lift_06"));
        AddSolid("Lift Platform 2", "plat_lift_2",
            ("Crossroads_07", "_Scenery/plat_lift_05"));

        Categories.Misc.Add(new PreloadObject("Strange Tablet", "strange_tablet",
                ("Tutorial_01", "_Props/Tut_tablet_top (2)"),
                preloadAction: MiscFixers.AddComponent<MiscFixers.Tablet>)
            .WithConfigGroup(ConfigGroup.Npcs));

        Categories.Misc.Add(new PreloadObject("Lifeblood Cocoon", "lifeblood_cocoon",
                ("Tutorial_01", "_Props/Health Cocoon"))
            .WithConfigGroup(ConfigGroup.Cocoon));

        AddEnemy("Tiktik", "tiktik", ("Crossroads_07", "_Enemies/Climber 3"))
            .WithRotationGroup(RotationGroup.Four);

        AddEnemy("Gruzzer", "gruzzer", ("Crossroads_07", "Uninfected Parent/Fly"));
        AddEnemy("Crawlid", "crawlid", ("Crossroads_07", "_Enemies/Crawler"));

        AddEnemy("Vengefly", "vengefly", ("Crossroads_16", "Uninfected Parent/Buzzer"));
        
        AddEnemy("Vengefly King", "vengefly_king", ("GG_Vengefly", "Giant Buzzer Col"),
            postSpawnAction: EnemyFixers.FixVengeflyKing);
        
        AddEnemy("Wandering Husk", "wandering_husk", ("Crossroads_21", "non_infected_event/Zombie Runner"));
        AddEnemy("Husk Bully", "husk_bully", ("Crossroads_21", "Zombie Barger"));
        AddEnemy("Leaping Husk", "leaping_husk", ("Crossroads_21", "non_infected_event/Zombie Leaper"));
        AddEnemy("Husk Hornhead", "husk_hornhead", ("Crossroads_16", "_Enemies/Zombie Hornhead"));
        AddEnemy("Husk Warrior", "husk_warrior", ("Crossroads_15", "_Enemies/Zombie Shield"));
        AddEnemy("Husk Guard", "husk_guard", ("Crossroads_21", "non_infected_event/Zombie Guard"));
        AddEnemy("Maggot", "maggot", ("Crossroads_10_boss_defeated", "Prayer Room/Prayer Slug"));
        AddEnemy("Baldur", "baldur", ("Crossroads_ShamanTemple", "_Enemies/Roller"));

        AddEnemy("Volatile Gruzzer", "volatile_gruzzer",
            ("Crossroads_07", "Infected Parent/Bursting Bouncer"));
        AddEnemy("Furious Vengefly", "furious_vengefly",
            ("Crossroads_16", "infected_event/Angry Buzzer"));
        AddEnemy("Violent Husk", "violent_husk",
            ("Crossroads_21", "infected_event/Bursting Zombie"));
        AddEnemy("Slobbering Husk", "slobbering_husk",
            ("Crossroads_15", "Infected Parent/Spitting Zombie"));

        AddEnemy("Elder Baldur", "elder_baldur", ("Crossroads_ShamanTemple", "Battle Scene/Blocker"),
                postSpawnAction: EnemyFixers.FixElderBaldur)
            .WithFlipAction(EnemyFixers.FlipElderBaldur);

        AddEnemy("Menderbug", "menderbug", ("Crossroads_01", "_Scenery/Mender Bug"),
            postSpawnAction: EnemyFixers.FixMenderbug);

        AddEnemy("Aspid Hunter", "aspid_hunter", ("Crossroads_19", "_Enemies/Spitter"));
        AddEnemy("Aspid Mother", "aspid_mother", ("Crossroads_19", "_Enemies/Hatcher"),
            postSpawnAction: EnemyFixers.FixAspidMother)
            .WithScaleAction(EnemyFixers.ScaleHatcher);

        AddEnemy("Aspid Hatchling", "aspid_hatchling",
            ("Crossroads_19", "Hatcher Cage (1)/Hatcher Baby Spawner"),
            postSpawnAction: EnemyFixers.FixHatchling);

        Categories.Interactable.Add(new PreloadObject("Bone Gate", "bone_gate",
            ("Crossroads_ShamanTemple", "Bone Gate"),
            postSpawnAction: o => o.LocateMyFSM("Bone Gate").GetState("Idle").DisableAction(2))
            .WithReceiverGroup(ReceiverGroup.BoneGate));

        Categories.Platforming.Add(new PreloadObject("Collapser", "collapser_1",
            ("Crossroads_36", "_Props/Collapser Small 1"))
            .WithConfigGroup(ConfigGroup.PersistentBreakable));

        Categories.Interactable.Add(new PreloadObject("Dive Collapser", "collapser_dive_1",
            ("Crossroads_52", "Quake Floor"), preloadAction: o =>
            {
                var child = o.transform.GetChild(0).GetChild(0);
                for (var i = 0; i < 4; i++) child.GetChild(i).gameObject.SetActive(false);
            })
            .WithConfigGroup(ConfigGroup.PersistentBreakable));

        Categories.Interactable.Add(new PreloadObject("Breakable Door", "breakable_door_1",
            ("Crossroads_07", "_Props/Tute Door 1"))
            .WithConfigGroup(ConfigGroup.PersistentBreakable)
            .WithRotationGroup(RotationGroup.Four));

        Categories.Interactable.Add(new PreloadObject("Breakable Wall", "breakable_wall_1",
            ("Mines_25", "Breakable Wall"), preloadAction: MiscFixers.FixBreakableWall)
            .WithConfigGroup(ConfigGroup.PersistentBreakable));

        Categories.Interactable.Add(new PreloadObject("One Way Wall", "one_way_wall_1",
            ("Cliffs_02", "One Way Wall"), 
            uiSprite: ResourceUtils.LoadSpriteResource("one_way_wall"))
            .WithConfigGroup(ConfigGroup.PersistentBreakable));

        Categories.Interactable.Add(new PreloadObject("Breakable Floor", "breakable_floor_1",
            ("Crossroads_04", "_Scenery/Break Floor 1"),
            uiSprite: ResourceUtils.LoadSpriteResource("breakable_floor"))
            .WithConfigGroup(ConfigGroup.PersistentBreakable));
        
        Categories.Hazards.Add(new PreloadObject("Goam", "goam", ("Crossroads_12", "_Enemies/Worm"))
            .WithRotationGroup(RotationGroup.Four)
            .WithConfigGroup(ConfigGroup.Goams));

        AddEnemy("Gruz Mother", "gruz_mother", ("Crossroads_04", "_Enemies/Giant Fly"),
            postSpawnAction: EnemyFixers.FixGruzMother)
            .WithConfigGroup(ConfigGroup.GruzMother)
            .WithReceiverGroup(ReceiverGroup.Wakeable);

        Categories.Platforming.Add(new PreloadObject("Zote Head", "zote_head",
            ("Fungus1_20_v02", "Zote Death/Head"), preloadAction: MiscFixers.FixZoteHead)
            .WithConfigGroup(ConfigGroup.ZoteHead)
            .WithBroadcasterGroup(BroadcasterGroup.ZoteHead));
        Categories.Platforming.Add(new PreloadObject("Grubfather", "grubfather",
            ("Crossroads_38", "Fat Grub King"), preloadAction: o => o.LocateMyFSM("FSM").enabled = false));

        Categories.Misc.Add(new PreloadObject("Breakable Barrel", "brk_barrel_1",
            ("Crossroads_07", "_Scenery/brk_barrel_05"))
            .WithBroadcasterGroup(BroadcasterGroup.Breakable)
            .WithConfigGroup(ConfigGroup.Decorations));
        Categories.Misc.Add(new PreloadObject("Breakable Cart", "brk_cart_1",
            ("Crossroads_07", "_Scenery/brk_cart_05"))
            .WithBroadcasterGroup(BroadcasterGroup.Breakable)
            .WithConfigGroup(ConfigGroup.Decorations));
        Categories.Misc.Add(new PreloadObject("Horned Statue", "statue_horned",
            ("Crossroads_47", "_Props/Crossroads Statue Horned"),
            preloadAction: MiscFixers.BreakableZ)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable)
            .WithConfigGroup(ConfigGroup.Decorations));
        Categories.Misc.Add(new PreloadObject("Stone Statue", "statue_stone",
            ("Crossroads_47", "_Props/Crossroads Statue Stone"),
            preloadAction: MiscFixers.BreakableZ)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable)
            .WithConfigGroup(ConfigGroup.Decorations));

        /*
        AddEnemy("Gorb", "gorb",
            ("Cliffs_02_boss", "Warrior/Ghost Warrior Slug"),
            preloadAction: MiscFixers.AddComponent<EnemyFixers.Gorb>)
            .WithConfigGroup(ConfigGroup.Gorb);*/
    }

    private static void AddGreenObjects()
    {
        Categories.Interactable.Add(new PreloadObject("Toll Machine", "toll_machine",
            ("Fungus1_31", "Toll Gate Machine"),
            postSpawnAction: MiscFixers.FixToll)
            .WithConfigGroup(ConfigGroup.TollMachine)
            .WithBroadcasterGroup(BroadcasterGroup.Toll));
        
        Categories.Interactable.Add(new PreloadObject("Toll Gate", "toll_gate",
            ("Fungus1_31", "Toll Gate"))
            .WithReceiverGroup(ReceiverGroup.TollGate));
        
        AddEnemy("Squit", "squit", ("Fungus1_22", "Mosquito"));
        AddEnemy("Fool Eater", "fool_eater", ("Fungus1_22", "Plant Trap"))
            .WithRotationGroup(RotationGroup.Four)
            ;
        AddEnemy("Gulka", "gulka", ("Fungus1_12", "Plant Turret"))
            .WithRotationGroup(RotationGroup.Eight);
        
        AddEnemy("Maskfly", "maskfly", ("Fungus1_12", "Pigeon"));
        
        AddEnemy("Moss Charger", "moss_charger", ("Fungus1_17", "Moss Charger"));
        AddEnemy("Massive Moss Charger", "massive_moss_charger", ("GG_Mega_Moss_Charger", "Mega Moss Charger"),
            postSpawnAction: EnemyFixers.FixMassiveMossCharger)
            .WithScaleAction(EnemyFixers.ScaleMassiveMossCharger)
            .WithReceiverGroup(ReceiverGroup.Wakeable)
            .WithConfigGroup(ConfigGroup.Wakeable);
        
        AddEnemy("Durandoo", "durandoo", ("Fungus1_12", "Acid Walker"));
        
        AddEnemy("Duranda", "duranda", ("Fungus1_09", "Acid Flyer"))
            .WithConfigGroup(ConfigGroup.Duranda);
        
        AddEnemy("Mosskin", "mosskin", ("Fungus1_31", "_Enemies/Mossman_Runner"));
        AddEnemy("Volatile Mosskin", "volatile_mosskin", ("Fungus1_22", "Mossman_Shaker"));
        
        AddEnemy("Moss Knight", "moss_knight", ("Fungus1_32", "Moss Knight C"),
            preloadAction: MiscFixers.AddComponent<EnemyFixers.MossKnight>)
            .WithConfigGroup(ConfigGroup.Wakeable)
            .WithReceiverGroup(ReceiverGroup.Wakeable);
        
        AddEnemy("Mosscreep", "mosscreep", ("Fungus1_22", "Moss Walker"));
        AddEnemy("Obble", "obble", ("Fungus1_31", "_Enemies/Fat Fly (1)"));
    }
    
    private static void AddCanyonObjects()
    {
        Categories.Hazards.Add(new PreloadObject("Jelly Egg Bomb", "jelly_egg_bomb",
            ("Fungus3_26", "Jelly Egg Bomb"))
            .WithReceiverGroup(ReceiverGroup.JellyEgg));
        
        Categories.Hazards.Add(new PreloadObject("Charged Lumaflies", "charged_lumaflies",
            ("Fungus3_26", "Zap Cloud")));
        
        AddEnemy("Ooma", "ooma", ("Fungus3_26", "Jellyfish"));
        AddEnemy("Uoma", "uoma", ("Fungus3_26", "Jellyfish Baby"));

        AddSolid("Fog Canyon Platform 1", "fog_plat_1", ("Fungus3_26", "fung_plat_float_01"));
        AddSolid("Fog Canyon Platform 2", "fog_plat_2", ("Fungus3_26", "fung_plat_float_04"));
        AddSolid("Fog Canyon Platform 3", "fog_plat_3", ("Fungus3_26", "fung_plat_float_05"));
        AddSolid("Fog Canyon Platform 4", "fog_plat_4", ("Fungus3_26", "fung_plat_float_06"));
        AddSolid("Fog Canyon Platform 5", "fog_plat_5", ("Fungus3_26", "fung_plat_float_07"));
        AddSolid("Fog Canyon Platform 6", "fog_plat_6", ("Fungus3_26", "fung_plat_float_08"));
        
        AddSolid("Archives Platform 1", "archive_plat_1", ("Fungus3_archive_02", "fung temple_plat_float_small"));
        AddSolid("Archives Platform 2", "archive_plat_2", ("Fungus3_archive_02", "fung_temple_plat_float (1)"));
    }

    private static void AddWastesObjects()
    {
        AddSolid("Fungal Wastes Platform 1", "wastes_plat_1", ("Fungus2_04", "mush_plat_float_01"));
        AddSolid("Fungal Wastes Platform 2", "wastes_plat_2", ("Fungus2_18", "_Scenery/mush_plat_float_03"));
        AddSolid("Fungal Wastes Platform 3", "wastes_plat_3", ("Fungus2_18", "_Scenery/mush_plat_float_04"));
        AddSolid("Fungal Wastes Platform 4", "wastes_plat_4", ("Fungus2_04", "mush_plat_float_05"));
        
        AddEnemy("Fungified Husk", "fungified_husk_a", ("Fungus2_18", "Zombie Fungus A"));
        
        AddEnemy("Shrumal Ogre", "shrumal_ogre", ("Fungus2_05", "Battle Scene v2/Completed/Mushroom Brawler"),
            preloadAction: MiscFixers.AddComponent<EnemyFixers.ShrumalOgre>)
            .WithConfigGroup(ConfigGroup.Wakeable)
            .WithReceiverGroup(ReceiverGroup.Wakeable);
        
        AddEnemy("Shrumeling", "shrumeling", ("Fungus2_05", "Battle Scene v2/Completed/Mushroom Baby"));
        AddEnemy("Fungling", "fungling", ("Fungus2_18", "_Scenery/Fungoon Baby"));
        AddEnemy("Fungoon", "fungoon", ("Fungus2_18", "_Scenery/Fungus Flyer"));
        AddEnemy("Shrumal Warrior", "shrumal_warrior", ("Fungus2_28", "Mushroom Roller"));
        AddEnemy("Ambloom", "ambloom", ("Fungus2_18", "Fung Crawler"));
        AddEnemy("Sporg", "sporg", ("Fungus2_04", "Mushroom Turret"), preloadAction: MiscFixers.FixRotation);
        
        AddEnemy("Mantis", "mantis", ("Fungus2_12", "Mantis"))
            .WithConfigGroup(ConfigGroup.Mantis);
        AddEnemy("Mantis Youth", "mantis_youth", ("Fungus2_12", "Mantis Flyer Child"))
            .WithConfigGroup(ConfigGroup.MantisChild);

        Categories.Interactable.Add(new PreloadObject("Mantis Lever", "mantis_lever",
            ("Fungus2_04", "Mantis Lever"),
            postSpawnAction: MiscFixers.FixLever)
            .WithBroadcasterGroup(BroadcasterGroup.Levers)
            .WithConfigGroup(ConfigGroup.Levers));
        Categories.Interactable.Add(new PreloadObject("Mantis Gate", "mantis_gate",
            ("Fungus2_04", "Mantis Gate"))
            .WithReceiverGroup(ReceiverGroup.MantisGate));

        Categories.Platforming.Add(new PreloadObject("Bounce Shroom", "bounce_shroom",
            ("Fungus2_18", "_Props/Bounce Shrooms 1/Bounce Shroom B (1)"),
            preloadAction: o => o.transform.SetPositionZ(-0.01f))
            .WithConfigGroup(ConfigGroup.BounceShroom));
    }

    private static void AddDeepnestObjects()
    {
        AddEnemy("Corpse Creeper (Wandering Husk)", "corpse_creeper_a",
            ("Deepnest_33", "Zombie Runner Sp (1)"));
        AddEnemy("Corpse Creeper (Husk Hornhead)", "corpse_creeper_b",
            ("Deepnest_33", "Zombie Hornhead Sp (2)"));
        
        AddEnemy("Dirtcarver", "dirtcarver", ("Deepnest_17", "Baby Centipede"));
        AddEnemy("Carver Hatcher", "carver_hatcher", ("Deepnest_26b", "Centipede Hatcher (4)"),
            postSpawnAction: EnemyFixers.FixCarverHatcher)
            .WithScaleAction(EnemyFixers.ScaleHatcher);
        
        AddEnemy("Flukemarm", "flukemarm", ("GG_Flukemarm", "Fluke Mother"),
            postSpawnAction: EnemyFixers.FixFlukemarm)
            .WithScaleAction(EnemyFixers.ScaleHatcher)
            .WithReceiverGroup(ReceiverGroup.Wakeable)
            .WithConfigGroup(ConfigGroup.Wakeable);

        AddEnemy("Little Weaver", "little_weaver", ("Deepnest_41", "Spider Flyer"))
            .WithConfigGroup(ConfigGroup.LittleWeaver);
        AddEnemy("Deepling", "deepling", ("Deepnest_Spider_Town", "Tiny Spider"),
            preloadAction: MiscFixers.KeepActive);
        AddEnemy("Deephunter", "deephunter", ("Deepnest_Spider_Town", "Spider Mini"),
            preloadAction: o =>
            {
                MiscFixers.KeepActive(o);
                MiscFixers.FixRotation(o);
            }).WithRotationGroup(RotationGroup.Four);
        AddEnemy("Stalking Devout", "stalking_devout", ("Deepnest_Spider_Town", "Slash Spider"),
            preloadAction: MiscFixers.KeepActive);
        AddEnemy("Bluggsac", "bluggsac", ("Deepnest_Spider_Town", "Egg Sac"));
    }
    
    private static void AddWaterwaysObjects()
    {
        AddEnemy("Hwurmp", "hwurmp", ("Waterways_01", "_Enemies/Inflater"));
        AddEnemy("Pilflip", "pilflip", ("Waterways_01", "_Enemies/Flip Hopper"));
        AddEnemy("Flukefey", "flukefey", ("GG_Pipeway", "Fluke Fly"));
        AddEnemy("Flukemon", "flukemon", ("GG_Pipeway", "Flukeman"));
        AddEnemy("Flukemunga", "flukemunga", ("GG_Pipeway", "Fat Fluke"));
    }
    
    private static void AddCityObjects()
    {
        Categories.Misc.Add(new PreloadObject("Standing Lantern", "lantern_stand_1", 
                ("Fungus1_31", "_Scenery/fung_lamp2"), preloadAction: MiscFixers.BreakableZ)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable));
        Categories.Misc.Add(new PreloadObject("Ceiling Lantern", "lantern_ceil_1", 
                ("Ruins_House_02", "ruind_dressing_light_03"), preloadAction: MiscFixers.BreakableZ)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable));
        
        Categories.Interactable.Add(new PreloadObject("Reusable Lever", "reusable_lever", 
            ("Ruins1_03", "Lift Call Lever"),
            postSpawnAction: InteractableFixers.FixReusableLever)
            .WithBroadcasterGroup(BroadcasterGroup.Activatable));

        Categories.Interactable.Add(new PreloadObject("City Lever", "city_lever",
                ("Ruins2_01", "Ruins Lever"),
                preloadAction: MiscFixers.FixRotation,
                postSpawnAction: MiscFixers.FixLever)
            .WithBroadcasterGroup(BroadcasterGroup.Levers)
            .WithConfigGroup(ConfigGroup.Levers)
            .WithRotationGroup(RotationGroup.Eight));
        Categories.Interactable.Add(new PreloadObject("Battle Gate", "battle_gate",
                ("Ruins2_01", "Battle Gate"))
            .WithConfigGroup(ConfigGroup.BattleGate)
            .WithReceiverGroup(ReceiverGroup.BattleGate));
        Categories.Interactable.Add(new PreloadObject("City Gate", "city_gate",
                ("Ruins2_01", "Ruins Gate 2"))
            .WithReceiverGroup(ReceiverGroup.CityGate));

        Categories.Hazards.Add(new PreloadObject("Roof Spikes", "roof_spikes",
            ("Ruins2_08", "ruind_bridge_roof_01 (1)/ruind_bridge_roof_04_spikes")));

        AddSolid("City Platform 1", "plat_city_1", ("Ruins1_03", "_Scenery/ruin_plat_float_01"));
        AddSolid("City Platform 2", "plat_city_2", ("Ruins1_03", "_Scenery/ruin_plat_float_01_wide"));
        AddSolid("City Platform 3", "plat_city_3", ("Ruins1_03", "_Scenery/ruin_plat_float_02"));
        AddSolid("City Platform 4", "plat_city_4", ("Ruins1_03", "_Scenery/ruin_plat_float_05"));
        
        AddSolid("Fancy Platform 1", "plat_fancy_1", ("Ruins2_01", "ruins_mage_building_0011_a_royal_plat"));
        AddSolid("Fancy Platform 2", "plat_fancy_2", ("Ruins2_01", "ruins_plat_royal_02"));

        AddEnemy("Husk Sentry", "husk_sentry", ("Ruins2_01", "Ruins Sentry 1"));
        AddEnemy("Winged Sentry", "winged_sentry", ("Ruins2_01", "Ruins Flying Sentry"));
        AddEnemy("Lance Sentry", "lance_sentry", ("Ruins2_01", "Ruins Flying Sentry Javelin"));
        AddEnemy("Heavy Sentry", "heavy_sentry", ("Ruins2_01", "Ruins Sentry Fat"));
        
        AddEnemy("Cowardly Husk", "cowardly_husk", ("Ruins2_01", "Royal Zombie Coward (1)"));
        AddEnemy("Husk Dandy", "husk_dandy", ("Ruins2_01", "Royal Zombie 1 (1)"));
        AddEnemy("Gluttonous Husk", "gluttonous_husk", ("Ruins2_01", "Royal Zombie Fat (2)"));
        AddEnemy("Great Husk Sentry", "great_husk_sentry", ("Ruins2_01", "Battle Scene/Great Shield Zombie"));
        AddEnemy("Gorgeous Husk", "gorgeous_husk", ("Ruins_House_02", "Gorgeous Husk"));

        Categories.Misc.Add(new PreloadObject("Millibelle", "millibelle",
            ("Ruins_Bathhouse", "Banker Spa NPC"),
            postSpawnAction: MiscFixers.FixMillibelle)
            .WithConfigGroup(ConfigGroup.TriggerActivator)
            .WithBroadcasterGroup(BroadcasterGroup.Hittable));

        AddEnemy("Mistake", "mistake", ("Ruins1_30", "Mage Blob 1"),
                preloadAction: MiscFixers.AddComponent<EnemyFixers.Mistake>)
            .WithConfigGroup(ConfigGroup.Wakeable)
            .WithReceiverGroup(ReceiverGroup.Wakeable);
        AddEnemy("Folly", "folly", ("Ruins1_30", "Mage Balloon"),
                preloadAction: MiscFixers.AddComponent<EnemyFixers.Folly>)
            .WithConfigGroup(ConfigGroup.Wakeable)
            .WithReceiverGroup(ReceiverGroup.Wakeable);

        AddEnemy("Volt Twister", "volt_twister",
            ("Room_Colosseum_Gold", "Colosseum Manager/Waves/Wave 25/Electric Mage New"),
            postSpawnAction: EnemyFixers.FixVoltTwister)
            .WithConfigGroup(ConfigGroup.Teleplane);
        AddEnemy("Soul Twister", "soul_twister",
            ("Ruins1_30", "Mage"),
            postSpawnAction: EnemyFixers.FixSoulTwister)
            .WithConfigGroup(ConfigGroup.Teleplane);
        
        AddEnemy("Soul Warrior", "soul_warrior", ("GG_Mage_Knight", "Mage Knight"),
            postSpawnAction: EnemyFixers.FixSoulWarrior);

        AddEnemy("Watcher Knight", "watcher_knight", ("GG_Watcher_Knights", "Battle Control/Black Knight 1"),
                preloadAction: EnemyFixers.FixWatcherKnight)
            .WithConfigGroup(ConfigGroup.Wakeable)
            .WithReceiverGroup(ReceiverGroup.Wakeable);
    }

    private static void AddPeakObjects()
    {
        AddEnemy("Shardmite", "shardmite", ("Mines_20", "Mines Crawler"),
                preloadAction: MiscFixers.FixRotation)
            .WithRotationGroup(RotationGroup.Four)
            .WithRotateAction(EnemyFixers.RotateShardmite);
        
        AddEnemy("Glimback", "glimback", ("Mines_20", "Crystal Crawler"));
        AddEnemy("Crystal Crawler", "crystal_crawler", ("Mines_20", "Crystallised Lazer Bug (3)"));
        AddEnemy("Crystal Hunter", "crystal_hunter", ("Mines_25", "Crystal Flyer"));
        AddEnemy("Husk Miner", "husk_miner", ("Mines_20", "Zombie Miner 1"));
        AddEnemy("Husk Myla", "husk_myla", ("Crossroads_45", "Zombie Myla"),
            preloadAction: MiscFixers.KeepActive);
        AddEnemy("Crystallised Husk", "crystallised_husk", ("Mines_25", "Zombie Beam Miner"));
        
        Categories.Hazards.Add(new PreloadObject("Laser Crystal", "laser_crystal", 
            ("Mines_31", "Laser Turret"), 
            preloadAction: MiscFixers.FixRotation,
            postSpawnAction: o => o.LocateMyFSM("Laser Bug").GetState("Idle").DisableAction(2))
            .WithConfigGroup(ConfigGroup.LaserCrystal)
            .WithRotationGroup(RotationGroup.Eight));
        
        Categories.Hazards.Add(new PreloadObject("Stomper", "stomper", 
            ("Mines_37", "stomper_offset"), 
            postSpawnAction: o => 
                o.GetComponentInChildren<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate)
            .WithConfigGroup(ConfigGroup.Stomper));
        
        Categories.Platforming.Add(new PreloadObject("Flipping Platform", "flipping_platform", 
            ("Mines_31", "Mines Platform")));
        
        Categories.Misc.Add(new PreloadObject("Breakable Crystal", "crystal_peak_crystals",
                ("Mines_20", "brk_Crystal3"), 
                preloadAction: MiscFixers.FixRotation)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable)
            .WithConfigGroup(ConfigGroup.Decorations));
        Categories.Misc.Add(new PreloadObject("Crystal Barrel", "crystal_peak_barrel",
                ("Mines_20", "crystal_barrel"),
                preloadAction: MiscFixers.BreakableZ)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable)
            .WithConfigGroup(ConfigGroup.Decorations));
        
        AddSolid("Metal Grate", "metal_grate", ("Mines_20", "mines_metal_grate_06"));
        AddSolid("Crystal Peak Platform", "peak_plat_1", ("Mines_20", "plat_float_06"));

        Categories.Interactable.Add(new PreloadObject("Metal Gate", "metal_gate", 
                ("Mines_20", "Metal Gate v2"))
            .WithReceiverGroup(ReceiverGroup.MetalGate)
            .WithRotationGroup(RotationGroup.Four));
        
        Categories.Interactable.Add(new PreloadObject("Chest", "chest_1", ("Mines_37", "Chest"),
                postSpawnAction: MiscFixers.FixChest)
            .WithConfigGroup(ConfigGroup.Chest)
            .WithBroadcasterGroup(BroadcasterGroup.Openable));
        
        Categories.Platforming.Add(new PreloadObject("Conveyor", "conveyor",
                ("Mines_31", "conveyor_belt_0mid (3)/conveyor_belt_simple0004"),
                postSpawnAction: MiscFixers.FixConveyor)
            .WithConfigGroup(ConfigGroup.Conveyor)
            .WithFlipAction(MiscFixers.FlipConveyor)
            .WithRotateAction(MiscFixers.RotateConveyor)
            .WithRotationGroup(RotationGroup.Three));
    }

    private static void AddGardensObjects()
    {
        AddEnemy("Mossy Vagabond", "mossy_vagabond", ("Fungus3_39", "Moss Knight Fat"),
            preloadAction: o =>
            {
                Object.Destroy(o.LocateMyFSM("FSM"));
            });
        AddEnemy("Spiny Husk", "spiny_husk", ("Fungus3_34", "Garden Zombie"));
        AddEnemy("Aluba", "aluba", ("Fungus3_48", "Lazy Flyer Enemy"));
        AddEnemy("Mossfly", "mossfly", ("Fungus3_34", "Moss Flyer"));
        AddEnemy("Loodle", "loodle", ("Fungus3_48", "Grass Hopper"));
        AddEnemy("Mantis Traitor", "mantis_traitor", ("Fungus3_10", "Battle Scene/Completed/Mantis Heavy"));
        AddEnemy("Mantis Petra", "mantis_petra", ("Fungus3_48", "Mantis Heavy Flyer"));

        AddEnemy("Traitor Lord", "traitor_lord",
            ("Fungus3_23_boss", "Battle Scene/Wave 3/Mantis Traitor Lord"),
            preloadAction: o => o.GetComponent<MeshRenderer>().enabled = true,
            postSpawnAction: EnemyFixers.FixTraitorLord);

        AddSolid("Queen's Gardens Platform 1", "qg_plat_1", ("Fungus3_44", "Royal_garden_plat_float_08"));
        AddSolid("Queen's Gardens Platform 2", "qg_plat_2", ("Fungus3_44", "Royal_garden_plat_float_06"));
        
        Categories.Platforming.Add(new PreloadObject("Collapsing Platform S", "collapsing_plat_s",
            ("Fungus3_34", "Royal Gardens Plat S")));
        Categories.Platforming.Add(new PreloadObject("Collapsing Platform L", "collapsing_plat_l",
            ("Fungus3_34", "Royal Gardens Plat L")));

        Categories.Interactable.Add(new PreloadObject("Queen's Gardens Door", "qg_door",
            ("Fungus3_04", "Garden Slide Floor"),
            preloadAction: ObjectUtils.RemoveComponent<PersistentBoolItem>,
            postSpawnAction: o => o.LocateMyFSM("Control").GetState("Tween In").DisableAction(1))
            .WithReceiverGroup(ReceiverGroup.Gate));

        Categories.Misc.Add(new PreloadObject("Cloth (Ally)", "cloth_ally", 
            ("Fungus3_23_boss", "Battle Scene/Cloth Entry/Cloth Fighter"),
            postSpawnAction: MiscFixers.FixCloth)).SpritePreview = true;
    }
    
    private static void AddEdgeObjects()
    {
        AddEnemy("Primal Aspid", "primal_aspid", ("Deepnest_East_07", "Super Spitter"));
        AddEnemy("Belfly", "belfly", ("Deepnest_East_07", "Ceiling Dropper"));
        AddEnemy("Boofly", "boofly", ("Deepnest_East_07", "Blow Fly"));
        
        AddEnemy("Hopper", "hopper", ("Deepnest_East_06", "Hopper"))
            .WithFlipAction(EnemyFixers.FlipHopper);
        AddEnemy("Great Hopper", "great_hopper", ("Deepnest_East_06", "Giant Hopper (1)"))
            .WithFlipAction(EnemyFixers.FlipGreatHopper);
        
        AddSolid("Love Platform", "love_plat_1", ("Ruins2_11_b", "jar_col_plat"));
    }

    private static void AddColosseumObjects()
    {
        AddColoEnemy("Heavy Fool", "heavy_fool", 
            "Colosseum Manager/Waves/Wave 1/Colosseum Cage Large");
        AddColoEnemy("Sturdy Fool", "sturdy_fool", 
            "Colosseum Manager/Waves/Wave 1/Colosseum Cage Large (1)");
        AddColoEnemy("Armoured Squit", "armoured_squit", 
            "Colosseum Manager/Waves/Wave 2/Colosseum Cage Small");
        AddColoEnemy("Shielded Fool", "shielded_fool", 
            "Colosseum Manager/Waves/Wave 3/Colosseum Cage Large");
        AddColoEnemy("Winged Fool", "winged_fool", 
            "Colosseum Manager/Waves/Wave 4/Colosseum Cage Large");
        AddColoEnemy("Sharp Baldur", "sharp_baldur", 
            "Colosseum Manager/Waves/Wave 4/Colosseum Cage Small");
        AddColoEnemy("Battle Obble", "battle_obble", 
            "Colosseum Manager/Waves/Wave 6/Colosseum Cage Small");
        AddColoEnemy("Death Loodle", "death_loodle", 
            "Colosseum Manager/Waves/Wave 9/Colosseum Cage Small (1)");

        AddEnemy("Brooding Mawlek", "brooding_mawlek",
            ("GG_Brooding_Mawlek", "Battle Scene/Mawlek Body"),
            postSpawnAction: EnemyFixers.FixBroodingMawlek);

        AddEnemy("Oblobble", "oblobble", ("GG_Oblobbles", "Mega Fat Bee"),
            preloadAction: MiscFixers.AddComponent<EnemyFixers.Oblobble>)
            .WithConfigGroup(ConfigGroup.Oblobble)
            .WithReceiverGroup(ReceiverGroup.Oblobble);

        AddEnemy("God Tamer Beast", "god_tamer_beast",
            ("Room_Colosseum_Gold", "Colosseum Manager/Waves/Lobster Lancer/Entry Object/Lobster"),
            postSpawnAction: EnemyFixers.FixTamerBeast);

        Categories.Platforming.Add(new PreloadObject("Colosseum Wall", "colo_wall",
            ("Room_Colosseum_Gold", "Colosseum Manager/Walls/Colosseum Wall L"),
            preloadAction: MiscFixers.FixColoWall)
            .WithFlipAction(MiscFixers.FlipColoWall)
            .WithRotateAction(MiscFixers.RotateColoWall)
            .WithRotationGroup(RotationGroup.Four)
            .WithConfigGroup(ConfigGroup.ColoWall)
            .WithReceiverGroup(ReceiverGroup.ColoWall)
            .WithInputGroup(InputGroup.ColoWall));

        Categories.Platforming.Add(new PreloadObject("Colosseum Platform", "colo_plat",
            ("Room_Colosseum_Gold", "Colosseum Manager/Waves/Arena 1/Colosseum Platform"))
            .WithConfigGroup(ConfigGroup.ColoPlat)
            .WithReceiverGroup(ReceiverGroup.ColoPlat));
        
        return;

        void AddColoEnemy(string name, string id, string path)
        {
            Categories.Enemies.Add(new PreloadObject(name, id, ("Room_Colosseum_Gold", path),
                    extraction: ExtractColoObject)
                .WithReceiverGroup(ReceiverGroup.Enemies)
                .WithBroadcasterGroup(BroadcasterGroup.Enemies)
                .WithConfigGroup(ConfigGroup.Enemies)
                .WithOutputGroup(OutputGroup.Enemies));
        }

        GameObject ExtractColoObject(GameObject obj)
        {
            var fsm = obj.LocateMyFSM("Spawn");
        
            var corpse = fsm.FsmVariables.FindFsmGameObject("Corpse to Instantiate");
            return corpse != null ? corpse.value : fsm.FsmVariables.FindFsmGameObject("Enemy Type").value;
        }
    }
    
    private static void AddHiveObjects()
    {
        AddSolid("Hive Platform 1", "hive_plat_1", ("Hive_03_c", "hive_plat_01 (4)"));
        AddSolid("Hive Platform 2", "hive_plat_2", ("Hive_03_c", "hive_plat_02 (2)"));
        AddSolid("Hive Platform 3", "hive_plat_3", ("Hive_03_c", "hive_plat_03 (3)"));
        AddSolid("Hive Platform 4", "hive_plat_4", ("Hive_03_c", "hive_plat_04 (4)"));
        
        AddSolid("Breakable Hive Platform 1", "brk_hive_plat_1", ("Hive_03_c", "hive_plat_brk_02"));
        AddSolid("Breakable Hive Platform 2", "brk_hive_plat_2", ("Hive_03_c", "hive_plat_brk_03 (1)"));
        AddSolid("Breakable Hive Platform 3", "brk_hive_plat_3", ("Hive_03_c", "hive_plat_brk_04"));

        AddEnemy("Hiveling", "hiveling", ("Hive_03_c", "Bee Hatchling Ambient (11)"));
        AddEnemy("Hive Soldier", "hive_soldier", ("Hive_03_c", "Bee Stinger (4)"));
        AddEnemy("Husk Hive", "husk_hive", ("Hive_01", "Zombie Hive"),
                postSpawnAction: EnemyFixers.FixHuskHive)
            .WithScaleAction(EnemyFixers.ScaleHatcher);
        AddEnemy("Hive Guardian", "hive_guardian", ("Hive_03_c", "Big Bee"));

        Categories.Interactable.Add(new PreloadObject("Breakable Hive Wall", "breakable_wall_2",
                ("Hive_03_c", "Hive Breakable Pillar (5)"), preloadAction: MiscFixers.FixBreakableWall)
            .WithConfigGroup(ConfigGroup.PersistentBreakable));
    }

    private static void AddGroundsObjects()
    {
        AddSolid("Resting Grounds Platform", "rest_plat_1", ("RestingGrounds_05", "plat_float_08"));
        
        AddEnemy("Entombed Husk", "entombed_husk", ("RestingGrounds_10", "Grave Zombie"));

        Categories.Interactable.Add(new PreloadObject("Dive Coffin", "collapser_dive_2",
                ("RestingGrounds_05", "rg_coffin_break_front"), postSpawnAction: MiscFixers.FixDiveCoffin)
            .WithConfigGroup(ConfigGroup.PersistentBreakable));
    }
    
    private static void AddBasinObjects()
    {
        AddEnemy("Lesser Mawlek", "lesser_mawlek", ("Abyss_17", "Lesser Mawlek"));
        
        AddEnemy("Mawlurk", "mawlurk", ("Abyss_20", "Mawlek Turret"))
            .WithRotationGroup(RotationGroup.Four)
            .WithRotateAction(EnemyFixers.RotateMawlurk);
        
        AddEnemy("Infected Balloon", "infected_balloon", ("Abyss_20", "Parasite Balloon (6)"),
                preloadAction: MiscFixers.AddComponent<EnemyFixers.InfectedBalloon>)
            .WithConfigGroup(ConfigGroup.Wakeable)
            .WithReceiverGroup(ReceiverGroup.Wakeable);

        AddEnemy("Broken Vessel", "broken_vessel", ("GG_Broken_Vessel", "Infected Knight"),
            postSpawnAction: EnemyFixers.FixNormalBrokenVessel);
        AddEnemy("Lost Kin", "lost_kin", ("Dream_03_Infected_Knight", "Lost Kin"),
            postSpawnAction: EnemyFixers.FixLostKin);
    }
    
    private static void AddWpObjects()
    {
        AddSolid("White Palace Platform 1", "wp_plat_1",
            ("White_Palace_07", "wp_plat_float_01_wide (1)"));
        AddSolid("White Palace Platform 2", "wp_plat_2",
            ("White_Palace_07", "wp_plat_float_07"));
        AddSolid("White Palace Platform 3", "wp_plat_3",
            ("White_Palace_07", "wp_plat_float_03"));
        AddSolid("White Palace Platform 4", "wp_plat_4",
            ("White_Palace_07", "wp_plat_float_05 (1)"));
        Categories.Hazards.Add(new PreloadObject("White Palace Saw", "wp_saw",
            ("White_Palace_07", "wp_saw"))
            .WithConfigGroup(ConfigGroup.WhiteSaw));
        
        Categories.Hazards.Add(new PreloadObject("White Spikes", "white_spikes",
            ("White_Palace_03_hub", "White_ Spikes"))
            .WithRotationGroup(RotationGroup.Four));
        Categories.Hazards.Add(new PreloadObject("White Trap Spikes", "wp_trap_spikes",
                ("White_Palace_07", "wp_trap_spikes"))
            .WithRotationGroup(RotationGroup.Four));
        
        Categories.Enemies.Add(new PreloadObject("Wingsmould", "wingsmould", 
            ("White_Palace_18", "White Palace Fly"))
            .WithConfigGroup(ConfigGroup.Wingsmould)
            .WithReceiverGroup(ReceiverGroup.Enemies)
            .WithBroadcasterGroup(BroadcasterGroup.Damageable));
        AddEnemy("Kingsmould", "kingsmould", ("White_Palace_20", "Battle Scene/Wave 1/Royal Gaurd (1)"));

        Categories.Interactable.Add(new PreloadObject("White Palace Gate", "wp_gate",
            ("White_Palace_20", "Battle Scene/Gate"),
            preloadAction: o =>
            {
                var col = o.GetComponent<BoxCollider2D>();
                col.offset = new Vector2(0.25f, 0.2165f);
                col.size = new Vector2(0.25f, 3.5f);
            }, postSpawnAction: o =>
            {
                var fsm = o.LocateMyFSM("FSM");
                var bc2d = o.GetComponent<BoxCollider2D>();
                fsm.GetState("Downed").AddAction(() => bc2d.enabled = false);
                fsm.GetState("Upped").AddAction(() => bc2d.enabled = true);
            }).WithReceiverGroup(ReceiverGroup.WpGate)
            .WithConfigGroup(ConfigGroup.WpGate));

        Categories.Platforming.Add(new PreloadObject("White Palace Lift", "wp_lift",
            ("White_Palace_03_hub", "White Palace Lift"),
            preloadAction: MiscFixers.AddComponent<MiscFixers.WpLift>)
            .WithConfigGroup(ConfigGroup.WpLift));
    }

    private static void AddDreamObjects()
    {
        Categories.Platforming.Add(new PreloadObject("Disappearing Platform 1", "plat_hide_1",
            ("Dream_03_Infected_Knight", "dream_plat_small (1)")));
        Categories.Platforming.Add(new PreloadObject("Disappearing Platform 2", "plat_hide_2",
            ("Dream_03_Infected_Knight", "dream_plat_med (7)")));
        Categories.Platforming.Add(new PreloadObject("Disappearing Platform 3", "plat_hide_3",
            ("Dream_03_Infected_Knight", "dream_plat_large (3)")));
        
        Categories.Platforming.Add(new PreloadObject("Radiant Platform 1", "plat_rad_1",
            ("GG_Radiance", "Boss Control/Plat Sets/P2 SetA/Radiant Plat Small (2)"),
            sprite: ResourceUtils.LoadSpriteResource("radiance_small", ppu: 64))
            .WithConfigGroup(ConfigGroup.RadiancePlat)
            .WithReceiverGroup(ReceiverGroup.RadiancePlat));
        Categories.Platforming.Add(new PreloadObject("Radiant Platform 2", "plat_rad_2",
            ("GG_Radiance", "Boss Control/Plat Sets/P2 SetA/Radiant Plat Wide (2)"),
            sprite: ResourceUtils.LoadSpriteResource("radiance_wide", ppu: 64))
            .WithConfigGroup(ConfigGroup.RadiancePlat)
            .WithReceiverGroup(ReceiverGroup.RadiancePlat));
        Categories.Platforming.Add(new PreloadObject("Radiant Platform 3", "plat_rad_3",
            ("GG_Radiance", "Boss Control/Plat Sets/P2 SetA/Radiant Plat Thick (2)"),
            sprite: ResourceUtils.LoadSpriteResource("radiance_thick", ppu: 64))
            .WithConfigGroup(ConfigGroup.RadiancePlat)
            .WithReceiverGroup(ReceiverGroup.RadiancePlat));

        AddSolid("Godhome Platform 1", "gg_plat_1", ("GG_Atrium_Roof", "gg_plat_float_small"));
        AddSolid("Godhome Platform 2", "gg_plat_2", ("GG_Workshop", "gg_plat_float_wide"));
    }

    private static void AddAbyssObjects()
    {
        AddEnemy("Shadow Creeper", "shadow_creeper", ("Abyss_04", "Abyss Crawler"))
            .WithRotationGroup(RotationGroup.Four);

        AddSolid("Abyss Platform 1", "plat_abyss_1", ("Abyss_06_Core", "_Scenery/abyss_plat_float_01"));
        AddSolid("Abyss Platform 2", "plat_abyss_2", ("Abyss_06_Core", "_Scenery/abyss_plat_float_02"));
        AddSolid("Abyss Platform 3", "plat_abyss_3", ("Abyss_06_Core", "_Scenery/abyss_plat_float_03"));
        AddSolid("Abyss Platform 4", "plat_abyss_4", ("Abyss_06_Core", "_Scenery/abyss_plat_float_04"));

        Categories.Interactable.Add(new PreloadObject("Shade Gate", "shade_gate",
            ("Fungus3_44", "shadow_gate"),
            preloadAction: o =>
            {
                o.transform.GetChild(1).gameObject.SetActive(false);
                o.transform.GetChild(5).gameObject.SetActive(false);
                foreach (Transform child in o.transform) child.SetLocalPositionZ(child.localPosition.z + 2.7657f);
                o.transform.SetLocalPositionZ(o.transform.localPosition.z - 2.7657f);
            },
            uiSprite: ResourceUtils.LoadSpriteResource("shade_gate", new Vector2(0.66f, 0.227f), ppu: 64))
            .WithRotationGroup(RotationGroup.Eight));

        Categories.Enemies.Add(new PreloadObject("Shade", "shade",
            ("Menu_Title", "_SceneManager"),
            extraction: o => o.GetComponent<SceneManager>().hollowShadeObject,
            preloadAction: MiscFixers.AddComponent<EnemyFixers.Shade>)
            .WithConfigGroup(ConfigGroup.Shade)
            .WithReceiverGroup(ReceiverGroup.Enemies)
            .WithBroadcasterGroup(BroadcasterGroup.Shades)
            .WithOutputGroup(OutputGroup.Enemies));

        Categories.Enemies.Add(new PreloadObject("Shade Sibling", "shade_sibling",
            ("Abyss_06_Core", "Shade Sibling Spawner"), 
            extraction: o => o.GetComponent<PersonalObjectPool>().startupPool[0].prefab,
            postSpawnAction: o => o.transform.SetPositionY(o.transform.GetPositionY() - 6.5f))
            .WithConfigGroup(ConfigGroup.ShadeSibling)
            .WithReceiverGroup(ReceiverGroup.Enemies)
            .WithBroadcasterGroup(BroadcasterGroup.Shades)
            .WithOutputGroup(OutputGroup.Enemies));
        
        Categories.Hazards.Add(new PreloadObject("Void Tendrils", "void_tendrils",
            ("Abyss_16", "Abyss Tendrils"))
            .WithConfigGroup(ConfigGroup.AbyssTendrils));
    }

    private static void AddSoulObjects()
    {
        AddSoulTotem("Mini Soul Totem", "mini_totem",
            ("Mines_31", "Soul Totem mini_horned"));
        AddSoulTotem("Horned Soul Totem", "horned_totem",
            ("Mines_31", "Soul Totem mini_two_horned"));
        AddSoulTotem("Ancestral Mound Soul Totem", "mound_totem",
            ("Crossroads_ShamanTemple", "Soul Totem 2"));
        AddSoulTotem("Angry Soul Totem", "angry_totem",
            ("Crossroads_36", "_Props/Soul Totem 4"));
        AddSoulTotem("Thin Soul Totem", "thin_totem",
            ("Ruins1_24", "Soul Totem 1"));
        AddSoulTotem("Round Soul Totem", "round_totem",
            ("Ruins1_32", "Soul Totem 3"));
        AddSoulTotem("Leaning Soul Totem", "leaning_totem",
            ("Ruins1_32", "Soul Totem 5"));
        AddSoulTotem("Pale Soul Totem", "pale_totem",
            ("White_Palace_03_hub", "Soul Totem white"));
        AddSoulTotem("Pure Vessel Soul Totem", "pv_totem",
            ("White_Palace_18", "Soul Totem white_Infinte"));

        AddRetainer("Royal Retainer 1", "white_servant_1", "White_Servant_01");
        AddRetainer("Royal Retainer 2", "white_servant_2", "White_Servant_02");
        AddRetainer("Royal Retainer 3", "white_servant_3", "White_Servant_03");

        Categories.Interactable.Add(new PreloadObject("Soul Vial", "soul_vial",
            ("Ruins1_25", "Ruins Vial Empty"))
            .WithConfigGroup(ConfigGroup.PersistentBreakable)
            .WithBroadcasterGroup(BroadcasterGroup.Breakable));
        
        return;
        
        void AddSoulTotem(string name, string id, (string, string) path)
        {
            Categories.Interactable.Add(new PreloadObject(name, id, path,
                    postSpawnAction: InteractableFixers.FixSoulTotem)
                .WithRotationGroup(RotationGroup.Eight)
                .WithBroadcasterGroup(BroadcasterGroup.Hittable));
        }

        void AddRetainer(string name, string id, string path)
        {
            Categories.Interactable.Add(new PreloadObject(name, id, ("White_Palace_03_hub", path)));
        }
    }

    private static void AddNpcObjects()
    {
        Categories.Npcs.Add(new PreloadObject("Zote NPC", "zote_town", 
                ("Town", "_NPCs/Zote Town"), 
                preloadAction: MiscFixers.FixZote)
            .WithConfigGroup(ConfigGroup.Npcs)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs));
        
        Categories.Npcs.Add(new PreloadObject("Elderbug NPC", "elderbug", 
                ("Town", "_NPCs/Elderbug"), 
                preloadAction: MiscFixers.AddComponent<MiscFixers.Elderbug>)
            .WithConfigGroup(ConfigGroup.Elderbug)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs));

        Categories.Npcs.Add(new PreloadObject("Quirrel NPC", "quirrel",
                ("Room_temple", "Quirrel"),
                preloadAction: MiscFixers.FixNpc<MiscFixers.Quirrel>)
            .WithConfigGroup(ConfigGroup.Npcs)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs));

        Categories.Npcs.Add(new PreloadObject("Hornet NPC", "hornet_npc",
                ("Abyss_06_Core", "Hornet Abyss NPC"),
                preloadAction: MiscFixers.FixNpc<MiscFixers.Hornet>)
            .WithConfigGroup(ConfigGroup.Npcs)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs));
        
        Categories.Npcs.Add(new PreloadObject("Midwife NPC", "midwife",
                ("Deepnest_41", "Happy Spider NPC"),
                preloadAction: MiscFixers.AddComponent<MiscFixers.Midwife>)
            .WithConfigGroup(ConfigGroup.Midwife)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs)
            .WithFlipAction(MiscFixers.FlipMidwife));
        
        Categories.Npcs.Add(new PreloadObject("Tiso NPC", "tiso", 
                ("Town", "_NPCs/Tiso Town NPC"), 
                preloadAction: MiscFixers.FixTiso)
            .WithConfigGroup(ConfigGroup.Npcs)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs));
        
        Categories.Npcs.Add(new PreloadObject("Godseeker NPC", "godseeker", 
                ("GG_Engine", "Godseeker EngineRoom NPC"), 
                preloadAction: MiscFixers.AddComponent<MiscFixers.Godseeker>)
            .WithConfigGroup(ConfigGroup.Npcs)
            .WithBroadcasterGroup(BroadcasterGroup.Npcs));
    }
    
    private static void AddOrdealObjects()
    {
        AddEnemy("Zoteling the Mighty", "zoteling",
            ("GG_Mighty_Zote", "Battle Control/Dormant Warriors/Zote Crew Normal (1)"),
            postSpawnAction: EnemyFixers.FixZoteling);
        
        AddEnemy("Winged Zoteling", "zoteling_winged",
            ("GG_Mighty_Zote", "Battle Control/Zotelings/Ordeal Zoteling"),
            postSpawnAction: EnemyFixers.FixWingedZoteling);
        
        AddEnemy("Hopping Zoteling", "zoteling_hop",
            ("GG_Mighty_Zote", "Battle Control/Zotelings/Ordeal Zoteling"),
            postSpawnAction: EnemyFixers.FixHopZoteling);
        
        AddEnemy("Heavy Zoteling", "zoteling_heavy",
            ("GG_Mighty_Zote", "Battle Control/Fat Zotes/Zote Crew Fat (1)"),
            postSpawnAction: EnemyFixers.FixHeavyZoteling);
        
        AddEnemy("Lanky Zoteling", "zoteling_lanky",
            ("GG_Mighty_Zote", "Battle Control/Tall Zotes/Zote Crew Tall (1)"),
            postSpawnAction: EnemyFixers.FixLankyZoteling);
        
        AddEnemy("Volatile Zoteling", "zoteling_volatile",
            ("GG_Mighty_Zote", "Battle Control/Zote Balloon Ordeal"),
            postSpawnAction: EnemyFixers.FixVolatileZoteling);
        
        AddEnemy("Fluke Zoteling", "zoteling_fluke",
            ("GG_Mighty_Zote", "Battle Control/Zote Fluke"),
            postSpawnAction: EnemyFixers.FixFlukeZoteling);
        
        /*
        AddEnemy("Grey Prince Zote", "grey_prince_zote",
            ("Dream_Mighty_Zote", "Grey Prince"),
            postSpawnAction: EnemyFixers.FixZote);*/

        /*
        Categories.Misc.Add(new PreloadObject("Score Counter", "score_counter",
            ("GG_Mighty_Zote", "Battle Control"),
            extraction: o =>
            {
                var counter = new GameObject("Score Counter");
                counter.SetActive(false);
                Object.DontDestroyOnLoad(counter);

                Object.Instantiate(o.transform.Find("Counter Icon").gameObject, counter.transform, true);
                Object.Instantiate(o.transform.Find("Counter Text").gameObject, counter.transform, true);
                Object.Instantiate(o.transform.Find("Counter Flash").gameObject, counter.transform, true);

                return counter;
            }, sprite: ResourceUtils.LoadSpriteResource("zote_counter", ppu:64)));*/
    }
    
    private static void AddGhostObjects()
    {
        AddEnemy("Marmu", "marmu",
            ("GG_Ghost_Marmu", "Warrior/Ghost Warrior Marmu"),
            postSpawnAction: EnemyFixers.FixMarmu);
    }
    
    private static PlaceableObject AddEnemy(
        string name,
        string id,
        (string, string) path, 
        string description = null, 
        bool notSceneBundle = false,
        [CanBeNull] Action<GameObject> preloadAction = null,
        [CanBeNull] Action<GameObject> postSpawnAction = null)
    {
        return Categories.Enemies.Add(new PreloadObject(name, id,
                path,
                description,
                notSceneBundle: notSceneBundle,
                preloadAction: preloadAction,
                postSpawnAction: postSpawnAction)
            .WithReceiverGroup(ReceiverGroup.Enemies)
            .WithBroadcasterGroup(BroadcasterGroup.Enemies)
            .WithConfigGroup(ConfigGroup.Enemies)
            .WithOutputGroup(OutputGroup.Enemies));
    }

    private static void AddSolid(string name, string id, (string, string) path,
        [CanBeNull] Action<GameObject> preloadAction = null)
    {
        Categories.Solids.Add(new PreloadObject(name, id, path, preloadAction: preloadAction))
            .WithRotationGroup(RotationGroup.Four)
            .WithConfigGroup(ConfigGroup.Colliders);
    }
}
