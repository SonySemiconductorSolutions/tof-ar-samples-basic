/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings.Plane;
using UnityEngine;

namespace TofArSamples.Plane
{
    public class PlaneStreamOrder : MonoBehaviour
    {
        private PlaneManagerController planeManagerController;

        protected void Awake()
        {
            planeManagerController = FindObjectOfType<PlaneManagerController>();
        }

        protected void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnStreamStarted;
            TofArTofManager.OnStreamStopped += OnStreamStopped;
        }

        protected void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnStreamStarted;
            TofArTofManager.OnStreamStopped -= OnStreamStopped;
        }

        private void OnStreamStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            planeManagerController.OnStreamStarted(sender, depthTexture, confidenceTexture, pointCloudData);
        }

        private void OnStreamStopped(object sender)
        {
            planeManagerController.OnStreamStopped(sender);
        }
    }
}
