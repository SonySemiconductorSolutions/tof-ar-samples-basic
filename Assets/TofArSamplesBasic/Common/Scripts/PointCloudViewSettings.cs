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

namespace TofArSamples
{
    public class PointCloudViewSettings : UI.SettingsBase
    {
        protected UI.ItemButton itemResetCamera, itemCamera90;
        protected UI.ItemDropdown itemColorDropdown;
        CameraController cameraController;
        MaterialColorController materialColorController;

        [SerializeField]
        private bool setColor = true;

        protected virtual void Awake()
        {
            cameraController = FindObjectOfType<CameraController>();
            uiOrder = new UnityAction[0];
            if (setColor)
            {
                materialColorController = FindObjectOfType<MaterialColorController>();
            }
        }

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        /// <summary>
        /// Prepare for UI creation
        /// </summary>
        protected virtual void PrepareUI()
        {
            var list = new List<UnityAction>(uiOrder);
            list.Add(MakeUIResetCamera);
            if (setColor)
            {
                list.Add(MakeUISetColor);
                controllers.Add(materialColorController);
            }

            // Set UI order
            uiOrder = list.ToArray();
        }

        private void MakeUIResetCamera()
        {
            itemResetCamera = settings.AddItem("Reset Camera", cameraController.ResetPosition);
        }

        private void MakeUISetColor()
        {
            itemColorDropdown = settings.AddItem("Point Color", materialColorController.ColorNames, materialColorController.Index, SetPointcloudColor);
            materialColorController.OnChange += (index) =>
            {
                itemColorDropdown.Index = index;
            };

            SetPointcloudColor(0);
        }

        private void SetPointcloudColor(int index)
        {
            materialColorController.Index = index;
        }
    }
}
