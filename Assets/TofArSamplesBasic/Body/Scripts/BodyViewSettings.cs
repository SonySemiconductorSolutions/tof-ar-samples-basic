/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.Body
{
    public class BodyViewSettings : ImageViewSettings
    {
        BodyModelController modelCtrl;

        UI.ItemToggle itemSkeleton;
        UI.ItemDropdown itemRealBody;
        UI.ItemSlider itemOffsetX, itemOffsetY, itemOffsetZ, itemScale;

        protected override void Awake()
        {
            base.Awake();
            modelCtrl = GetComponent<BodyModelController>();
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();

            var list = new List<UnityAction>(uiOrder);
            list.Add(MakeUIModel);
            controllers.Add(modelCtrl);

            list.Add(MakeUIOffset);
            //list.Add(MakeUIScale); // Remove for now

            // Set the UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Create Real Body UI
        /// </summary>
        void MakeUIModel()
        {
            itemRealBody = settings.AddItem("Model", modelCtrl.ModelNames,
                modelCtrl.Index, ChangeRealBody);

            modelCtrl.OnChangeIndex += (index) =>
            {
                itemRealBody.Index = index;
            };
        }

        /// <summary>
        /// Toggle Real Body display
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeRealBody(int index)
        {
            modelCtrl.Index = index;
        }

        /// <summary>
        /// Create Offset UI
        /// </summary>
        void MakeUIOffset()
        {
            var offset = GetOffset();
            itemOffsetX = settings.AddItem("Offset X", BodyModelController.OffsetMin,
                BodyModelController.OffsetMax, BodyModelController.OffsetStep,
                offset.x, ChangeOffsetX);
            itemOffsetX.IsNotifyImmediately = true;
            itemOffsetY = settings.AddItem("Offset Y", BodyModelController.OffsetMin,
                BodyModelController.OffsetMax, BodyModelController.OffsetStep,
                offset.y, ChangeOffsetY);
            itemOffsetY.IsNotifyImmediately = true;
            itemOffsetZ = settings.AddItem("Offset Z", BodyModelController.OffsetMin,
                BodyModelController.OffsetMax, BodyModelController.OffsetStep,
                offset.z, ChangeOffsetZ);
            itemOffsetZ.IsNotifyImmediately = true;

            settings.AddItem("Reset Offset", ResetOffset);
            modelCtrl.OnChangeOffset += OnChangeOffset;
        }

        /// <summary>
        /// Change offset on the x-axis
        /// </summary>
        /// <param name="val">X-axis offset value</param>
        void ChangeOffsetX(float val)
        {
            var newOffset = GetOffset();
            newOffset.x = val;
            ChangeOffset(newOffset);
        }

        /// <summary>
        /// Change offset on the y-axis
        /// </summary>
        /// <param name="val">Y-axis offset value</param>
        void ChangeOffsetY(float val)
        {
            var newOffset = GetOffset();
            newOffset.y = val;
            ChangeOffset(newOffset);
        }

        /// <summary>
        /// Change offset on the z-axis
        /// </summary>
        /// <param name="val">Z-axis offset value</param>
        void ChangeOffsetZ(float val)
        {
            var newOffset = GetOffset();
            newOffset.z = val;
            ChangeOffset(newOffset);
        }

        void ResetOffset()
        {
            ChangeOffset(BodyModelController.OffsetDefault);
        }

        void ChangeOffset(Vector3 newOffset)
        {
            modelCtrl.Offset = newOffset;
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
        /// Create Scale UI
        /// </summary>
        void MakeUIScale()
        {
            float scale = 1;
            scale = modelCtrl.Scale;

            itemScale = settings.AddItem("Scale", BodyModelController.ScaleMin,
                BodyModelController.ScaleMax, BodyModelController.ScaleStep,
                scale, ChangeScale);
            itemScale.IsNotifyImmediately = true;

            settings.AddItem("Reset Scale", ResetScale);

            modelCtrl.OnChangeScale += OnChangeScale;
        }

        void ChangeScale(float newScale)
        {
            modelCtrl.Scale = newScale;
        }

        void ResetScale()
        {
            ChangeScale(BodyModelController.ScaleDefault);
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
        /// Get current offset value from BodyModelController
        /// </summary>
        /// <returns>Offset</returns>
        Vector3 GetOffset()
        {
            var offset = BodyModelController.OffsetDefault;
            offset = modelCtrl.Offset;

            return offset;
        }
    }
}
