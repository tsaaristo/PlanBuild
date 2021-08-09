﻿using Jotunn.Managers;
using UnityEngine;

namespace PlanBuild.Blueprints.Tools
{
    internal class DeleteObjectsComponent : ToolComponentBase
    {
        public override void UpdatePlacement(Player self)
        {
            if (!self.m_placementMarkerInstance)
            {
                return;
            }

            EnableSelectionProjector(self);

            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0f)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    UpdateCameraOffset(scrollWheel);
                }
                else
                {
                    UpdateSelectionRadius(scrollWheel);
                }
                UndoRotation(self, scrollWheel);
            }
        }

        public override bool PlacePiece(Player self, Piece piece)
        {
            if (!(BlueprintConfig.allowFlattenConfig.Value || SynchronizationManager.Instance.PlayerIsAdmin))
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_terrain_disabled");
                return false;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                TerrainTools.RemoveAll(self.m_placementGhost.transform, SelectionRadius);
            }
            else
            {
                TerrainTools.RemoveVegetation(self.m_placementGhost.transform, SelectionRadius);
            }
            return false;
        }
    }
}
