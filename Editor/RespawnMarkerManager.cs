using System;
using Architect.Storage;
using Architect.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architect.Editor;

public static class RespawnMarkerManager
{
    private static GameObject _marker;
    private static GameObject _icon;
    
    public static void Init()
    {
        _marker = new GameObject("[Architect] Respawn Marker")
        {
            transform = { position = new Vector3(0, 0, 0.005f) }
        };
        
        _marker.SetActive(false);
        Object.DontDestroyOnLoad(_marker);
        
        _icon = new GameObject("Icon")
        {
            transform =
            {
                parent = _marker.transform,
                localPosition = Vector3.zero
            }
        };

        _marker.AddComponent<SpriteRenderer>().sprite = ResourceUtils.LoadSpriteResource("respawn_text", ppu: 64);
        _icon.AddComponent<SpriteRenderer>().sprite = ResourceUtils.LoadSpriteResource("respawn_marker", ppu: 80);

        typeof(HeroController).Hook("Awake",
            (Action<HeroController> orig, HeroController self) =>
            {
                orig(self);
                self.gameObject.AddComponent<RespawnPreview>();
            });
    }

    public class RespawnPreview : MonoBehaviour
    {
        private PlayerData _pd;
        private Vector3 _lastPos;
        private bool _lastLeft;

        private bool _showPoint;
        
        private void Start()
        {
            _pd = PlayerData.instance;
        }

        private void Update()
        {
            var show = Settings.ShowRespawnPoint.Value;
            if (show != _showPoint)
            {
                _marker.SetActive(show);
                _showPoint = show;
            }

            if (!_showPoint) return;

            var facingLeft = !_pd.hazardRespawnFacingRight;
            if (facingLeft == _lastLeft && _pd.hazardRespawnLocation == _lastPos) return;

            _lastLeft = facingLeft;
            _lastPos = _pd.hazardRespawnLocation;

            var point = HeroController.instance.FindGroundPoint(_lastPos);
            
            _marker.transform.SetPositionX(point.x);
            _marker.transform.SetPositionY(point.y - 0.58f);
            _icon.transform.SetScaleX(facingLeft ? 1 : -1);
        }
        
        private void OnDisable()
        {
            if (_marker) _marker.SetActive(false);
        }
    }
}