/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofArSettings.UI;

namespace TofArSamples.BasicStream
{
    public class BasicStreamViewSettings : SettingsBase
    {
        [Header("Default value")]

        [SerializeField]
        bool depth = true;

        [SerializeField]
        bool confidence = true;

        [SerializeField]
        bool color = true;

        [SerializeField]
        bool texts = true;

        ImageViewAdjuster imgViewAdjuster;
        Text[] txts;

        protected override void MakeUI()
        {
            // Create UI content
            settings.AddItem("Depth View", depth, ShowDepth);
            settings.AddItem("Confidence View", confidence, ShowConfidence);
            settings.AddItem("Color View", color, ShowColor);
            settings.AddItem("Texts", texts, ShowTexts);

            // Get display area UI
            imgViewAdjuster = GetComponent<ImageViewAdjuster>();
            txts = imgViewAdjuster.ImageViewArea.GetComponentsInChildren<Text>();

            // Toggle display
            ShowDepth(depth);

            base.MakeUI();
        }

        /// <summary>
        /// Display depth ON/OFF
        /// </summary>
        /// <param name="onOff">ON/OFF</param>
        void ShowDepth(bool onOff)
        {
            imgViewAdjuster.ShowView("Depth", onOff);
        }

        /// <summary>
        /// Display Confidence ON/OFF
        /// </summary>
        /// <param name="onOff">ON/OFF</param>
        void ShowConfidence(bool onOff)
        {
            imgViewAdjuster.ShowView("Confidence", onOff);
        }

        /// <summary>
        /// Display Color ON/OFF
        /// </summary>
        /// <param name="onOff">ON/OFF</param>
        void ShowColor(bool onOff)
        {
            imgViewAdjuster.ShowView("Color", onOff);
        }

        /// <summary>
        /// Display text on the bottom of View ON/OFF
        /// </summary>
        /// <param name="onOff">ON/OFF</param>
        void ShowTexts(bool onOff)
        {
            for (int i = 0; i < txts.Length; i++)
            {
                txts[i].enabled = onOff;
            }
        }

    }
}
