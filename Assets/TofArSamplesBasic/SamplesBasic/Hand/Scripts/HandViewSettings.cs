/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.Hand
{
    public class HandViewSettings : ImageViewSettings
    {
        //[Header("Use functionality")]

        /// <summary>
        /// Toggle save function
        /// </summary>
        /*[SerializeField]
        bool save = true;*/

        [Header("Use Hand Model")]


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

        UI.ItemToggle itemSkeleton;//, itemSaveButton;
        UI.ItemDropdown itemRealHand;
        UI.ItemSlider itemOffsetX, itemOffsetY, itemOffsetZ, itemScale;//, itemSaveTime;

        protected override void Awake()
        {
            base.Awake();

            skeletonCtrl = GetComponent<SkeletonHandController>();
            skeletonCtrl.enabled = skeleton;
            realCtrl = GetComponent<RealHandController>();
            realCtrl.enabled = real;
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();

            var list = new List<UnityAction>(uiOrder);

            if (skeleton)
            {
                list.Add(MakeUISkeletonHand);
                controllers.Add(skeletonCtrl);
            }

            if (real)
            {
                list.Add(MakeUIRealHand);
                controllers.Add(realCtrl);
            }

            list.Add(MakeUIOffset);
            list.Add(MakeUIScale);

            // Set UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make Skeleton Hand UI
        /// </summary>
        void MakeUISkeletonHand()
        {
            itemSkeleton = settings.AddItem("Show Bones", skeletonCtrl.IsShow,
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
            itemRealHand = settings.AddItem("Real Hand", realCtrl.HandNames,
                realCtrl.Index, ChangeRealHand);

            realCtrl.OnChangeIndex += (index) =>
            {
                itemRealHand.Index = index;
            };
        }

        /// <summary>
        /// Toggle Real Hand UI display
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
            settings.AddItem("Offset", FontStyle.Bold);

            var offset = GetOffset();
            itemOffsetX = settings.AddItem(" X", HandModelController.OffsetMin,
                HandModelController.OffsetMax, HandModelController.OffsetStep,
                offset.x, ChangeOffsetX, 0, 0, lineAlpha);
            itemOffsetX.IsNotifyImmediately = true;
            itemOffsetY = settings.AddItem(" Y", HandModelController.OffsetMin,
                HandModelController.OffsetMax, HandModelController.OffsetStep,
                offset.y, ChangeOffsetY, 0, 0, lineAlpha);
            itemOffsetY.IsNotifyImmediately = true;
            itemOffsetZ = settings.AddItem(" Z", HandModelController.OffsetMin,
                HandModelController.OffsetMax, HandModelController.OffsetStep,
                offset.z, ChangeOffsetZ, 0, 0, lineAlpha);
            itemOffsetZ.IsNotifyImmediately = true;

            settings.AddItem("Reset Offset", ResetOffset, 0, 0, lineAlpha);

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
        /// Reset offset
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

            itemScale = settings.AddItem("Scale", HandModelController.ScaleMin,
                HandModelController.ScaleMax, HandModelController.ScaleStep,
                scale, ChangeScale);
            itemScale.IsNotifyImmediately = true;

            settings.AddItem("Reset Scale", ResetScale, 0, 0, lineAlpha);

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
        /// Get current offset value from HandModelController
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
