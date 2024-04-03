/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofArSamples.Hand;
using TofArSamples.Mesh;
using TofArSettings.UI;
using UnityEngine;
using UI = TofArSettings.UI;

namespace TofArSamples.Hand
{
    public class HandMeshViewSettings : MonoBehaviour
    {
        MeshViewSettings meshViewSettings;

        HandOcclusionController handOcclusionController;

        /// <summary>
        /// Toggle hand only occlusion
        /// </summary>
        [SerializeField]
        bool handOcclusion = true;

        /// <summary>
        /// Toggle hand
        /// </summary>
        [SerializeField]
        bool handSettings = false;

        /// <summary>
        /// Toggle Skeleton Hand
        /// </summary>
        [SerializeField]
        bool skeleton = true;

        /// <summary>
        /// Toggle Real Hand
        /// </summary>
        [SerializeField]
        bool real = true;

        SkeletonHandController skeletonCtrl;
        RealHandController realCtrl;

        UI.ItemToggle itemSkeleton, itemSaveButton, itemHandOcclusion;
        UI.ItemDropdown itemRealHand;
        UI.ItemSlider itemOffsetX, itemOffsetY, itemOffsetZ, itemScale, itemSaveTime, itemClippingDistance;

        void Awake()
        {
            meshViewSettings = GetComponent<MeshViewSettings>();
            if (meshViewSettings != null)
            {
                meshViewSettings.AddListenerAddControllersEvent(SetController);

                handOcclusionController = GetComponent<HandOcclusionController>();
                handOcclusionController.enabled = handOcclusion;

                skeletonCtrl = GetComponent<SkeletonHandController>();
                skeletonCtrl.enabled = skeleton;
                realCtrl = GetComponent<RealHandController>();
                realCtrl.enabled = real;
            }
        }

        /// <summary>
        /// Setting Controller
        /// </summary>
        /// <param name="settingsPanel">SettingsPanel</param>
        public void SetController(SettingsPanel settingsPanel)
        {
            if (handOcclusion)
            {
                meshViewSettings.SetUIOrderList(MakeUIHandOcclusion);
                meshViewSettings.SetController(handOcclusionController);
            }

            if (handSettings)
            {
                if (skeleton)
                {
                    meshViewSettings.SetUIOrderList(MakeUISkeletonHand);
                    meshViewSettings.SetController(skeletonCtrl);
                }

                if (real)
                {
                    meshViewSettings.SetUIOrderList(MakeUIRealHand);
                    meshViewSettings.SetController(realCtrl);
                }

                meshViewSettings.SetUIOrderList(MakeUIOffset);
                meshViewSettings.SetUIOrderList(MakeUIScale);
            }
        }

        /// <summary>
        /// Make Occlusion Object UI
        /// </summary>
        void MakeUIHandOcclusion()
        {
            itemHandOcclusion = meshViewSettings.GetSettings().AddItem("HandOcclusion", handOcclusionController.IsHandOnlyOcclusion,
                ChangeHandOcclusion);

            handOcclusionController.OnChangeHandOcclusion += (onOff) =>
            {
                itemHandOcclusion.OnOff = onOff;
                itemClippingDistance.Interactable = onOff;
            };

            itemClippingDistance = meshViewSettings.GetSettings().AddItem("Clipping Distance", HandOcclusionController.minClippingDistance, HandOcclusionController.maxClippingDistance, HandOcclusionController.clippingDistanceStep, handOcclusionController.ClippingDistance, ChangeHandOcclusionDistance, -3);

            handOcclusionController.OnChangeClippingDistance += (val) =>
            {
                itemClippingDistance.Value = val;
            };

            itemClippingDistance.Interactable = handOcclusionController.IsHandOnlyOcclusion;
        }

        /// <summary>
        /// Toggle Hand occlusion
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeHandOcclusion(bool onOff)
        {
            handOcclusionController.IsHandOnlyOcclusion = onOff;
        }

        /// <summary>
        /// Toggle Cube display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeHandOcclusionDistance(float val)
        {
            handOcclusionController.ClippingDistance = val;
        }

