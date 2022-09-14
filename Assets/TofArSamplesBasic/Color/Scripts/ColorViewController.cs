/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using TofArSettings.Color;
using UnityEngine;

namespace TofArSamples.Color
{
    public class ColorViewController : ImageViewController
    {
        public TextureMapper3D TexMapper3d;

        public TextureMapperRawImage TexMapperRawImg;

        protected override void Awake()
        {
            base.Awake();
            mgrCtrl = FindObjectOfType<ColorManagerController>();
        }

        protected override void Start()
        {
            // Should only be one of each ColorView
            if (!TexMapper3d)
            {
                TexMapper3d = FindObjectOfType<TextureMapper3D>();
            }

            if (!TexMapperRawImg)
            {
                TexMapperRawImg = FindObjectOfType<TextureMapperRawImage>();
            }

            base.Start();
        }

        protected override void AdjustAspect(int index = 0)
        {
            base.AdjustAspect(index);

            // Calculate aspect ratio from resolution
            var colorMgrCtrl = mgrCtrl as ColorManagerController;
            if (colorMgrCtrl == null)
            {
                return;
            }
            var prop = colorMgrCtrl.CurrentResolution;
            if (prop == null || prop.width <= 0 || prop.height <= 0)
            {
                return;
            }
        }

        protected override bool ExistQuad()
        {
            return (TexMapper3d);
        }

        protected override bool ExistRawImage()
        {
            return (TexMapperRawImg);
        }

        protected override RectTransform GetRawImageRt()
        {
            return TexMapperRawImg.GetComponent<RectTransform>();
        }

        protected override void ChangeMaximize(bool onOff)
        {
            TexMapperRawImg.Maximize = onOff;
        }

        protected override void ShowQuad(bool onOff)
        {
            base.ShowQuad(onOff);

            if (TexMapper3d)
            {
                Renderer renderer = TexMapper3d.gameObject.GetComponent<Renderer>();

                if (renderer.enabled != onOff)
                {
                    renderer.enabled = onOff;
                }
            }
        }

        protected override void ShowRawImage(bool onOff)
        {
            base.ShowRawImage(onOff);

            if (TexMapperRawImg)
            {
                UnityEngine.UI.RawImage rawImage = TexMapperRawImg.gameObject.GetComponent<UnityEngine.UI.RawImage>();

                if (rawImage.enabled != onOff)
                {
                    rawImage.enabled = onOff;
                }
            }
        }

        protected override int GetPositionIndex()
        {
            return 0;
        }

        protected override Vector2 GetAdjustedSize()
        {
            var currentResolution= TofArColorManager.Instance.GetProperty<ResolutionProperty>();

            float defWidth = defaultImgSize.x;

            float ratio = (float)currentResolution.width / currentResolution.height;

            int width = (int)(defWidth );
            int height = (int)(defWidth / ratio);

            return new Vector2(width, height);

        }
    }
}
