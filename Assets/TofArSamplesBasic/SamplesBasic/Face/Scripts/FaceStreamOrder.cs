/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0.Color;
using TofAr.V0.Face;
using TofAr.V0.Tof;
using TofArSettings.Face;
using UnityEngine;

namespace TofArSamples.Body
{
    public class FaceStreamOrder : MonoBehaviour
    {
        private FaceManagerController faceManagerController;

        protected void Awake()
        {
            faceManagerController = FindObjectOfType<FaceManagerController>();
        }

        protected void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;

            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
            TofArColorManager.OnStreamStopped += OnColorStreamStopped;
        }

        protected void OnDisable()
        {
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
            TofArColorManager.OnStreamStopped -= OnColorStreamStopped;

            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;
        }

        protected void Start()
        {
            if (TofArFaceManager.Instance.DetectorType == FaceDetectorType.Internal_ARKit)
            {
                StartCoroutine(WaitAndStartFace());
            }
        }

        IEnumerator WaitAndStartFace()
        {
            yield return new WaitForSeconds(1.0f);
            faceManagerController.StartStream();
        }

        private void OnTofStreamStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            faceManagerController.OnTofStreamStarted(sender, depthTexture, confidenceTexture, pointCloudData);
        }

        private void OnTofStreamStopped(object sender)
        {
            faceManagerController.OnTofStreamStopped(sender);
        }

        private void OnColorStreamStarted(object sender, Texture2D colorTexture)
        {
            faceManagerController.OnColorStreamStarted(sender, colorTexture);
        }

        private void OnColorStreamStopped(object sender)
        {
            faceManagerController.OnColorStreamStopped(sender);
        }
    }
}
