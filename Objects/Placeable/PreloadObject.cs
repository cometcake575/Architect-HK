using System;
using System.Collections;
using Architect.Content.Preloads;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architect.Objects.Placeable;

public class PreloadObject : PlaceableObject, IPreload
{
    public bool Loaded { get; protected set; }
    public string Scene { get; }
    public string Path { get; }

    public void MarkLoaded() { }

    public bool IsNotSceneBundle { get; }

    public bool ShouldLoad;
    public bool ShouldAlwaysLoad => true;

    private readonly Action<GameObject> _preloadAction;
    private readonly Func<GameObject, GameObject> _extraction;

    public PreloadObject(
        string name, 
        string id, 
        (string, string) path,
        string description = null,
        Action<GameObject> postSpawnAction = null, 
        Action<GameObject> preloadAction = null,
        Func<GameObject, GameObject> extraction = null,
        bool notSceneBundle = false,
        Sprite sprite = null,
        Sprite uiSprite = null)
        : base(name, id, description, postSpawnAction, sprite, uiSprite)
    {
        Scene = path.Item1;
        Path = path.Item2;
        IsNotSceneBundle = notSceneBundle;

        _preloadAction = preloadAction;
        _extraction = extraction;
        
        PreloadManager.RegisterPreload(this);
    }

    public override IEnumerator EnsureLoaded()
    {
        Loaded = true;
        yield break;/*
        if (Loaded) yield break;

        PreloadManager.IsLoading = true;
        yield return _asset.Load();
        if (_asset.Handle.OperationException != null || Loaded)
        {
            PreloadManager.IsLoading = false;
            yield break;
        }

        Loaded = true;
        OnPreload(_asset.Handle.Result);
        PreloadManager.IsLoading = false;*/
    }

    public virtual void OnPreload(GameObject preload)
    {
        if (IsNotSceneBundle && preload.GetComponent<HealthManager>())
        {
            var active = preload.activeSelf;
            preload.SetActive(false);
            var p = preload;
            preload = Object.Instantiate(preload);
            Object.DontDestroyOnLoad(preload);
            if (active) p.SetActive(true);
        }
        
        if (_extraction != null) preload = _extraction(preload);
        
        _preloadAction?.Invoke(preload);
        FinishSetup(preload);
        Loaded = true;
    }
}