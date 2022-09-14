/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Color;
using TofAr.V0.Tof;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples.BasicStream
{
    public class BaseTransform : MonoBehaviour
    {
        private RectTransform rectTransform;
        private RectTransform childRectTransform;

        private float aspectRatio = 1.0f;

        private const float defaultRatio = 4f / 3f;

        private bool isColor = false;
        private bool wasRotated = false;

        private bool configChanged = false;

        // Start is called before the first frame update
        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            var rawImage = transform.GetComponentInChildren<TofAr.V0.Tof.TextureMapperRawImage>();
            if (rawImage != null)
            {
                childRectTransform = rawImage.gameObject.GetComponent<RectTransform>();
            }
            else
            {
                var rawImageColor = transform.GetComponentInChildren<TofAr.V0.Color.TextureMapperRawImage>();
                if (rawImageColor != null)
                {
                    childRectTransform = rawImageColor.gameObject.GetComponent<RectTransform>();
                    isColor = true;
                }
            }
        }

        void OnEnable()
        {
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationChanged;

            TofArColorManager.OnStreamStarted += OnColorStarted;
            TofArTofManager.OnStreamStarted += OnTofStarted;

            UpdateRotation();
        }

        private void OnDisable()
        {
            TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationChanged;
            TofArColorManager.OnStreamStarted -= OnColorStarted;
            TofArTofManager.OnStreamStarted -= OnTofStarted;
        }

        private void OnTofStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            if (isColor)
            {
                return;
            }
            var aspectFitter = GetComponentInChildren<AspectRatioFitter>();


            var instance = TofArTofManager.Instance;
            var currentConfiguration = instance.GetProperty<CameraConfigurationProperty>();
            if (currentConfiguration != null)
            {
                aspectFitter.aspectRatio = (float)currentConfiguration.width / currentConfiguration.height;
            }

            configChanged = true;
        }

        private void OnColorStarted(object sender, Texture2D colorTexture)
        {
            if (!isColor)
            {
                return;
            }
            var aspectFitter = GetComponentInChildren<AspectRatioFitter>(true);


            var instance = TofArColorManager.Instance;
            var currentConfiguration = instance.GetProperty<ResolutionProperty>();
            if (currentConfiguration != null)
            {
                aspectFitter.aspectRatio = (float)currentConfiguration.width / currentConfiguration.height;
            }

            configChanged = true;
        }

        private void UpdateRotation()
        {
            this.wasRotated = true;
        }

        private void OnScreenOrientationChanged(ScreenOrientation previousDeviceOrientation, ScreenOrientation newDeviceOrientation)
        {
            UpdateRotation();
        }

        private void UpdateTransformColor(int imageRotation, float ratio)
        {
            if (imageRotation == 270 || imageRotation == 90)
            {
                if (TofArColorManager.Instance.IsStreamActive || TofArColorManager.Instance.IsPlaying)
                {
                    var currentConfiguration = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
                    if (currentConfiguration != null)
                    {
                        var scaleFactor = Mathf.Min((float)currentConfiguration.width / currentConfiguration.height, ratio);
                        if (currentConfiguration.width > currentConfiguration.height)
                        {
                            scaleFactor = Mathf.Max((float)currentConfiguration.height / currentConfiguration.width, scaleFactor);
                        }
                        else
                        {
                            scaleFactor = Mathf.Min((float)currentConfiguration.height / currentConfiguration.width, scaleFactor);
                        }

                        childRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                    }

                    if (this.rectTransform.rect.width != this.childRectTransform.rect.width)
                    {
                        var adjustScale = Mathf.Max(
                            this.rectTransform.rect.width / this.childRectTransform.rect.width,
                            this.rectTransform.rect.height / this.childRectTransform.rect.height);
                        this.childRectTransform.localScale *= adjustScale;
                    }
                }
                else
                {
                    if (defaultRatio < ratio)
                    {
                        childRectTransform.localScale = new Vector3(defaultRatio, defaultRatio, defaultRatio);
                    }
                    else
                    {
                        childRectTransform.localScale = new Vector3(ratio, ratio, ratio);
                    }
                }

            }
            else
            {
                childRectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        private void UpdateTransformDepth(int imageRotation, float ratio)
        {
            if (imageRotation == 90 || imageRotation == 270)
            {
                if (TofArTofManager.Instance.IsStreamActive || TofArTofManager.Instance.IsPlaying)
                {
                    var currentConfiguration = TofArTofManager.Instance.GetProperty<CameraConfigurationProperty>();
                    if (currentConfiguration != null)
                    {
                        float scaleFactor = Mathf.Min((float)currentConfiguration.width / currentConfiguration.height, ratio);
                        if (currentConfiguration.width > currentConfiguration.height)
                        {
                            scaleFactor = Mathf.Max((float)currentConfiguration.height / currentConfiguration.width, scaleFactor);
                        }
                        else
                        {
                            scaleFactor = Mathf.Min((float)currentConfiguration.height / currentConfiguration.width, scaleFactor);
                        }

                        childRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                    }

                    if (this.rectTransform.rect.width != this.childRectTransform.rect.width)
                    {
                        var adjustScale = Mathf.Max(
                            this.rectTransform.rect.width / this.childRectTransform.rect.width,
                            this.rectTransform.rect.height / this.childRectTransform.rect.height);
                        this.childRectTransform.localScale *= adjustScale;
                    }
                }
                else
                {
                    if (defaultRatio < ratio)
                    {
                        childRectTransform.localScale = new Vector3(defaultRatio, defaultRatio, defaultRatio);
                    }
                    else
                    {
                        childRectTransform.localScale = new Vector3(ratio, ratio, ratio);
                    }
                    
                }

                
            }
            else
            {
                childRectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        private void Update()
        {
            if (rectTransform.rect.width > 0)
            {
                float ratio = rectTransform.rect.height / rectTransform.rect.width;

                if (childRectTransform != null && (ratio != aspectRatio || this.wasRotated || this.configChanged))
                {
                    this.wasRotated = false;
                    this.configChanged = false;
                    this.aspectRatio = ratio;

                    int imageRotation = TofArManager.Instance.GetScreenOrientation();

                    if (isColor)
                    {
                        UpdateTransformColor(imageRotation, ratio);
                    }
                    else
                    {
                        UpdateTransformDepth(imageRotation, ratio);
                    }
                }
            }
        }

    }
}
