using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GlobalEnums;
using JetBrains.Annotations;
using UnityEngine;

namespace Architect.Utils;

public static class MiscUtils
{
    public static IEnumerator FreeControl(this HeroController hero, Predicate<HeroController> condition = null)
    {
        yield return new WaitUntil(() => !hero.controlReqlinquished &&
                                         HeroController.instance.transitionState ==
                                         HeroTransitionState.WAITING_TO_TRANSITION &&
                                         (condition == null || condition.Invoke(hero)));
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
    {
        return collection == null || collection.Count == 0;
    }

    public static Vector3 Where(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
    }

    public static Vector2 Where(this Vector2 original, float? x = null, float? y = null)
    {
        return new Vector2(x ?? original.x, y ?? original.y);
    }

    public static Color Where(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
    }

    public static void SetLocalPositionX(this Transform t, float newX)
    {
        var vector3 = t.localPosition;
        vector3 = new Vector3(newX, vector3.y, vector3.z);
        t.localPosition = vector3;
    }

    public static void SetLocalPositionY(this Transform t, float newY)
    {
        var pos = t.localPosition;
        pos.y = newY;
        t.localPosition = pos;
    }

    public static void SetLocalPositionZ(this Transform t, float newZ)
    {
        var vector3 = t.localPosition;
        vector3 = new Vector3(vector3.x, vector3.y, newZ);
        t.localPosition = vector3;
    }

    public static T GetRandomElement<T>(this T[] array)
    {
        return array == null || array.Length == 0 ? default : array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static void SetVelocityX(this Rigidbody2D rb2d, float x)
    {
        var vel = rb2d.velocity;
        vel.x = x;
        rb2d.velocity = vel;
    }

    public static void SetVelocityY(this Rigidbody2D rb2d, float y)
    {
        var vel = rb2d.velocity;
        vel.y = y;
        rb2d.velocity = vel;
    }

    public static T2 GetValueOrDefault<T1, T2>(this Dictionary<T1, T2> d, T1 key, T2 val = default)
    {
        return d.TryGetValue(key, out var v) ? v : val;
    }
    
    public static bool IsNullOrWhiteSpace(this string self)
    {
        return self == null || self.All(char.IsWhiteSpace);
    }
}
