using System;
using System.Collections.Generic;
using Architect.Workshop.Items;
using Architect.Workshop.Types;
using GlobalEnums;
using UnityEngine;

namespace Architect.Workshop.Config;

public static class ConfigGroup
{
    public static readonly List<ConfigType> SpriteItem =
    [
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<SpriteItem>("Icon URL", "png_url", (item, value) =>
            {
                item.IconUrl = value.GetValue();
            })
        ),
        ConfigurationManager.RegisterConfigType(
            new BoolConfigType<SpriteItem>("Anti Aliasing", "png_antialias", (item, value) =>
            {
                item.Point = !value.GetValue();
            }).WithDefaultValue(true)
        ),
        ConfigurationManager.RegisterConfigType(
            new FloatConfigType<SpriteItem>("Pixels Per Unit", "png_ppu", (item, value) =>
            {
                item.Ppu = value.GetValue();
            }).WithDefaultValue(100)
        )
    ];
    
    public static readonly List<ConfigType> Scene = [
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<CustomScene>("Group ID", "scene_group", (item, value) =>
            {
                item.Group = value.GetValue();
            }).WithDefaultValue("None")
        ),
        ConfigurationManager.RegisterConfigType(
            new IntConfigType<CustomScene>("Scene Width", "scene_tilemap_width", (item, value) =>
            {
                item.TilemapWidth = value.GetValue();
            }).WithDefaultValue(500)
        ),
        ConfigurationManager.RegisterConfigType(
            new IntConfigType<CustomScene>("Scene Height", "scene_tilemap_height", (item, value) =>
            {
                item.TilemapHeight = value.GetValue();
            }).WithDefaultValue(500)
        ),
        ConfigurationManager.RegisterConfigType(
            new ChoiceConfigType<CustomScene>("Environment Type", "scene_enviro_type", (item, value) =>
            {
                item.Environment = value.GetValue();
            }).WithOptions("Dust", "Grass", "Bone", "Spa", "Metal", "None", "Wet").WithDefaultValue(0)
        )
    ];
    
    public static readonly List<ConfigType> SceneGroup = [
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<SceneGroup>("Area Name", "scene_group_name", (item, value) =>
            {
                item.GroupName = value.GetValue();
            })
        )
    ];
    
    public static readonly List<ConfigType> Charm = [
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<CustomCharm>("Charm Name", "charm_name", (item, value) =>
            {
                item.CharmName = value.GetValue();
            }).WithDefaultValue("Sample Name")
        ),
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<CustomCharm>("Charm Desc", "charm_desc", (item, value) =>
            {
                item.CharmDesc = value.GetValue();
            }).WithDefaultValue("Sample Desc")
        ),
        ConfigurationManager.RegisterConfigType(
            new IntConfigType<CustomCharm>("Charm Cost", "charm_cost", (item, value) =>
            {
                item.Cost = value.GetValue();
            }).WithDefaultValue(1)
        )
    ];
    
    public static readonly List<ConfigType> Item = [
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<CustomItem>("Item Name", "item_name", (item, value) =>
            {
                item.Name = value.GetValue();
            }).WithDefaultValue("Sample Name")
        ),
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<CustomItem>("Item Desc", "item_desc", (item, value) =>
            {
                item.Desc = value.GetValue();
            }).WithDefaultValue("Sample Desc")
        ),
        ConfigurationManager.RegisterConfigType(
            new BoolConfigType<CustomItem>("Show Count", "item_count", (item, value) =>
            {
                item.HasCount = value.GetValue();
            }).WithDefaultValue(false)
        )
    ];
    
    public static readonly List<ConfigType> SceneGroupIcon = [
        ConfigurationManager.RegisterConfigType(
            new StringConfigType<SpriteItem>("Save Icon URL", "scene_group_url", (item, value) =>
            {
                item.IconUrl = value.GetValue();
            })
        ),
        ConfigurationManager.RegisterConfigType(
            new BoolConfigType<SpriteItem>("Anti Aliasing", "scene_group_antialias", (item, value) =>
            {
                item.Point = !value.GetValue();
            }).WithDefaultValue(true)
        )
    ];
}