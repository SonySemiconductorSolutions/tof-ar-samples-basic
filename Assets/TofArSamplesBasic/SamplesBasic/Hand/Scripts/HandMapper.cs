/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Tof;
using UnityEngine;
using TofArSettings;
using TofAr.V0.Hand;
using System.Threading;
using System.Collections;

namespace TofArSamples.Hand
{
    public class HandMapper : MonoBehaviour
    {
        Camera camMain, camHand;

        /// <summary>
        /// Overlay Hand on Color image
        /// </summary>
        public bool RemapToColor = true;

        ReorientRelativeColorCamera reorient;
        ScreenRotateController scRotCtrl;
        CalibrationSettingsProperty settings;
        SynchronizationContext context;

        private float screenAspect;

        void Awake()
        {
            context = SynchronizationContext.Current;
            // Get camera
            camMain = GetComponent<Camera>();
            foreach (var cam in GetComponentsInChildren<Camera>())
            {
                if (cam != camMain)
                {
                    camHand = cam;
                    break;
                }
            }

            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            if (scRotCtrl.IsPortraitDevice)
            {
                screenAspect = (float)Screen.width / Screen.height;
            }
            else
            {
                screenAspect = (float)Screen.height / Screen.width;
            }


            // Disable unnecessary objects if not remapping
            if (!RemapToColor)
            {
                reorient = FindObjectOfType<ReorientRelativeColorCamera>();
                reorient.enabled = false;
                reorient.transform.localPosition = Vector3.zero;
                reorient.transform.localRotation = Quaternion.identity;

                var depthViewQuad = reorient.transform.GetComponentInChildren<TofQuadAspectFitter>();
                depthViewQuad.AutoRotate = true;
                var realHandModels = reorient.transform.GetComponentInChildren<RealHandModel>();
                realHandModels.autoRotate = true;
                var handModels = reorient.transform.GetComponentsInChildren<HandModel>();
                foreach (var h in handModels)
                {
                    h.AutoRotate = true;
                }

                // Set the Camera for Hand to not be used, but set MainCamera to show everything
                camHand.enabled = false;
                camMain.cullingMask |= (1 << LayerMask.NameToLayer("Hands"));

                enabled = false;
            }
        }

        void OnEnable()
        {
            TofArTofManager.Instance?.CalibrationSettingsLoaded.AddListener(
                UpdateProjectionMatrix);
            scRotCtrl.OnRotateDevice += OnRotateDevice;
        }

        void OnDisable()
        {
            TofArTofManager.Instance?.CalibrationSettingsLoaded.RemoveListener(
                UpdateProjectionMatrix);
            if (scRotCtrl)
            {
                scRotCtrl.OnRotateDevice -= OnRotateDevice;
            }
        }

        /// <summary>
        ///  Event that is called when screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateDevice(ScreenOrientation ori)
        {
            UpdateProjectionMatrix(settings);
        }

        /// <summary>
        /// Set Camera ProjectionMatrix from calibration settings
        /// </summary>
        /// <param name="settings">Calibration settings</param>
        void UpdateProjectionMatrix(CalibrationSettingsProperty settings)
        {
            if (!RemapToColor || settings == null ||
                settings.colorWidth <= 0 || settings.colorHeight <= 0)
            {
                return;
            }
            this.settings = settings;
            context.Post((s) => StartCoroutine(UpdateProjectionMatrixCoroutine()), null);
        }

        //make sure the screen size parameters have also had time to update before using them
        private IEnumerator UpdateProjectionMatrixCoroutine()
        {
            yield return new WaitForEndOfFrame();
            // Create ProjectionMatrix
            float right = settings.colorWidth * camHand.nearClipPlane / (2 * settings.c.fx);
            float top = settings.colorHeight * camHand.nearClipPlane / (2 * settings.c.fy);
            float rightOffset = ((settings.c.cx / settings.colorWidth) - 0.5f) * camHand.nearClipPlane;
            float topOffset = ((settings.c.cy / settings.colorHeight) - 0.5f) * camHand.nearClipPlane;

            Matrix4x4 pMatrix;
            if (scRotCtrl.IsPortraitDevice)
            {
                pMatrix = Matrix4x4.Frustum(topOffset - top,
                    topOffset + top, rightOffset - right, rightOffset + right,
                    camHand.nearClipPlane, camHand.farClipPlane);
            }
            else
            {
                pMatrix = Matrix4x4.Frustum(rightOffset - right,
                    rightOffset + right, topOffset - top, topOffset + top,
                    camHand.nearClipPlane, camHand.farClipPlane);
            }

            // Calculate FoV from ProjectionMatrix
            camHand.fieldOfView = Mathf.Atan(1 / pMatrix[1, 1]) * 2 * Mathf.Rad2Deg;

            AdjustAspect(settings.colorWidth, settings.colorHeight);
        }

        /// <summary>
        /// Adjust screen aspect ratio to fit the Color image
        /// </summary>
        void AdjustAspect(float imgWidth, float imgHeight)
        {
            float scAspect = screenAspect;
            float imgW = imgWidth;
            float imgH = imgHeight;
            if (scRotCtrl.IsPortraitDevice)
            {
                scAspect = 1 / scAspect;
                imgW = imgHeight;
                imgH = imgWidth;
            }

            float camWidth = (imgW * scAspect) / imgH;

            camHand.fieldOfView *= camWidth;
        }
    }
}
