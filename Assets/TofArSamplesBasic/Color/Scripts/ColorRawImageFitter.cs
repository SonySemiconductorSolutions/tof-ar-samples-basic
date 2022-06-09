/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using TofArSettings.Color;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples.Color
{
    public class ColorRawImageFitter : RawImageFitter
    {
        [SerializeField]
        TextureMapperRawImage texMapper = null;

        protected override void Awake()
        {
            mgrCtrl = FindObjectOfType<ColorManagerController>();
            rt = texMapper.GetComponent<RectTransform>();

            base.Awake();
        }

        protected override void SetupAspectFitterComponent()
        {
            base.SetupAspectFitterComponent();

            if (!texMapper.TryGetComponent(out aspectFitter))
            {
                aspectFitter = texMapper.gameObject.AddComponent<AspectRatioFitter>();
                aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            }

            aspectFitter.aspectRatio = texMapper.AspectRatio;
        }

        protected override void GetImageSize(out float width, out float height)
        {
            var colorMgrCtrl = mgrCtrl as ColorManagerController;
            width = height = 0;
            if (colorMgrCtrl != null)
            {
                var prop = colorMgrCtrl.CurrentResolution;
                width = prop.width;
                height = prop.height;
            }
        }
    }
}
