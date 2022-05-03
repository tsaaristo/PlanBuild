﻿using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Managers;
using PlanBuild.Blueprints;
using PlanBuild.Plans;
using PlanBuild.Utils;
using System;
using UnityEngine;

namespace PlanBuild
{
    internal static class Config
    {
        public enum DefaultBuildMode
        {
            Planned, Direct
        }
        
        public static bool DirectBuildDefault => DefaultBuildModeConfig.Value == DefaultBuildMode.Direct;
        
        private const string ServerSection = "Server Settings";
        public static ConfigEntry<bool> AllowDirectBuildConfig;
        public static ConfigEntry<bool> AllowTerrainmodConfig;
        public static ConfigEntry<bool> AllowServerBlueprints;
        public static ConfigEntry<bool> AllowMarketHotkey;
        public static ConfigEntry<string> PlanBlacklistConfig;

        private const string RuneSection = "Blueprints";
        public static ConfigEntry<DefaultBuildMode> DefaultBuildModeConfig;
        public static ConfigEntry<bool> InvertCameraOffsetScrollConfig;
        public static ConfigEntry<bool> InvertPlacementOffsetScrollConfig;
        public static ConfigEntry<bool> InvertSelectionScrollConfig;
        public static ConfigEntry<float> RayDistanceConfig;
        public static ConfigEntry<float> CameraOffsetIncrementConfig;
        public static ConfigEntry<float> PlacementOffsetIncrementConfig;
        public static ConfigEntry<float> SelectionIncrementConfig;
        public static ConfigEntry<float> SelectionConnectedMarginConfig;
        public static ConfigEntry<bool> ShowGridConfig;
        public static ConfigEntry<bool> TooltipEnabledConfig;
        public static ConfigEntry<Color> TooltipBackgroundConfig;
        
        private const string DirectorySection = "Directories";
        public static ConfigEntry<string> BlueprintSearchDirectoryConfig;
        public static ConfigEntry<string> BlueprintSaveDirectoryConfig;

        private const string KeybindSection = "Keybindings";
        public static ConfigEntry<KeyCode> MarketHotkeyConfig;
        public static ButtonConfig MarketHotkeyButton;
        public static ConfigEntry<KeyCode> CameraModifierConfig;
        public static ButtonConfig CameraModifierButton;
        public static ConfigEntry<KeyCode> RadiusModifierConfig;
        public static ButtonConfig RadiusModifierButton;
        public static ConfigEntry<KeyCode> ToggleConfig;
        public static ButtonConfig ToggleButton;
        public static ConfigEntry<KeyCode> DeleteModifierConfig;
        public static ButtonConfig DeleteModifierButton;
        
        private const string PlansSection = "Plans";
        public static ConfigEntry<bool> ShowAllPieces;
        public static ConfigEntry<float> RadiusConfig;
        public static ConfigEntry<bool> ShowParticleEffects;
        
        private const string VisualSection = "Visual";
        public static ConfigEntry<bool> ConfigTransparentGhostPlacement;
        public static ConfigEntry<Color> UnsupportedColorConfig;
        public static ConfigEntry<Color> SupportedPlanColorConfig;
        public static ConfigEntry<float> TransparencyConfig;
        public static ConfigEntry<Color> GlowColorConfig;

