using System;
using Architect.Utils;
using UnityEngine;

namespace Architect.Behaviour.Utility;

public class LifebloodState : MonoBehaviour
{
  public int healAmount = 5;
  private int _maxHp;
  private float _timer;
  private HealthManager _healthManager;
  private SpriteFlash _spriteFlash;

  public static void Init()
  {
    typeof(HealthManager).Hook(nameof(HealthManager.TakeDamage),
      (Action<HealthManager, HitInstance> orig, HealthManager self, HitInstance hit) =>
      {
        orig(self, hit);
        if (self && self.hp > 0)
        {
          var lbs = self.GetComponent<LifebloodState>();
          if (lbs) lbs.TakeDamage();
        }
      });
  }

  private void Start()
  {
    _healthManager = gameObject.GetComponent<HealthManager>();
    _spriteFlash = gameObject.GetComponent<SpriteFlash>();
    _maxHp = _healthManager.hp;
  }

  private void Update()
  {
    var hp = _healthManager.hp;
    if (hp < _maxHp)
    {
      if (_timer < 0.75)
      {
        _timer += Time.deltaTime;
      }
      else
      {
        int num = hp + healAmount;
        if (num > _maxHp)
          num = _maxHp;
        _healthManager.hp = num;
        _spriteFlash.flashHealBlue();
        _timer -= 0.75f;
      }
    }
    else
      _timer = 0.0f;
  }

  private void TakeDamage() => _timer = 0.0f;
}
