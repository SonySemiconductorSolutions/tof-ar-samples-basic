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

namespace TofArSamples.Face
{
    public class FaceViewSettings : ImageViewSettings
    {
        FaceModelController modelCtrl;

        //UI.ItemToggle itemSkeleton;
        UI.ItemToggle itemFace, itemGaze;
        UI.ItemSlider itemOffsetX, itemOffsetY, itemOffsetZ, itemScale;

        protected override void Awake()
        {
            base.Awake();
            modelCtrl = GetComponent<FaceModelController>();
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();

            var list = new List<UnityAction>(uiOrder);
            //list.Add(MakeUIModel);
            controllers.Add(modelCtrl);

            list.Add(MakeUIFace);
            list.Add(MakeUIOffset);
            list.Add(MakeUIScale);

            // Set UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make Face Model UI
        /// </summary>
        void MakeUIFace()
        {
            itemFace = settings.AddItem("Show FaceMesh", modelCtrl.IsShow,
                ChangeFace);

            modelCtrl.OnChangeShow += (onOff) =>
            {
                itemFace.OnOff = onOff;
            };

            itemGaze = settings.AddItem("Show Gaze", modelCtrl.IsShowGaze, ChangeGaze);

            modelCtrl.OnChangeShowGaze += (onOff) =>
            {
                itemGaze.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle Face Model display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeFace(bool onOff)
        {
            modelCtrl.IsShow = onOff;
        }

        /// <summary>
        /// Toggle Gaze
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeGaze(bool onOff)
        {
            modelCtrl.IsShowGaze = onOff;
        }

        /// <summary>
        /// Make Offset UI
        /// </summary>
        void MakeUIOffset()
        {
            settings.AddItem("Offset", FontStyle.Bold);

            var offset = GetOffset();
            itemOffsetX = settings.AddItem(" X", FaceModelController.OffsetMin,
                FaceModelController.OffsetMax, FaceModelController.OffsetStep,
                offset.x, ChangeOffsetX, 0, 0, lineAlpha);
            itemOffsetX.IsNotifyImmediately = true;
            itemOffsetY = settings.AddItem(" Y", FaceModelController.OffsetMin,
                FaceModelController.OffsetMax, FaceModelController.OffsetStep,
                offset.y, ChangeOffsetY, 0, 0, lineAlpha);
            itemOffsetY.IsNotifyImmediately = true;
            itemOffsetZ = settings.AddItem(" Z", FaceModelController.OffsetMin,
                FaceModelController.OffsetMax, FaceModelController.OffsetStep,
                offset.z, ChangeOffsetZ, 0, 0, lineAlpha);
            itemOffsetZ.IsNotifyImmediately = true;

            settings.AddItem("Reset Offset", ResetOffset, 0, 0, lineAlpha);
            modelCtrl.OnChangeOffset += OnChangeOffset;
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
            ChangeOffset(FaceModelController.OffsetDefault);
        }

        /// <summary>
        /// Apply offset
        /// </summary>
        /// <param name="newOffset">Offset</param>
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
        /// Make Scale UI
        /// </summary>
        void MakeUIScale()
        {
            float scale = 1;
            scale = modelCtrl.Scale;

            itemScale = settings.AddItem("Scale", FaceModelController.ScaleMin,
                FaceModelController.ScaleMax, FaceModelController.ScaleStep,
                scale, ChangeScale);
            itemScale.IsNotifyImmediately = true;

            settings.AddItem("Reset Scale", ResetScale, 0, 0, lineAlpha);

            modelCtrl.OnChangeScale += OnChangeScale;
        }

        /// <summary>
        /// Change scale
        /// </summary>
        /// <param name="newScale">Scale</param>
        void ChangeScale(float newScale)
        {
            modelCtrl.Scale = newScale;
        }

        /// <summary>
        /// Reset scale
        /// </summary>
        void ResetScale()
        {
            ChangeScale(FaceModelController.ScaleDefault);
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
        /// Get current offset values from BodyModelController
        /// </summary>
        /// <returns>Offset</returns>
        Vector3 GetOffset()
        {
            var offset = FaceModelController.OffsetDefault;
            offset = modelCtrl.Offset;

            return offset;
        }
    }
}
