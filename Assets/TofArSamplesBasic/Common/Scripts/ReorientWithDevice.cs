/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;

namespace TofArSamples
{
    public class ReorientWithDevice : MonoBehaviour
    {
        private DeviceOrientation previousOrientation = DeviceOrientation.Unknown;
        private bool orientationChanged = true;

        private void OnEnable()
        {
            TofArManager.OnDeviceOrientationUpdated += OnDeviceOrientationChanged;

            UpdateRotation();
        }

        private void OnDisable()
        {
            TofArManager.OnDeviceOrientationUpdated -= OnDeviceOrientationChanged;
        }

        /// <summary>
        /// DeviceOrientation ChangedCallback
        /// </summary>
        /// <param name="previousDeviceOrientation">previousDeviceOrientation</param>
        /// <param name="newDeviceOrientation">newDeviceOrientation</param>
        public void OnDeviceOrientationChanged(DeviceOrientation previousDeviceOrientation, DeviceOrientation newDeviceOrientation)
        {
            UpdateRotation();
        }

        /// <summary>
        /// Update rotation
        /// </summary>
        public void UpdateRotation()
        {
            orientationChanged = true;
        }

        private void Update()
        {
            if (orientationChanged)
            {
                orientationChanged = false;
                RotateCameraByDeviceOrientation();
            }
        }

        void RotateCameraByDeviceOrientation()
        {
            DeviceOrientation currentOrientation = GetOrientation();
            switch (currentOrientation)
            {
                case DeviceOrientation.FaceDown:
                case DeviceOrientation.FaceUp:
                case DeviceOrientation.Unknown:
                    currentOrientation = previousOrientation;
                    break;
            }

            if (previousOrientation != currentOrientation)
            {
                var rotation = GetRotationAngle(currentOrientation) - GetRotationAngle(previousOrientation);
                this.transform.RotateAround(Vector3.zero, Vector3.forward, rotation);
                previousOrientation = currentOrientation;

            }
        }
        static float GetRotationAngle(DeviceOrientation orientation)
        {
            var rotationAngle = 0f;
            switch (orientation)
            {
                case DeviceOrientation.Portrait:
                    rotationAngle = -90f;
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    rotationAngle = 90f;
                    break;
                case DeviceOrientation.LandscapeLeft:
                    rotationAngle = 0f;
                    break;
                case DeviceOrientation.LandscapeRight:
                    rotationAngle = 180f;
                    break;
            }
            return rotationAngle;
        }

        DeviceOrientation GetOrientation()
        {
            DeviceOrientation result = TofArManager.Instance.GetProperty<DeviceOrientationsProperty>().deviceOrientation;

            if (result == DeviceOrientation.Unknown)
            {
                if (Screen.width < Screen.height)
                {
                    result = DeviceOrientation.Portrait;
                }
                else
                {
                    result = DeviceOrientation.LandscapeLeft;
                }
            }
            return result;
        }
    }
}
