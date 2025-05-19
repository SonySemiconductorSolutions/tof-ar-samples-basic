/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSamples.BasicStream
{
    [RequireComponent(typeof(RectTransform))]
    public class FollowRectTransform : MonoBehaviour
    {
        public RectTransform followTo = null;
        private RectTransform selfTransform = null;

        void Start()
        {
            this.selfTransform = this.GetComponent<RectTransform>();
        }

        void Update()
        {
            this.selfTransform.anchorMin = this.followTo.anchorMin;
            this.selfTransform.anchorMax = this.followTo.anchorMax;
            this.selfTransform.anchoredPosition = this.followTo.anchoredPosition;
            this.selfTransform.sizeDelta = this.followTo.sizeDelta;

            this.selfTransform.localPosition = this.followTo.localPosition;
            this.selfTransform.localRotation = this.followTo.localRotation;
            this.selfTransform.localScale = this.followTo.localScale;
        }
    }
}