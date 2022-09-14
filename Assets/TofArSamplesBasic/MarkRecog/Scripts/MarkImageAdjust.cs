/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofArSettings;
using UnityEngine;
using UI = TofArSettings.UI;

namespace TofArSamples
{
    public class MarkImageAdjust : MonoBehaviour
    {
        ScreenRotateController scRotCtrl;
        UI.Toolbar toolbar;
        RectTransform rawImgRt;

        Vector2 initPos = Vector2.zero;

        private void Awake()
        {
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            toolbar = FindObjectOfType<UI.Toolbar>();
            rawImgRt = GetComponent<RectTransform>();

            if (rawImgRt != null)
            {
                initPos = rawImgRt.anchoredPosition;
            }
        }

        private void OnEnable()
        {
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateScreen += OnTofArScreenRotated;

                ApplyAnchor();
            }
        }

        private void OnDisable()
        {
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateScreen -= OnTofArScreenRotated;
            }
        }

        private void OnTofArScreenRotated(ScreenOrientation ori)
        {
            ApplyAnchor();
        }

        void ApplyAnchor()
        {
            if (!rawImgRt || !toolbar || !scRotCtrl)
            {
                return;
            }

            Vector2 pos = initPos;
            
            if (!scRotCtrl.IsPortrait)
            {
                pos.x -= toolbar.BarWidth;
            }

            rawImgRt.anchoredPosition = pos;
        }
    }
}
