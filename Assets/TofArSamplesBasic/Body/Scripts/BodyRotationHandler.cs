/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Body;
using TofAr.V0.Body.SV2;
using UnityEngine;

namespace TofArSamples.Body
{
    public class BodyRotationHandler : MonoBehaviour
    {

        private void OnEnable()
        {
            TofArBodyManager.OnStreamStarted += TofArBodyManager_OnStreamStarted;
            TofArManager.OnScreenOrientationUpdated += TofArManager_OnScreenOrientationUpdated;
        }

        private void OnDisable()
        {
            TofArBodyManager.OnStreamStarted -= TofArBodyManager_OnStreamStarted;
            TofArManager.OnScreenOrientationUpdated -= TofArManager_OnScreenOrientationUpdated;
        }

        private void TofArManager_OnScreenOrientationUpdated(ScreenOrientation previousScreenOrientation, ScreenOrientation newScreenOrientation)
        {
            UpdateOrientation();
        }

        private void TofArBodyManager_OnStreamStarted(object sender)
        {
            UpdateOrientation();
        }

        private void UpdateOrientation()
        {
            var detectorTypeProperty = TofArBodyManager.Instance.GetProperty<DetectorTypeProperty>();
            
            if (TofArBodyManager.Instance.IsPlaying && detectorTypeProperty.detectorType == BodyPoseDetectorType.External)
            {
                var prop = TofArBodyManager.Instance.GetProperty<CameraOrientationProperty>();

                var orientation = prop.cameraOrientation;

                int screenRotation = 0;
                switch (orientation)
                {
                    case CameraOrientation.Portrait:
                        screenRotation = 270; break;
                    case CameraOrientation.LandscapeRight:
                        screenRotation = 180; break;
                    case CameraOrientation.PortraitUpsideDown:
                        screenRotation = 90; break;
                }

                int currentScreenOrientation = TofArManager.Instance.GetScreenOrientation();

                Debug.Log($"Body rotation={screenRotation} compared to screen={currentScreenOrientation}");

                this.transform.localRotation = Quaternion.Euler(0, 0, (currentScreenOrientation - screenRotation));
            }
            else
            {
                this.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

    }
}
