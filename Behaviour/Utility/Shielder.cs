using System;
using System.Collections.Generic;
using Architect.Placements;
using Architect.Utils;
using UnityEngine;

namespace Architect.Behaviour.Utility;

public class Shielder : MonoBehaviour
{
    public string id;

    public bool startApplied = true;

    public static void Init()
    {
        typeof(HealthManager).Hook(nameof(HealthManager.IsBlockingByDirection),
            (Func<HealthManager, int, AttackTypes, bool> orig,
                HealthManager self,
                int direction,
                AttackTypes hit) =>
            {
                var ext = self.GetComponent<ExtraResistance>();
                if (ext && ext.resistances.Contains(hit)) return true;
                return orig(self, direction, hit);
            });
    }

    public List<AttackTypes> extraImmunities = [];

    private bool _shielded;

    public void Shield()
    {
        if (!PlacementManager.TryGetValue(id, out var target)) return;
        
        Shield(target);
    }

    public void Shield(GameObject target)
    {
        var hm = target.GetComponentInChildren<HealthManager>();
        if (!hm) return;

        if (!extraImmunities.IsNullOrEmpty())
            hm.gameObject.GetOrAddComponent<ExtraResistance>().resistances = extraImmunities;
    }

    public class ExtraResistance : MonoBehaviour
    {
        public List<AttackTypes> resistances;
    }

    private void Update()
    {
        if (!startApplied) return;
        if (!_shielded)
        {
            _shielded = true;
            Shield();
        }
    }
}