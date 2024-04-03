/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofAr.V0.Hand;
using TofArSettings;
using UnityEngine;
using System.Linq;
using TofAr.V0.Mesh;

namespace TofArSamples.Hand
{
    public class HandOcclusionController : ControllerBase
    {
        DynamicMesh dynamicMesh;

        const float defaultClippingDistance = 1000;

        public event ChangeToggleEvent OnChangeHandOcclusion;

        public event ChangeValueEvent OnChangeClippingDistance;

        private void Awake()
        {
            dynamicMesh = FindObjectOfType<DynamicMesh>();
        }

        private void OnEnable()
        {
            TofArHandManager.OnFrameArrived += HandFrameArrived;
        }

        private void OnDisable()
        {
            TofArHandManager.OnFrameArrived -= HandFrameArrived;
        }

        public const float maxClippingDistance = 0.3f;
        public const float minClippingDistance = 0;
        public const float clippingDistanceStep = 0.01f;

        private float clippingDistance = 0.1f;

        public float ClippingDistance
        {
            get => clippingDistance;
            set
            {
                if(clippingDistance != value)
                {
                    clippingDistance = value;
                    OnChangeClippingDistance?.Invoke(clippingDistance);
                }
            }
        }

        private bool isHandOnlyOcclusion = true;

        public bool IsHandOnlyOcclusion
        {
            get => isHandOnlyOcclusion;
            set
            {
                if (value != isHandOnlyOcclusion)
                {
                    isHandOnlyOcclusion = value;
                    dynamicMesh.ClippingDistance = value ? clippingDistance : defaultClippingDistance;
                    OnChangeHandOcclusion?.Invoke(value);
                }
            }
        }

        private void HandFrameArrived(object sender)
        {
            if (!isHandOnlyOcclusion)
            {
                dynamicMesh.ClippingDistance = defaultClippingDistance;
            }
            var manager = sender as TofArHandManager;
            if (manager == null)
            {
                return;
            }
            float distanceRight = -1;
            float distanceLeft = -1;
            if (manager.HandData.Data.featurePointsLeft != null)
            {
                distanceLeft = manager.HandData.Data.featurePointsLeft[(int)HandPointIndex.HandCenter].magnitude;
            }

            if (manager.HandData.Data.featurePointsRight != null)
            {
                distanceRight = manager.HandData.Data.featurePointsRight[(int)HandPointIndex.HandCenter].magnitude;
            }
            if (dynamicMesh != null && isHandOnlyOcclusion)
            {
                dynamicMesh.ClippingDistance = (distanceRight == 0 && distanceLeft == 0) ? 0 : Mathf.Max(distanceLeft, distanceRight) + clippingDistance;
            }

        }
    }
}
