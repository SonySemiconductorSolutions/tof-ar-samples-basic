/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings.Tof;
using UnityEngine;

namespace TofArSamples.Tof
{

    public class DepthViewController : ImageViewController
    {
        public TextureMapper3D TexMapper3d;

        public TextureMapperRawImage TexMapperRawImg;

        protected override void Awake()
        {
            base.Awake();
            mgrCtrl = FindObjectOfType<TofManagerController>();
        }

        protected override void Start()
        {
            // Should only be one of each DepthView
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
            var tofMgrCtrl = mgrCtrl as TofManagerController;
            if (tofMgrCtrl == null)
            {
                return;
            }
            var prop = tofMgrCtrl.CurrentConfig;
            if (prop == null || prop.width <= 0 || prop.height <= 0)
            {
                return;
            }
        }

        protected override bool ExistQuad()
        {
            return TexMapper3d;
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

            Renderer rendererDepth = null;
            if (TexMapper3d)
            {
                rendererDepth = TexMapper3d.gameObject.GetComponent<Renderer>();
                rendererDepth.enabled = false;
            }

            if (rendererDepth && rendererDepth.enabled != onOff)
            {
                rendererDepth.enabled = onOff;
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
            return 1;
        }
    }
}
