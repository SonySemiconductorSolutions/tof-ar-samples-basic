/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TofArSettings;
using TofArSettings.Segmentation;

namespace TofArSamples.HumanPointCloud
{
    public class HumanPointCloudController : ControllerBase
    {
        private HumanPointCloud humanPointCloud;
        private HumanSegmentationController segmentationController;

        protected void Awake()
        {
            humanPointCloud = FindObjectOfType<HumanPointCloud>();
            segmentationController = FindObjectOfType<HumanSegmentationController>();
        }

        protected override void Start()
        {
            base.Start();
            //make sure we start with human segmentation on
            segmentationController.HumanSegmentationEnabled = true;
        }

        public const float thresholdMax = 255;
        public const float thresholdMin = 0;
        public const float thresholdStep = 5;

        public event ChangeValueEvent OnThresholdChange;

        public float Threshold
        {
            get => humanPointCloud.Threshold;
            set
            {
                if (value != Threshold)
                {
                    humanPointCloud.Threshold = (byte)value;
                    OnThresholdChange.Invoke(Threshold);
                }
            }
        }

        public event ChangeToggleEvent OnSegmentHumanChange;

        public bool SegmentHuman
        {
            get => humanPointCloud.SegmentHuman;
            set
            {
                if (value != SegmentHuman)
                {
                    humanPointCloud.SegmentHuman = value;
                    OnSegmentHumanChange.Invoke(SegmentHuman);
                }
            }
        }

        public bool ColorDisplay
        {
            get => humanPointCloud.ColorDisplay;
            set
            {
                if (value != ColorDisplay)
                {
                    humanPointCloud.ColorDisplay = value;
                    OnColorDisplayChange.Invoke(ColorDisplay);
                }
            }
        }

        public event ChangeToggleEvent OnColorDisplayChange;
    }
}
