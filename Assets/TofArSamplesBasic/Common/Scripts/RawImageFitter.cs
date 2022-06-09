/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples
{
    public class RawImageFitter : MonoBehaviour
    {
        protected CameraManagerController mgrCtrl;
        ScreenRotateController scRotCtrl;

        protected AspectRatioFitter aspectFitter;
        protected RectTransform rt;
        RectTransform parentRt;

        protected virtual void Awake()
        {
            // Executed after the child class
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            parentRt = rt.parent.GetComponent<RectTransform>();

            SetupAspectFitterComponent();
        }

        void OnEnable()
        {
            scRotCtrl.OnRotateScreen += OnRotateScreen;
            mgrCtrl.OnChangeAfter += OnChangeImageSize;
        }

        void OnDisable()
        {
            scRotCtrl.OnRotateScreen -= OnRotateScreen;
            mgrCtrl.OnChangeAfter -= OnChangeImageSize;
        }

        /// <summary>
        /// Execute AspectRatioFitter setup
        /// </summary>
        protected virtual void SetupAspectFitterComponent()
        {
        }

        /// <summary>
        /// Get resolution
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        protected virtual void GetImageSize(out float width, out float height)
        {
            width = 0;
            height = 0;
        }

        /// <summary>
        /// Adjust UI
        /// </summary>
        public void Adjust()
        {
            var scale = Vector3.one;

            // The display is rotated after the long side is fitted to the width, so enlarge and fit the side to the width
            // If the parent is horizontal, do not enlarge as it will protrude
            if (scRotCtrl.IsPortrait &&
                parentRt.sizeDelta.x < parentRt.sizeDelta.y)
            {
                scale *= aspectFitter.aspectRatio;
            }

            rt.localScale = scale;
        }

        /// <summary>
        /// Event that is called when screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            Adjust();
        }

        /// <summary>
        /// Event that is called when Camera image size is changed
        /// </summary>
        /// <param name="index">CameraConfig/Resolution index</param>
        void OnChangeImageSize(int index)
        {
            // Calculate aspect ratio from resolution
            GetImageSize(out float width, out float height);
            if (width <= 0 || height <= 0)
            {
                return;
            }

            aspectFitter.aspectRatio = width / height;
            Adjust();
        }
    }
}
