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

namespace TofArSamples.Tof
{
    public class SaveRegionViewSettings : UI.SettingsBase
    {
        UI.ItemButton itemSave;
        UI.ItemToggle itemWholeImageToggle;
        //UI.ItemInputField itemRegionSizeInputField, itemMeasureFramesInputField;
        UI.ItemSlider itemRegionSizeSlider, itemMeasureFramesSlider;
        SaveFromRegion saveFromRegion;

        protected virtual void Awake()
        {
            saveFromRegion = FindObjectOfType<SaveFromRegion>();
            uiOrder = new UnityAction[0];
        }

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        protected virtual void PrepareUI()
        {
            var list = new List<UnityAction>(uiOrder);
            list.Add(MakeUISave);

            // Set UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make Save UI
        /// </summary>
        private void MakeUISave()
        {
            itemSave = settings.AddItem("Save Depth", saveFromRegion.SavePoint);
            itemWholeImageToggle = settings.AddItem("WholeImage", saveFromRegion.IsFullScreenRegion, SetWholeImage);
            itemRegionSizeSlider = settings.AddItem("Region Size", 1, 100, 1, saveFromRegion.RegionWidth, SetRegionSizeValue);
            itemMeasureFramesSlider = settings.AddItem("Measure Frames", 1, 100, 1, saveFromRegion.SaveFrames, SetSaveFramesValue);
            SetWholeImage(saveFromRegion.IsFullScreenRegion);
        }

        /// <summary>
        /// Toggle the use of whole image
        /// </summary>
        /// <param name="onOff">On/Off</param>
        private void SetWholeImage(bool onOff)
        {
            saveFromRegion.IsFullScreenRegion = onOff;
            //set visibilities
            itemRegionSizeSlider.Interactable = !onOff;
            itemMeasureFramesSlider.Interactable = !onOff;
        }

        private void SetRegionSizeValue(float value)
        {
            saveFromRegion.RegionWidth = (int)value;
        }

        private void SetSaveFramesValue(float value)
        {
            saveFromRegion.SaveFrames = (int)value;
        }
    }
}
