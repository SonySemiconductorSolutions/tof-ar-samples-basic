/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.HumanPointCloud
{
    public class HumanPointCloudViewSettings : PointCloudViewSettings
    {
        private HumanPointCloudController humanPointCloudController;
        private UI.ItemToggle itemHumanSegmentationToggle, itemColorDisplayToggle;
        private UI.ItemSlider itemThresholdSlider;

        protected override void Awake()
        {
            base.Awake();
            humanPointCloudController = FindObjectOfType<HumanPointCloudController>();
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();

            var list = new List<UnityAction>(uiOrder);
            list.Add(MakeUIHumanPointCloud);

            // Set UI order
            uiOrder = list.ToArray();
            controllers.Add(humanPointCloudController);
        }

        /// <summary>
        /// Make the UI for human point cloud control
        /// </summary>
        private void MakeUIHumanPointCloud()
        {
            itemColorDisplayToggle = settings.AddItem("Color Display", humanPointCloudController.ColorDisplay, ChangeColorDisplay);
            humanPointCloudController.OnColorDisplayChange += (onOff) =>
             {
                 itemColorDisplayToggle.OnOff = onOff;
                 itemColorDropdown.Interactable = !onOff;
             };
            itemColorDropdown.Interactable = !humanPointCloudController.ColorDisplay;

            itemHumanSegmentationToggle = settings.AddItem("Segment Pointcloud", humanPointCloudController.SegmentHuman, ChangeHumanSegmentation);
            humanPointCloudController.OnSegmentHumanChange += (onOff) =>
            {
                itemHumanSegmentationToggle.OnOff = onOff;
            };

            itemThresholdSlider = settings.AddItem("Segmentation\nThreshold", HumanPointCloudController.thresholdMin, HumanPointCloudController.thresholdMax, HumanPointCloudController.thresholdStep, humanPointCloudController.Threshold, ChangeThreshold);
            humanPointCloudController.OnThresholdChange += (val) =>
            {
                itemThresholdSlider.Value = val;
            };
        }

        /// <summary>
        /// Switch color display on or off
        /// </summary>
        /// <param name="onOff"></param>
        private void ChangeColorDisplay(bool onOff)
        {
            humanPointCloudController.ColorDisplay = onOff;
        }

        /// <summary>
        /// Switch human segmentation on or off
        /// </summary>
        /// <param name="onOff"></param>
        private void ChangeHumanSegmentation(bool onOff)
        {
            humanPointCloudController.SegmentHuman = onOff;
        }

        /// <summary>
        /// change the threshold value
        /// </summary>
        /// <param name="val"></param>
        private void ChangeThreshold(float val)
        {
            humanPointCloudController.Threshold = val;
        }
    }
}
