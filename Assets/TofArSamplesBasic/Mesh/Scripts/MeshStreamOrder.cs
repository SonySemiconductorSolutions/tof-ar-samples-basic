/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using TofArSettings.Mesh;
using TofArSettings.Segmentation;
using UnityEngine;

namespace TofArSamples.Mesh
{
    public class MeshStreamOrder : MonoBehaviour
    {
        private MeshManagerController meshManagerController;

        protected void Awake()
        {
            meshManagerController = FindObjectOfType<MeshManagerController>();
        }

        protected void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnStreamStopped += OnTofStreamStopped;
        }

        protected void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnStreamStopped -= OnTofStreamStopped;
        }

        private void OnTofStreamStarted(object sender, UnityEngine.Texture2D depth, UnityEngine.Texture2D conf, PointCloudData pc)
        {
            meshManagerController.OnTofStreamStarted(sender, depth, conf, pc);
        }

        private void OnTofStreamStopped(object sender)
        {
            meshManagerController.OnTofStreamStopped(sender);
        }
    }
}
