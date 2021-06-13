﻿using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using PlanBuild.Plans;
using UnityEngine;

namespace PlanBuild.Blueprints
{
    internal class BlueprintRunePrefab
    {
        public const string PieceTableName = "_BlueprintPieceTable";
        public const string CategoryTools = "Tools";
        public const string CategoryBlueprints = "Blueprints";

        public const string BlueprintRuneName = "BlueprintRune";

        public const string BlueprintSnapPointName = "piece_blueprint_snappoint"; 
        public const string BlueprintCenterPointName = "piece_blueprint_centerpoint";
        public const string BlueprintStandingBlueprintRune = "piece_standing_blueprint_rune";
        public const string MakeBlueprintName = "make_blueprint";
        public const string UndoBlueprintName = "undo_blueprint";
        public const string DeletePlansName = "delete_plans";

        public static string BlueprintRuneItemName;
        public GameObject runeprefab;

        public BlueprintRunePrefab(AssetBundle assetBundle)
        {
            // Rune piece table
            CustomPieceTable table = new CustomPieceTable(PieceTableName, new PieceTableConfig
            {
                UseCategories = false,
                UseCustomCategories = true,
                CustomCategories = new string[]
                {
                    "Tools", "Blueprints"
                }
            });
            PieceManager.Instance.AddPieceTable(table);

            // Rune item
            GameObject runeprefab = assetBundle.LoadAsset<GameObject>(BlueprintRuneName);
            CustomItem item = new CustomItem(runeprefab, false, new ItemConfig
            {
                Amount = 1,
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig {Item = "Stone", Amount = 1}
                }
            }); 
            ItemManager.Instance.AddItem(item); 
            BlueprintRuneItemName = item.ItemDrop.m_itemData.m_shared.m_name;
            item.ItemDrop.m_itemData.m_shared.m_buildPieces = PieceManager.Instance.GetPieceTable(PlanPiecePrefab.PlanHammerPieceTableName);

            // Tool pieces
            CustomPiece piece;
            GameObject prefab;
            foreach (string pieceName in new string[]
            {
                MakeBlueprintName, BlueprintSnapPointName, BlueprintCenterPointName,
                UndoBlueprintName, DeletePlansName
            })
            {
                prefab = assetBundle.LoadAsset<GameObject>(pieceName);
                piece = new CustomPiece(prefab, new PieceConfig
                {
                    PieceTable = PieceTableName,
                    Category = CategoryTools
                });
                PieceManager.Instance.AddPiece(piece);
            }

            // Blueprint stub
            GameObject placebp_prefab = assetBundle.LoadAsset<GameObject>(Blueprint.BlueprintPrefabName);
            PrefabManager.Instance.AddPrefab(placebp_prefab);
        }
    }
}