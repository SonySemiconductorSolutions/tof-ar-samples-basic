/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TofArSamples;
using UI = TofArSettings.UI;
using UnityEngine;
using UnityEngine.Events;

namespace TofArSamples.Segmentation
{
    public class SegmentationViewSettings : ImageViewSettings
    {
        protected UI.ItemDropdown itemColorDropdown;
        UI.ItemSlider itemTransparent;

        MaterialColorController materialColorController;
        MaterialTransparentController materialTransparentController;

        protected override void Awake()
        {
            base.Awake();

            materialColorController = FindObjectOfType<MaterialColorController>();
            materialTransparentController = FindObjectOfType<MaterialTransparentController>();
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();

            var list = new List<UnityAction>(uiOrder);

            list.Add(MakeUISetColor);
            list.Add(MakeUITransparent);

            // Set UI order
            uiOrder = list.ToArray();
        }

        private void MakeUISetColor()
        {
            itemColorDropdown = settings.AddItem("Segmentation Color", materialColorController.ColorNames,
                materialColorController.Index, SetSegmentationColor, 0, 0, 240);
            materialColorController.OnChange += (index) =>
            {
                itemColorDropdown.Index = index;
            };

            SetSegmentationColor(0);
        }

        private void SetSegmentationColor(int index)
        {
            materialColorController.Index = index;
        }

        /// <summary>
        /// Make Scale UI
        /// </summary>
        void MakeUITransparent()
        {
            float transparent = materialTransparentController.Transparent;

            itemTransparent = settings.AddItem("Transparent", MaterialTransparentController.transparentMin,
                MaterialTransparentController.transparentMax, MaterialTransparentController.transparentStep,
                transparent, ChangeTransparent);
        }

        /// <summary>
        /// Change transparency
        /// </summary>
        /// <param name="value"></param>
        public void ChangeTransparent(float value)
        {
            materialTransparentController.Transparent = value;
        }
    }
}
