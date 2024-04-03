/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings.Tof;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples.Tof
{
    public class DepthRawImageFitter : RawImageFitter
    {
        /// <summary>
        /// DepthViewRawImage
        /// </summary>
        [SerializeField]
        TextureMapperRawImage texMapper = null;

        protected override void Awake()
        {
            mgrCtrl = FindObjectOfType<TofManagerController>();
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
            var tofMgrCtrl = mgrCtrl as TofManagerController;
            width = height = 0;
            if (tofMgrCtrl != null)
            {
                var prop = tofMgrCtrl.CurrentConfig;
                width = prop.width;
                height = prop.height;
            }
            
        }
    }
}
