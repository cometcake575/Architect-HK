using System.Collections.Generic;
using UnityEngine;

namespace Architect.Behaviour.Custom;

public class Bumper : SoundMaker, IHitResponder
{
    public static readonly Sprite NormalIcon = 
        ResourceUtils.LoadSpriteResource("Bumper.Normal.icon", FilterMode.Point, ppu:15);
    public static readonly Sprite EvilIcon = 
        ResourceUtils.LoadSpriteResource("Bumper.Evil.icon", FilterMode.Point, ppu:15);

    private static AudioClip _hitNormal;
    private static AudioClip _hitEvil;

    public int activation;

    public static void Init()
    {
        ResourceUtils.LoadClipResource("Bumper.Normal.hit", clip => _hitNormal = clip);
        ResourceUtils.LoadClipResource("Bumper.Evil.hit", clip => _hitEvil = clip);

        ModHooks.HeroUpdateHook += () =>
        {
            for (var i = DecayingVelocities.Count - 1; i >= 0; i--)
            {
                var dv = DecayingVelocities[i];
                
                dv.Velocity -= dv.Velocity * (dv.Decay * Time.deltaTime);
                if (dv.Velocity.magnitude < 0.01f) DecayingVelocities.RemoveAt(i);
                else DecayingVelocities[i] = dv;
            }
        };

        On.HeroController.Move += (orig, self, direction) =>
        {
            orig(self, direction);
            foreach (var dv in DecayingVelocities) self.rb2d.velocity += dv.Velocity;
        };

        ModHooks.TakeHealthHook += damage =>
        {
            if (damage > 0) DecayingVelocities.Clear();
            return damage;
        };

        On.HeroController.BackOnGround += (orig, self) =>
        {
            orig(self);
            DecayingVelocities.Clear();
        };
    }
    
    private static readonly Sprite[][] Normal = LoadGroups("Bumper.Normal");
    private static readonly Sprite[][] Evil = LoadGroups("Bumper.Evil");
    
    private static Sprite[][] LoadGroups(string path)
    {
        return
        [
            LoadSprites($"{path}.Idle", 32),
            LoadSprites($"{path}.Hit", 9),
            LoadSprites($"{path}.Off", 1),
            LoadSprites($"{path}.On", 3)
        ];
    }

    private static Sprite[] LoadSprites(string path, int count)
    {
        var sprites = new Sprite[count];
        for (var c = 0; c < count; c++) 
            sprites[c] = ResourceUtils.LoadSpriteResource($"{path}.f{c}", FilterMode.Point, ppu:15);
        return sprites;
    }
    
    private SpriteRenderer _renderer;
    private CircleCollider2D _col2d;

    private float _frame;
    private bool _evil;
    private int _stage;

    public override void Awake()
    {
        base.Awake();
        _renderer = GetComponent<SpriteRenderer>();
        _col2d = GetComponent<CircleCollider2D>();
    }

    public void SetEvil(bool newEvil)
    {
        _evil = newEvil;
        transform.GetChild(0).gameObject.SetActive(_evil);
    }

    private void Update()
    {
        var sprites = (_evil ? Evil : Normal)[_stage];
        _renderer.sprite = sprites[Mathf.FloorToInt(_frame)];
        
        _frame += Time.deltaTime * 15;

        if (_frame >= sprites.Length)
        {
            switch (_stage)
            {
                case 1:
                case 2:
                    _stage++;
                    break;
                case 3:
                    _stage = 0;
                    _col2d.enabled = true;
                    transform.GetChild(0).gameObject.SetActive(_evil);
                    break;
            }

            _frame %= Normal[_stage].Length;
        }
    }

    public void Hit(HitInstance damageInstance)
    {
        if (damageInstance.AttackType == AttackTypes.Nail)
        {
            DoBounce(damageInstance.Direction);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activation == 1) return;
        if (_evil) return;
        if (!other.GetComponent<HeroController>()) return;
        
        var angle = Mathf.Atan2(
            transform.position.y - other.transform.position.y, 
            transform.position.x - other.transform.position.x) * Mathf.Rad2Deg;
        angle = RoundToMultipleOf(angle, 90);
        DoBounce(angle);
    }

    private static float RoundToMultipleOf(float value, float factor)
    {
        return Mathf.Round(value / factor) * factor;
    }

    private void DoBounce(float direction)
    {
        PlaySound(_evil ? _hitEvil : _hitNormal, pitch: Random.Range(0.8f, 1) - (_evil ? 0.2f : 0), volume: _evil ? 25 : 5);
        
        _col2d.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        _frame = 0;
        _stage = 1;
        
        var hero = HeroController.instance;
        
        hero.doubleJumped = false;
        hero.airDashed = false;
        
        var velocity = Quaternion.Euler(0, 0, direction)
                        * new Vector2(-20, 0)
                       * (_evil ? 0.36f : 1)
                       + new Vector3(0, 10, 0);

        Wind.ActuallyJumping = false;
        hero.rb2d.SetVelocityY(velocity.y);

        velocity.x *= _evil ? 1 : 1.75f;
        hero.rb2d.SetVelocityX(velocity.x);

        var dv = new DecayingVelocity
        {
            Velocity = new Vector2(velocity.x, 0),
            Decay = 3
        };
        DecayingVelocities.Add(dv);
    }

    private static readonly List<DecayingVelocity> DecayingVelocities = [];
    
    public struct DecayingVelocity
    {
        public Vector2 Velocity;
        public float Decay;
    }
}   