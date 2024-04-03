/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Threading;
using UnityEngine;
using TofAr.V0.Color;
using TofAr.V0.Segmentation.Sky;
using TofAr.V0.Segmentation.Human;
using TofArSettings.Segmentation;
using System.Collections;
using TofAr.V0.Segmentation;

namespace TofArSamples.Segmentation
{
    /// <summary>
    /// SegmentationController
    /// </summary>
    public class SegmentationController : MonoBehaviour
    {
        private SkySegmentationDetector skyDetector;
        private HumanSegmentationDetector humanDetector;

        private SegmentationManagerController segmentationManagerController;
        private HumanSegmentationController humanSegmentationController;
        private SkySegmentationController skySegmentationController;

        /// <summary>
        /// AutoStartState(Human)
        /// </summary>
        public bool AutoStartHuman;

        /// <summary>
        /// AutoStartState(Sky)
        /// </summary>
        public bool AutoStartSky;

        private void Awake()
        {
            humanDetector = FindObjectOfType<HumanSegmentationDetector>();
            skyDetector = FindObjectOfType<SkySegmentationDetector>();
            segmentationManagerController = FindObjectOfType<SegmentationManagerController>();
            humanSegmentationController = FindObjectOfType<HumanSegmentationController>();
            skySegmentationController = FindObjectOfType<SkySegmentationController>();
        }

        private SynchronizationContext context;

        public Material segmentationMaskMaterial = null;


        private void OnEnable()
        {
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArSegmentationManager.OnStreamStarted += OnSegmentationStreamStarted;
            TofArSegmentationManager.OnStreamStopped += OnSegmentationStreamStopped;
            humanSegmentationController.OnHumanChange += OnHumanChange;
            skySegmentationController.OnSkyChange += OnSkyChange;
            humanSegmentationController.OnNotHumanChange += OnNotHumanChange;
            skySegmentationController.OnNotSkyChange += OnNotSkyChange;
        }


        private void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArSegmentationManager.OnStreamStarted -= OnSegmentationStreamStarted;
            TofArSegmentationManager.OnStreamStopped -= OnSegmentationStreamStopped;
            humanSegmentationController.OnHumanChange -= OnHumanChange;
            skySegmentationController.OnSkyChange -= OnSkyChange;
            humanSegmentationController.OnNotHumanChange -= OnNotHumanChange;
            skySegmentationController.OnNotSkyChange -= OnNotSkyChange;
            this.segmentationMaskMaterial.SetFloat("_useHuman", 0);
            this.segmentationMaskMaterial.SetFloat("_useSky", 0);
            this.segmentationMaskMaterial.SetFloat("_invertHuman", 0);
            this.segmentationMaskMaterial.SetFloat("_invertSky", 0);
            this.segmentationMaskMaterial.SetFloat("_ScaleV", 1f);
            this.segmentationMaskMaterial.SetFloat("_ScaleU", 1f);
            this.segmentationMaskMaterial.SetFloat("_OffsetV", 0f);
            this.segmentationMaskMaterial.SetFloat("_OffsetU", 0f);
        }

        private void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            var resolutionProperty = TofAr.V0.Color.TofArColorManager.Instance.GetProperty<TofAr.V0.Color.ResolutionProperty>();

            int width = resolutionProperty.width;
            int height = resolutionProperty.height;

            int segWidth = humanDetector.MaskTexture.width;
            int segHeight = humanDetector.MaskTexture.height;

            float segRatio = (float)segWidth / (float)segHeight;
            float imgRatio = (float)width / (float)height;

            float segHeightScale = 1f;
            float segWidthScale = 1f;
            float vOffset = 0;
            float uOffset = 0;

            // add offset and scale to match segmentation ratio
            if (segRatio > imgRatio)
            {
                int colorHeightAdjusted = (int)(width / segRatio);
                int vOffset0 = ((height - colorHeightAdjusted) / 2);
                vOffset = -(float)vOffset0 / (float)colorHeightAdjusted;

                segHeightScale = (float)height / (float)colorHeightAdjusted;
            }
            else if (segRatio < imgRatio)
            {
                int colorWidthAdjusted = (int)(height * segRatio);
                int uOffset0 = ((width - colorWidthAdjusted) / 2);
                uOffset = -(float)uOffset0 / (float)colorWidthAdjusted;

                segWidthScale = (float)width / (float)colorWidthAdjusted;
            }

            this.segmentationMaskMaterial.SetFloat("_ScaleV", segHeightScale);
            this.segmentationMaskMaterial.SetFloat("_ScaleU", segWidthScale);
            this.segmentationMaskMaterial.SetFloat("_OffsetV", vOffset);
            this.segmentationMaskMaterial.SetFloat("_OffsetU", uOffset);
        }

        private void OnSegmentationStreamStarted(object sender)
        {
            this.segmentationMaskMaterial.SetTexture("_MaskTexHuman", this.humanDetector.MaskTexture);
            this.segmentationMaskMaterial.SetTexture("_MaskTexSky", this.skyDetector.MaskTexture);
        }
        private void OnSegmentationStreamStopped(object sender)
        {
            this.segmentationMaskMaterial.SetTexture("_MaskTexHuman", null);
            this.segmentationMaskMaterial.SetTexture("_MaskTexSky", null);
        }

        private IEnumerator Start()
        {
            this.context = SynchronizationContext.Current;
            yield return new WaitForEndOfFrame();

            if (AutoStartHuman || AutoStartSky)
            {
                humanSegmentationController.HumanSegmentationEnabled = AutoStartHuman;
                skySegmentationController.SkySegmentationEnabled = AutoStartSky;
            }
        }

        private void OnHumanChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_useHuman", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_useHuman", 0);
                }
            }, null);
        }

        private void OnNotHumanChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_invertHuman", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_invertHuman", 0);
                }
            }, null);
        }
        private void OnSkyChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_useSky", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_useSky", 0);
                }
            }, null);
        }

        private void OnNotSkyChange(bool val)
        {
            context.Post((s) =>
            {
                if (val)
                {
                    this.segmentationMaskMaterial.SetFloat("_invertSky", 1);
                }
                else
                {
                    this.segmentationMaskMaterial.SetFloat("_invertSky", 0);
                }
            }, null);
        }
    }
}
