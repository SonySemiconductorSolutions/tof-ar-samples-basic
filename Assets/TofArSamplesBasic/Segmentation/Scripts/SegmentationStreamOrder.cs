/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using TofArSettings.Segmentation;
using UnityEngine;

namespace TofArSamples.Segmentation
{
    public class SegmentationStreamOrder : MonoBehaviour
    {
        private SegmentationManagerController segmentationManagerController;

        protected void Awake()
        {
            segmentationManagerController = FindObjectOfType<SegmentationManagerController>();
        }

        protected void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;
        }

        protected void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;
        }

        void OnColorStreamStarted(object sender, UnityEngine.Texture2D tex)
        {
            var colorFormat = TofArColorManager.Instance.GetProperty<FormatConvertProperty>();
            if (colorFormat.format != ColorFormat.BGR)
            {
                segmentationManagerController.OnColorStreamStarted(sender, tex);
            }
        }

        void OnColorStreamStopped(object sender)
        {
            segmentationManagerController.OnColorStreamStopped(sender);
        }
    }
}
