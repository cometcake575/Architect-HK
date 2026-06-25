using System.Collections;
using System.Collections.Generic;
using Architect.Content.Preloads;
using Newtonsoft.Json;
using UnityEngine;

namespace Architect.Behaviour.Utility;

public class ObjectExtractor : PreviewableBehaviour
{
    public string sceneName = string.Empty;
    public string path = string.Empty;

    public GameObject spawn;

    private bool _needsToSpawn;
    private bool _spawned;

    private static readonly Dictionary<string, HashSet<string>> Assets = [];
    private static bool _shouldRepack = true;
    private static AssetBundle _bundle;
    private static bool _bundleReady;
    
    public static void Init()
    {
        On.HeroController.SceneInit += (orig, self) =>
        {
            orig(self);
            Assets.Clear();
            if (_bundle)
            {
                _bundle.Unload(true);
                _bundle = null;
            }

            _bundleReady = false;
        };

        ModHooks.HeroUpdateHook += () =>
        {
            if (!_shouldRepack) return;
            _shouldRepack = false;
            
            ArchitectPlugin.Instance.StartCoroutine(Repack());
        };
    }

    private void Start()
    {
        if (isAPreview) return;
        if (!Assets.TryGetValue(sceneName, out var scene)) Assets[sceneName] = scene = [];
        if (scene.Add(path))
        {
            _shouldRepack = true;
            _bundleReady = false;
            _needsToSpawn = true;
        }
    }

    private void Update()
    {
        if (_spawned || !_needsToSpawn) return;
        if (!_bundleReady) return;
        
        _spawned = true;
        _needsToSpawn = false; 
        var assetName = $"{sceneName}/{path}.prefab";
        StartCoroutine(Spawn(assetName));
    }

    private IEnumerator Spawn(string assetName)
    {
        var op = _bundle.LoadAssetAsync<GameObject>(assetName);
        yield return op;
        if (op.GetResult() is not GameObject obj) yield break;
        
        spawn = Instantiate(obj, transform.position, transform.rotation);
        spawn.name = name;
        spawn.SetActive(true);
    }

    private static IEnumerator Repack()
    {
        var preloadJson = JsonConvert.SerializeObject(Assets);
        
        var (bundleData, _) = UnitySceneRepacker.Repack(
            $"{PreloadManager.PRELOAD_BUNDLE_NAME}_extractor_{GameManager.instance.sceneName}",
            Preloader.DataPath,
            preloadJson,
            UnitySceneRepacker.Mode.AssetBundle
        );
        
        var op = AssetBundle.LoadFromMemoryAsync(bundleData);
        yield return op;

        _bundle = op.assetBundle;
        _bundleReady = true;
    }
}