        internal static void Init()
        {
            int order = 0;
            
            // Server Section

            AllowDirectBuildConfig = PlanBuildPlugin.Instance.Config.Bind(
                ServerSection, "Allow direct build", false,
                new ConfigDescription($"Allow placement of blueprints without materials on this server.{Environment.NewLine}Admins are always allowed to use it.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));

            AllowTerrainmodConfig = PlanBuildPlugin.Instance.Config.Bind(
                ServerSection, "Allow terrain tools", false,
                new ConfigDescription($"Allow usage of the terrain modification tools on this server.{Environment.NewLine}Admins are always allowed to use them.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));

            AllowServerBlueprints = PlanBuildPlugin.Instance.Config.Bind(
                ServerSection, "Allow serverside blueprints", false,
                new ConfigDescription($"Allow sharing of blueprints on this server.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));

            AllowMarketHotkey = PlanBuildPlugin.Instance.Config.Bind(
                ServerSection, "Allow clients to use the GUI toggle key", true,
                new ConfigDescription($"Allow clients to use the Hotkey to access server blueprints.{Environment.NewLine}Admins are always allowed to use it.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));
            
            PlanBlacklistConfig = PlanBuildPlugin.Instance.Config.Bind(
                ServerSection, "Excluded plan prefabs", "AltarPrefab,FloatingIslandMO",
                new ConfigDescription($"Comma separated list of prefab names to exclude from the planned piece table on this server{Environment.NewLine}Admins are always allowed to build them.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order, Browsable = false }));

            // Rune Section

            DefaultBuildModeConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Default build mode", DefaultBuildMode.Planned,
                new ConfigDescription("Default build mode when placing blueprints.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            DefaultBuildModeConfig.SettingChanged += (sender, args) =>
            {
                foreach (var bp in BlueprintManager.LocalBlueprints.Values)
                {
                    bp.CreateKeyHint();
                }
            };

            RayDistanceConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Place distance", 50f,
                new ConfigDescription("Place distance while using the Blueprint Rune.",
                    new AcceptableValueRange<float>(8f, 80f),
                    new ConfigurationManagerAttributes { Order = ++order }));

            CameraOffsetIncrementConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Camera offset increment", 2f,
                new ConfigDescription("Camera height change when holding the camera modifier key and scrolling while in Blueprint mode.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            InvertCameraOffsetScrollConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Invert camera offset scroll", false,
                new ConfigDescription("Invert the direction of camera offset scrolling.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            PlacementOffsetIncrementConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Placement offset increment", 0.1f,
                new ConfigDescription("Placement height change when holding the modifier key and scrolling while in Blueprint mode.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            InvertPlacementOffsetScrollConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Invert placement height change scroll", false,
                new ConfigDescription("Invert the direction of placement offset scrolling.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            SelectionIncrementConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Selection increment", 1f,
                new ConfigDescription("Selection radius increment when scrolling while in Blueprint mode.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            InvertSelectionScrollConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Invert selection scroll", false,
                new ConfigDescription("Invert the direction of selection scrolling.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            SelectionConnectedMarginConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Selection connected check margin", 0.01f,
                new ConfigDescription("Distance of the shell used to check for connectedness.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            ShowGridConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Show the transform bound grid", false,
                new ConfigDescription("Shows a grid around the blueprints' bounds to visualize the blueprints' edges.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            TooltipEnabledConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Tooltip enabled", true,
                new ConfigDescription("Show a tooltip with a bigger thumbnail for blueprint pieces.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            TooltipBackgroundConfig = PlanBuildPlugin.Instance.Config.Bind(
                RuneSection, "Tooltip Color", new Color(0.13f, 0.13f, 0.13f, 0.65f),
                new ConfigDescription("Set the background color for the tooltip on blueprint pieces.", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            // Directory Section

            BlueprintSearchDirectoryConfig = PlanBuildPlugin.Instance.Config.Bind(
                DirectorySection, "Search directory", ".",
                new ConfigDescription("Base directory to scan (recursively) for blueprints and vbuild files, relative paths are relative to the valheim.exe location", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            BlueprintSaveDirectoryConfig = PlanBuildPlugin.Instance.Config.Bind(
                DirectorySection, "Save directory", "BepInEx/config/PlanBuild/blueprints",
                new ConfigDescription("Directory to save blueprint files, relative paths are relative to the valheim.exe location", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            // Keybind Section

            MarketHotkeyConfig = PlanBuildPlugin.Instance.Config.Bind(
                KeybindSection, "Blueprint Marketplace GUI toggle key", KeyCode.End,
                new ConfigDescription("Hotkey to show blueprint marketplace GUI", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            CameraModifierConfig = PlanBuildPlugin.Instance.Config.Bind(
                KeybindSection, "CameraModifier", KeyCode.LeftShift,
                new ConfigDescription("Modifier key to modify camera behavior on various tools", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            RadiusModifierConfig = PlanBuildPlugin.Instance.Config.Bind(
                KeybindSection, "RadiusModifier", KeyCode.LeftControl,
                new ConfigDescription("Modifier key to use radius based selection on various tools", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            DeleteModifierConfig = PlanBuildPlugin.Instance.Config.Bind(
                KeybindSection, "DeleteModifier", KeyCode.LeftAlt,
                new ConfigDescription("Modifier key for removal operations on various tools", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            ToggleConfig = PlanBuildPlugin.Instance.Config.Bind(
                KeybindSection, "Toggle", KeyCode.Q,
                new ConfigDescription("Key to switch between modes on various tools", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            // Plans Section

            ShowAllPieces = PlanBuildPlugin.Instance.Config.Bind(
                PlansSection, "Plan unknown pieces", false,
                new ConfigDescription("Show all plans, even for pieces you don't know yet", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));
            RadiusConfig = PlanBuildPlugin.Instance.Config.Bind(
                PlansSection, "Plan totem build radius", 30f,
                new ConfigDescription("Build radius of the plan totem", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));
            ShowParticleEffects = PlanBuildPlugin.Instance.Config.Bind(
                PlansSection, "Plan totem particle effects", true,
                new ConfigDescription("Show particle effects when building pieces with the plan totem", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true, Order = ++order }));

            ShowAllPieces.SettingChanged += (obj, attr) => PlanManager.Instance.UpdateKnownRecipes();
            //RadiusConfig.SettingChanged += (_, _) => PlanManager.Instance.UpdateAllPlanTotems();  // that doesnt change the radius...
            //PlanBlacklistConfig.SettingChanged += (sender, args) => PlanBlacklist.Reload();
            
            // Visual Section

            ConfigTransparentGhostPlacement = PlanBuildPlugin.Instance.Config.Bind(
                VisualSection, "Transparent Ghost Placement", true,
                new ConfigDescription("Apply plan shader to ghost placement (currently placing piece)", null,
                    new ConfigurationManagerAttributes { Order = ++order }));
            UnsupportedColorConfig = PlanBuildPlugin.Instance.Config.Bind(
                VisualSection, "Unsupported color", new Color(1f, 1f, 1f, 0.1f),
                new ConfigDescription("Color of unsupported plan pieces", null,
                    new ConfigurationManagerAttributes { Order = ++order }));
            SupportedPlanColorConfig = PlanBuildPlugin.Instance.Config.Bind(
                VisualSection, "Supported color", new Color(1f, 1f, 1f, 0.5f),
                new ConfigDescription("Color of supported plan pieces", null,
                    new ConfigurationManagerAttributes { Order = ++order }));
            TransparencyConfig = PlanBuildPlugin.Instance.Config.Bind(
                VisualSection, "Transparency", 0.30f,
                new ConfigDescription("Additional transparency", new AcceptableValueRange<float>(0f, 1f),
                    new ConfigurationManagerAttributes { Order = ++order }));
            GlowColorConfig = PlanBuildPlugin.Instance.Config.Bind(
                VisualSection, "Plan totem glow color", Color.cyan,
                new ConfigDescription("Color of the glowing lines on the Plan totem", null,
                    new ConfigurationManagerAttributes { Order = ++order }));

            ConfigTransparentGhostPlacement.SettingChanged += UpdateGhostPlanPieceTextures;
            UnsupportedColorConfig.SettingChanged += UpdateAllPlanPieceTextures;
            SupportedPlanColorConfig.SettingChanged += UpdateAllPlanPieceTextures;
            TransparencyConfig.SettingChanged += UpdateAllPlanPieceTextures;
            GlowColorConfig.SettingChanged += UpdateAllPlanTotems;
            
            // Create Buttons
            CreateCustomButtons();
            
            // Create KeyHints if and when PixelFix is created
            GUIManager.OnCustomGUIAvailable += CreateCustomKeyHints;
        }
        
        private static void CreateCustomButtons()
        {
            // Global

            MarketHotkeyButton = new ButtonConfig
            {
                Name = "GUIToggle",
                Config = MarketHotkeyConfig,
                ActiveInCustomGUI = true
            };
            InputManager.Instance.AddButton(PlanBuildPlugin.PluginGUID, MarketHotkeyButton);

            // Shared

            CameraModifierButton = new ButtonConfig
            {
                Name = nameof(CameraModifierButton),
                Config = CameraModifierConfig
            };
            InputManager.Instance.AddButton(PlanBuildPlugin.PluginGUID, CameraModifierButton);

            RadiusModifierButton = new ButtonConfig
            {
                Name = nameof(RadiusModifierButton),
                Config = RadiusModifierConfig
            };
            InputManager.Instance.AddButton(PlanBuildPlugin.PluginGUID, RadiusModifierButton);

            DeleteModifierButton = new ButtonConfig
            {
                Name = nameof(DeleteModifierButton),
                Config = DeleteModifierConfig
            };
            InputManager.Instance.AddButton(PlanBuildPlugin.PluginGUID, DeleteModifierButton);

            ToggleButton = new ButtonConfig
            {
                Name = nameof(ToggleButton),
                Config = ToggleConfig
            };
            InputManager.Instance.AddButton(PlanBuildPlugin.PluginGUID, ToggleButton);
        }

        /// <summary>
        ///     Create custom KeyHints for the static Blueprint Rune pieces
        /// </summary>
        private static void CreateCustomKeyHints()
        {
            // Mode Switch

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "BuildMenu", HintToken = "$hud_buildmenu" }
                }
            });
            
            // Add selection

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceSelectAddName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_blueprint_select_add" },
                    new ButtonConfig { Name = ToggleButton.Name, Config = ToggleConfig, HintToken = "$hud_blueprint_select_add_switch" },
                    new ButtonConfig { Name = DeleteModifierButton.Name, Config = DeleteModifierConfig, HintToken = "$hud_blueprint_select_add_connected" },
                    new ButtonConfig { Name = RadiusModifierButton.Name, Config = RadiusModifierConfig, HintToken = "$hud_blueprint_select_add_radius" },
                    new ButtonConfig { Name = CameraModifierButton.Name, Config = CameraModifierConfig, HintToken = "$hud_bpcamera" }
                }
            });

            // Remove selection

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceSelectRemoveName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_blueprint_select_remove" },
                    new ButtonConfig { Name = ToggleButton.Name, Config = ToggleConfig, HintToken = "$hud_blueprint_select_remove_switch" },
                    new ButtonConfig { Name = DeleteModifierButton.Name, Config = DeleteModifierConfig, HintToken = "$hud_blueprint_select_remove_connected" },
                    new ButtonConfig { Name = RadiusModifierButton.Name, Config = RadiusModifierConfig, HintToken = "$hud_blueprint_select_remove_radius" },
                    new ButtonConfig { Name = $"{DeleteModifierButton.Key} + {RadiusModifierButton.Key}", HintToken = "$hud_blueprint_select_remove_clear" },
                    new ButtonConfig { Name = CameraModifierButton.Name, Config = CameraModifierConfig, HintToken = "$hud_bpcamera" }
                }
            });

            // Save selection

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceSelectSaveName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_blueprint_select_save" }
                }
            });

            // Snap Point

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceSnapPointName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_bpsnappoint" },
                    new ButtonConfig { Name = "Scroll", Axis = "Mouse ScrollWheel", HintToken = "$hud_bprotate" },
                }
            });

            // Center point

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceCenterPointName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_bpcenterpoint" },
                    new ButtonConfig { Name = "Scroll", Axis = "Mouse ScrollWheel", HintToken = "$hud_bprotate" },
                }
            });

            // Remove

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceDeletePlansName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_bpdelete" },
                    new ButtonConfig { Name = RadiusModifierButton.Name, Config = RadiusModifierConfig, HintToken = "$hud_bpdelete_radius" },
                    new ButtonConfig { Name = DeleteModifierButton.Name, Config = DeleteModifierConfig, HintToken = "$hud_bpdelete_all" },
                    new ButtonConfig { Name = CameraModifierButton.Name, Config = CameraModifierConfig, HintToken = "$hud_bpcamera" }
                }
            });

            // Terrain

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceTerrainName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_bpterrain_flatten" },
                    new ButtonConfig { Name = ToggleButton.Name, Config = ToggleConfig, HintToken = "$hud_bpterrain_marker" },
                    new ButtonConfig { Name = DeleteModifierButton.Name, Config = DeleteModifierConfig, HintToken = "$hud_bpterrain_delete" },
                    new ButtonConfig { Name = CameraModifierButton.Name, Config = CameraModifierConfig, HintToken = "$hud_bpcamera" },
                    new ButtonConfig { Name = "Scroll", Axis = "Mouse ScrollWheel", HintToken = "$hud_bpterrainradius" }
                }
            });

            // Delete

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PieceDeleteObjectsName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_bpobjects_deleteveg" },
                    new ButtonConfig { Name = RadiusModifierButton.Name, Config = RadiusModifierConfig, HintToken = "$hud_bpobjects_deletepieces" },
                    new ButtonConfig { Name = DeleteModifierButton.Name, Config = DeleteModifierConfig, HintToken = "$hud_bpobjects_deleteall" },
                    new ButtonConfig { Name = CameraModifierButton.Name, Config = CameraModifierConfig, HintToken = "$hud_bpcamera" },
                    new ButtonConfig { Name = "Scroll", Axis = "Mouse ScrollWheel", HintToken = "$hud_bpradius" }
                }
            });

            // Paint

            KeyHintManager.Instance.AddKeyHint(new KeyHintConfig
            {
                Item = BlueprintAssets.BlueprintRuneName,
                Piece = BlueprintAssets.PiecePaintName,
                ButtonConfigs = new[]
                {
                    new ButtonConfig { Name = "Attack", HintToken = "$hud_bppaint_reset" },
                    new ButtonConfig { Name = "Ctrl", HintToken = "$hud_bppaint_dirt" },
                    new ButtonConfig { Name = "Alt", HintToken = "$hud_bppaint_paved" },
                    new ButtonConfig { Name = CameraModifierButton.Name, HintToken = "$hud_bpcamera" },
                    new ButtonConfig { Name = "Scroll", Axis = "Mouse ScrollWheel", HintToken = "$hud_bpradius" }
                }
            });

            GUIManager.OnCustomGUIAvailable -= CreateCustomKeyHints;
        }

        private static void UpdateGhostPlanPieceTextures(object sender, EventArgs e)
        {
            PlanManager.Instance.UpdateAllPlanPieceTextures();
        }

        private static void UpdateAllPlanPieceTextures(object sender, EventArgs e)
        {
            ShaderHelper.ClearCache();
            PlanManager.Instance.UpdateAllPlanPieceTextures();
        }

        private static void UpdateAllPlanTotems(object sender, EventArgs e)
        {
            PlanManager.Instance.UpdateAllPlanTotems();
        }
    }
}