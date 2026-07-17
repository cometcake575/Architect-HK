using System;
using System.Collections.Generic;
using System.Linq;
using Architect.Config;
using Architect.Config.Types;
using Architect.Objects.Placeable;
using Newtonsoft.Json;
using UnityEngine;

namespace Architect.Updater;

public class DcmObjectType(string aName, 
    float scaleMultiplier, 
    Dictionary<string, string> configMappings,
    Vector3 offset,
    float rotOffset)
{
    public PlaceableObject GetPlaceableObject()
    {
        return PlaceableObject.RegisteredObjects[aName];
    }

    public float GetScaleMultiplier() => scaleMultiplier;
    
    public Vector3 GetOffset() => offset;
    
    public float GetRotOffset() => rotOffset;
    
    public ConfigValue[] TranslateConfigValues(Dictionary<string, string> oldValues)
    {
        var values = GetPlaceableObject()
            .ConfigGroup
            .Select(c => c.GetDefaultValue())
            .Where(c => c != null).ToList();
        foreach (var (key, val) in oldValues)
        {
            if (!configMappings.TryGetValue(key, out var k)) continue;
            var deserialized = ConfigurationManager
                .DeserializeConfigValue(k, val);
            if (deserialized != null)
            {
                values.RemoveAll(d => d.GetTypeId() == k);
                values.Add(deserialized);
            }
        }

        return values.ToArray();
    }

    public class DcmTypeConverter : JsonConverter<DcmObjectType>
    {
        public override void WriteJson(JsonWriter writer, DcmObjectType value, JsonSerializer serializer) { }

        public override DcmObjectType ReadJson(JsonReader reader, Type objectType, DcmObjectType existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = string.Empty;
            var scaleMultiplier = 1f;
            var rotOffset = 0f;
            var xOffset = 0f;
            var yOffset = 0f;
            var zOffset = 0f;
            Dictionary<string, string> map = [];

            while (reader.Read() && reader.Value is string s)
            {
                switch (s)
                {
                    case "name":
                        name = reader.ReadAsString();
                        break;
                    case "scale":
                        scaleMultiplier = (float)reader.ReadAsDouble()!;
                        break;
                    case "x_offset":
                        xOffset = (float)reader.ReadAsDouble()!;
                        break;
                    case "rot_offset":
                        rotOffset = (float)reader.ReadAsDouble()!;
                        break;
                    case "y_offset":
                        yOffset = (float)reader.ReadAsDouble()!;
                        break;
                    case "z_offset":
                        zOffset = (float)reader.ReadAsDouble()!;
                        break;
                    case "config":
                        reader.Read();
                        map = serializer.Deserialize<Dictionary<string, string>>(reader);
                        break;
                }
            }

            return new DcmObjectType(name, scaleMultiplier, map, new Vector3(xOffset, yOffset, zOffset), rotOffset);
        }
    }
}