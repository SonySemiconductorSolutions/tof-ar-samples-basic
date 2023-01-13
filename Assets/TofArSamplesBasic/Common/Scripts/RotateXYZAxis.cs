/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
namespace TofArSamples
{
    public class RotateXYZAxis : MonoBehaviour
    {
        private GameObject mainCamera;

        private void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Update()
        {
            RotateWithCamera();
        }

        private void RotateWithCamera()
        {
            gameObject.transform.rotation = Quaternion.Inverse(mainCamera.transform.rotation);
        }
    }
}
