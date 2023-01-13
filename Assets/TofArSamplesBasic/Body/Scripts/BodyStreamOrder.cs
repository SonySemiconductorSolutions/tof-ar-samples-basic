/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings.Body;
using UnityEngine;

namespace TofArSamples.Body
{
    public class BodyStreamOrder : MonoBehaviour
    {
        private BodyManagerController bodyManagerController;

        protected void Awake()
        {
            bodyManagerController = FindObjectOfType<BodyManagerController>();
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
            bodyManagerController.OnStreamStarted(sender, depthTexture, confidenceTexture, pointCloudData);
        }

        private void OnStreamStopped(object sender)
        {
            bodyManagerController.OnStreamStopped(sender);
        }
    }
}
