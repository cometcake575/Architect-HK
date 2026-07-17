using System;
using System.Collections.Generic;
using Architect.Placements;
using Newtonsoft.Json;
using UnityEngine;

namespace Architect.Updater;

public class DcmObject(DcmObjectType dType, float rot, float scale, Dictionary<string, string> configs, Vector2 pos)
{
    public readonly Vector3 Pos = pos;
    
    public string GetConfig(string config) => configs[config];
    
    public void SetConfig(string config, string val) => configs[config] = val;
    
    public ObjectPlacement GetPlacement()
    {
        var type = dType.GetPlaceableObject();
        
        return new ObjectPlacement(
            type,
            new Vector3(Pos.x, Pos.y) + dType.GetOffset(),
            Guid.NewGuid().ToString()[..8],
            false,
            rot + dType.GetRotOffset(),
            scale * dType.GetScaleMultiplier(),
            false,
            0,
            [],
            [],
            dType.TranslateConfigValues(configs)
        );
    }
    
    public class DcmObjectConverter : JsonConverter<List<DcmObject>>
    {
        private static Dictionary<string, DcmObjectType> _objectTypes;

        public DcmObjectConverter()
        {
            var dtc = new DcmObjectType.DcmTypeConverter();

            _objectTypes =
                JsonConvert.DeserializeObject<Dictionary<string, DcmObjectType>>(
                    ResourceUtils.LoadTextResource("Updater.types.json"), dtc);
        }

        public static DcmObjectType GetObjectType(string name)
        {
            if (!_objectTypes.TryGetValue(name, out var val))
            {
                ArchitectPlugin.Instance.Log($"No object found for DcM object {name}");
                return null;
            } 
            return val;
        }
        
        public override void WriteJson(JsonWriter writer, List<DcmObject> value, JsonSerializer serializer) { }

        public override List<DcmObject> ReadJson(JsonReader reader, Type objectType, List<DcmObject> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            List<DcmObject> objects = [];

            while (reader.Read())
            {
                if (reader.Value as string != "$type") continue;
                reader.Read();
                if ((reader.Value as string).StartsWith("System.")
                    || (reader.Value as string).StartsWith("DecorationMaster.ItemSettings")) continue;
                
                var pos = Vector2.zero;
                var rot = 0f;
                var scale = 1f;
                Dictionary<string, string> configs = [];
                var dName = string.Empty;

                reader.Read();
                while (reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.Value as string == "position" || reader.Value as string == "Center")
                    {
                        reader.Read();
                        while (reader.Value as string != "x") reader.Read();
                        var x = reader.ReadAsDouble()!;
                        reader.Read();
                        var y = reader.ReadAsDouble()!;
                        pos = new Vector2((float)x, (float)y);

                        reader.Read();
                    }
                    else
                    {
                        var rv = reader.Value as string;
                        switch (rv)
                        {
                            case null:
                                continue;
                            case "pname":
                                dName = reader.ReadAsString();
                                break;
                            case "angle":
                                rot = (float)reader.ReadAsDouble()!;
                                break;
                            case "size":
                                scale = (float)reader.ReadAsDouble()!;
                                break;
                            default:
                                configs[(reader.Value as string)!] = reader.ReadAsString();
                                break;
                        }
                    }
                    reader.Read();
                }

                var ot = GetObjectType(dName);
                if (ot == null) continue;
                objects.Add(new DcmObject(ot, rot, scale, configs, pos));
            }

            return objects;
        }
    }
}