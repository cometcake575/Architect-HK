using System;
using Architect.Utils;

namespace Architect.Events;

public static class BroadcasterHooks
{
    public static void Init()
    {
        typeof(HealthManager).Hook("TakeDamage", 
            (Action<HealthManager, HitInstance> orig, HealthManager self, HitInstance hitInstance) => 
            {
                orig(self, hitInstance);
                EventManager.BroadcastEvent(self.gameObject, "OnDamage");
            }
        );

        On.Breakable.Break += (orig, self, min, max, multiplier) =>
        {
            orig(self, min, max, multiplier);
            self.gameObject.BroadcastEvent("OnBreak");
        };
        
        typeof(HealthManager).Hook(nameof(HealthManager.Die),
            (Action<HealthManager, float?, AttackTypes, bool> orig,
                HealthManager self,
                float? attackDirection, AttackTypes attackType, bool ignoreEvasion) =>
            {
                var dead = self.isDead;
                
                orig(self, attackDirection, attackType, ignoreEvasion);

                if (dead) return;
                
                self.gameObject.BroadcastEvent("OnDeath");
                self.gameObject.BroadcastEvent("FirstDeath");
            }
        );
        
        typeof(HealthManager).Hook("Awake",
            (Action<HealthManager> orig, HealthManager self) =>
            {
                var component = self.GetComponent<PersistentBoolItem>();
                if (component)
                {
                    component.OnSetSaveState += value =>
                    {
                        if (value) self.gameObject.BroadcastEvent("OnDeath");
                        if (value) self.gameObject.BroadcastEvent("LoadedDead");
                    };
                }

                orig(self);
            }
        );
    }
}