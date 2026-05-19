using System;
using Architect.Placements;
using Newtonsoft.Json;
using UnityEngine;

namespace Architect.Utils;

public static class Converters
{
    public static readonly ColorConverter ColorConverter = new();
    public static readonly Vector2Converter Vector2Converter = new();
    public static readonly Vector3Converter Vector3Converter = new();
    
    private static readonly LevelData.LevelDataConverter Ldc = new();
    private static readonly ObjectPlacement.ObjectPlacementConverter Opc = new();

    public static readonly JsonConverter[] All = 
        [ColorConverter, Vector2Converter, Vector3Converter, Ldc, Opc];
}

public class ColorConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, (value.r, value.g, value.b, value.a));
    }

    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var read = serializer.Deserialize<(float, float, float, float)>(reader);
        return new Color(read.Item1, read.Item2, read.Item3, read.Item4);
    }
}

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, (value.x, value.y));
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var read = serializer.Deserialize<(float, float)>(reader);
        return new Vector2(read.Item1, read.Item2);
    }
}

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, (value.x, value.y, value.z));
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var read = serializer.Deserialize<(float, float, float)>(reader);
        return new Vector3(read.Item1, read.Item2, read.Item3);
    }
}