        /// <summary>
        /// Make Skeleton Hand UI
        /// </summary>
        void MakeUISkeletonHand()
        {
            itemSkeleton = meshViewSettings.GetSettings().AddItem("Show Bones", skeletonCtrl.IsShow,
                ChangeSkeletonHand);

            skeletonCtrl.OnChangeShow += (onOff) =>
            {
                itemSkeleton.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle Skeleton Hand display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeSkeletonHand(bool onOff)
        {
            skeletonCtrl.IsShow = onOff;
        }

        /// <summary>
        /// Make Real Hand UI
        /// </summary>
        void MakeUIRealHand()
        {
            itemRealHand = meshViewSettings.GetSettings().AddItem("Real Hand", realCtrl.HandNames,
                realCtrl.Index, ChangeRealHand);

            realCtrl.OnChangeIndex += (index) =>
            {
                itemRealHand.Index = index;
            };
        }

        /// <summary>
        /// Toggle Real Hand display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeRealHand(int index)
        {
            realCtrl.Index = index;
        }

        /// <summary>
        /// Make Offset UI
        /// </summary>
        void MakeUIOffset()
        {
            var offset = GetOffset();
            itemOffsetX = meshViewSettings.GetSettings().AddItem("Offset X", HandModelController.OffsetMin,
                HandModelController.OffsetMax, HandModelController.OffsetStep,
                offset.x, ChangeOffsetX);
            itemOffsetX.IsNotifyImmediately = true;
            itemOffsetY = meshViewSettings.GetSettings().AddItem("Offset Y", HandModelController.OffsetMin,
                HandModelController.OffsetMax, HandModelController.OffsetStep,
                offset.y, ChangeOffsetY);
            itemOffsetY.IsNotifyImmediately = true;
            itemOffsetZ = meshViewSettings.GetSettings().AddItem("Offset Z", HandModelController.OffsetMin,
                HandModelController.OffsetMax, HandModelController.OffsetStep,
                offset.z, ChangeOffsetZ);
            itemOffsetZ.IsNotifyImmediately = true;

            meshViewSettings.GetSettings().AddItem("Reset Offset", ResetOffset);

            if (skeleton)
            {
                skeletonCtrl.OnChangeOffset += OnChangeOffset;
            }

            if (real)
            {
                realCtrl.OnChangeOffset += OnChangeOffset;
            }
        }

        /// <summary>
        /// Change offset on the x-axis
        /// </summary>
        /// <param name="val">x-axis offset value</param>
        void ChangeOffsetX(float val)
        {
            var newOffset = GetOffset();
            newOffset.x = val;
            ChangeOffset(newOffset);
        }

        /// <summary>
        /// Change offset on the y-axis
        /// </summary>
        /// <param name="val">y-axis offset value</param>
        void ChangeOffsetY(float val)
        {
            var newOffset = GetOffset();
            newOffset.y = val;
            ChangeOffset(newOffset);
        }

        /// <summary>
        /// Change offset on the z-axis
        /// </summary>
        /// <param name="val">z-axis offset value</param>
        void ChangeOffsetZ(float val)
        {
            var newOffset = GetOffset();
            newOffset.z = val;
            ChangeOffset(newOffset);
        }

        /// <summary>
        /// Reset offset values
        /// </summary>
        void ResetOffset()
        {
            ChangeOffset(HandModelController.OffsetDefault);
        }

        /// <summary>
        /// Apply offset
        /// </summary>
        /// <param name="newOffset">Offset</param>
        void ChangeOffset(Vector3 newOffset)
        {
            if (skeleton)
            {
                skeletonCtrl.Offset = newOffset;
            }

            if (real)
            {
                realCtrl.Offset = newOffset;
            }
        }

        /// <summary>
        /// Event that is called when offset is changed
        /// </summary>
        /// <param name="offset">Offset</param>
        void OnChangeOffset(Vector3 offset)
        {
            itemOffsetX.Value = offset.x;
            itemOffsetY.Value = offset.y;
            itemOffsetZ.Value = offset.z;
        }

        /// <summary>
        /// Make Scale UI
        /// </summary>
        void MakeUIScale()
        {
            float scale = 1;
            if (skeleton)
            {
                scale = skeletonCtrl.Scale;
            }
            else if (real)
            {
                scale = realCtrl.Scale;
            }

            itemScale = meshViewSettings.GetSettings().AddItem("Scale", HandModelController.ScaleMin,
                HandModelController.ScaleMax, HandModelController.ScaleStep,
                scale, ChangeScale);
            itemScale.IsNotifyImmediately = true;

            meshViewSettings.GetSettings().AddItem("Reset Scale", ResetScale);

            if (skeleton)
            {
                skeletonCtrl.OnChangeScale += OnChangeScale;
            }
            if (real)
            {
                realCtrl.OnChangeScale += OnChangeScale;
            }
        }

        /// <summary>
        /// Change scale
        /// </summary>
        /// <param name="newScale">Scale</param>
        void ChangeScale(float newScale)
        {
            if (skeleton)
            {
                skeletonCtrl.Scale = newScale;
            }

            if (real)
            {
                realCtrl.Scale = newScale;
            }
        }

        /// <summary>
        /// Reset scale
        /// </summary>
        void ResetScale()
        {
            ChangeScale(HandModelController.ScaleDefault);
        }

        /// <summary>
        /// Event that is called when scale is changed
        /// </summary>
        /// <param name="scale">Scale</param>
        void OnChangeScale(float scale)
        {
            itemScale.Value = scale;
        }

        /// <summary>
        /// Get current offset values from HandModelController
        /// </summary>
        /// <returns>Offset</returns>
        Vector3 GetOffset()
        {
            var offset = HandModelController.OffsetDefault;
            if (skeleton)
            {
                offset = skeletonCtrl.Offset;
            }
            else if (real)
            {
                offset = realCtrl.Offset;
            }

            return offset;
        }
    }
}
