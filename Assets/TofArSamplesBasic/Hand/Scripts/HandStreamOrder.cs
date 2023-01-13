/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings.Hand;
using UnityEngine;

namespace TofArSamples.Hand
{
    public class HandStreamOrder : MonoBehaviour
    {
        private HandManagerController handManagerController;

        protected void Awake()
        {
            handManagerController = FindObjectOfType<HandManagerController>();
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

        private void OnStreamStarted(object sender, Texture2D depthTexture,Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            handManagerController.OnStreamStarted(sender, depthTexture, confidenceTexture, pointCloudData);
        }

        private void OnStreamStopped(object sender)
        {
            handManagerController.OnStreamStopped(sender);
        }
    }
}
