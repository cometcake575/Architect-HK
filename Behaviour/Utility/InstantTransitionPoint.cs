using System.Collections;
using GlobalEnums;
using UnityEngine;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Architect.Behaviour.Utility;

public class InstantTransitionPoint : PreviewableBehaviour
{
    public static readonly Sprite OnSprite = ResourceUtils.LoadSpriteResource("Crystals.tp_on", FilterMode.Point, ppu:15);
    private static readonly Sprite OffSprite = ResourceUtils.LoadSpriteResource("Crystals.tp_off", FilterMode.Point, ppu:15);

    private static readonly Color OnColour = Color.white;
    private static readonly Color OffColour = new(1, 1, 1, 0.5f);
    
    public string id = string.Empty;
    public string targetScene = string.Empty;
    public string targetId = string.Empty;

    private static string _teleportingTo;
    private static bool _teleporting;

    private SpriteRenderer _renderer;
    private Collider2D _collider;

    public static void Init()
    {
        On.GameManager.LevelActivated += (orig, self, from, to) =>
        {
            if (_teleporting)
            {
                if (self.startedOnThisScene && self.IsGameplayScene()) self.tilemapDirty = true;
                self.SetupSceneRefs(true);
            } else orig(self, from, to);
        };
    }

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        if (targetScene.IsNullOrWhiteSpace()) targetScene = gameObject.scene.name;
    }

    private void Update()
    {
        if (!_teleporting || _teleportingTo != id) return;

        _teleporting = false;
        
        // Teleport end
        var hc = HeroController.instance;
        hc.transform.position = transform.position;
        hc.SetHazardRespawn(transform.position, true);
        hc.SetStartingMotionState(false);
        hc.AffectedByGravity(true);
        hc.acceptingInput = true;

        StartCoroutine(Deactivate());
    }

    private void OnTriggerEnter2D(Collider2D other) => Teleport();

    private void OnTriggerStay2D(Collider2D other) => Teleport();

    private IEnumerator Deactivate()
    {
        _renderer.sprite = OffSprite;
        _renderer.color = OffColour;
        _collider.enabled = false;
        yield return new WaitForSeconds(2);
        _renderer.sprite = OnSprite;
        _renderer.color = OnColour;
        _collider.enabled = true;
    }

    public void Teleport()
    {
        if (isAPreview || _teleporting) return;
        _teleporting = true;
        _teleportingTo = targetId;
        StartCoroutine(Deactivate());
        StartCoroutine(DoTeleport());
    }

    private IEnumerator DoTeleport()
    {
        if (GameManager.instance.isPaused) 
            yield return GameManager.instance.PauseGameToggle();

        // Prepare teleport
        var hc = HeroController.instance;
        hc.acceptingInput = false;
        hc.ResetMotion();
        hc.airDashed = false;
        hc.doubleJumped = false;
        hc.ResetHardLandingTimer();
        hc.ResetAttacksDash();
        hc.AffectedByGravity(false);

        // Change scenes if needed
        var gm = GameManager.instance;
        if (gm.sceneName != targetScene)
        {
            var gc = GameCameras.instance;
            gc.cameraController.FreezeInPlace(true);
            gc.cameraController.FadeOut(CameraFadeType.LEVEL_TRANSITION);
            
            var load = new SceneLoad(gm, targetScene)
            {
                IsFetchAllowed = true,
                IsActivationAllowed = true
            };

            var lastSceneName = gm.sceneName;
            load.ActivationComplete += () =>
            {
                USceneManager.UnloadSceneAsync(lastSceneName);
                gm.SetupSceneRefs(true);
                gm.BeginScene();
                
                if (gm.gameMap) gm.gameMap.GetComponent<GameMap>().LevelReady();

                hc.vignetteFSM.SendEvent("RESET");
                hc.cState.transitioning = false;
                hc.transitionState = HeroTransitionState.WAITING_TO_TRANSITION;
                
                gm.SetState(GameState.PLAYING);
                gm.hasFinishedEnteringScene = true;

                gc.SceneInit();
                gc.cameraController.FadeSceneIn();
                
                gc.sceneParticles.BeginScene();
            };
            
            load.Begin();
        }
    }
}