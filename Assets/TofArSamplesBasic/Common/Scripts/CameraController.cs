/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TofArSamples
{
    public class CameraController : MonoBehaviour
    {
        public float rotateParam = 0.5f;

        public float zoomParam = 0.5f;

        public float moveParam = 0.5f;

        public float rotationCenterDistance = 3f;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private float landscapeFov;

#if !UNITY_EDITOR
        private float previousDist = 0;
        private int previousFingerId = 0;
#endif
        private Vector2 previousCenterPoint = Vector2.zero;
        private Vector2 previousTouchPoint = Vector2.zero;


        private DeviceOrientation previousOrientation = DeviceOrientation.Unknown;

#if UNITY_EDITOR
        bool leftDown = false;
        bool middleDown = false;
#endif

        private bool orientationChanged = true;

        void Start()
        {
            initialPosition = Camera.main.transform.position;
            initialRotation = Camera.main.transform.rotation;
            landscapeFov = Camera.main.fieldOfView;
        }

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
        /// UpdateRotation
        /// </summary>
        public void UpdateRotation()
        {
            orientationChanged = true;
        }

        /// <summary>
        /// DeviceOrientation ChangeCallback
        /// </summary>
        /// <param name="previousDeviceOrientation"></param>
        /// <param name="newDeviceOrientation"></param>
        public void OnDeviceOrientationChanged(DeviceOrientation previousDeviceOrientation, DeviceOrientation newDeviceOrientation)
        {
            UpdateRotation();
        }

        void Update()
        {
            if (orientationChanged)
            {
                orientationChanged = false;
                RotateCameraByDeviceOrientation();
            }
#if UNITY_EDITOR
            bool lDown = this.leftDown;
            bool mDown = this.middleDown;

            if (Input.GetMouseButtonDown(0))
            {
                leftDown = true;
                previousTouchPoint = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                leftDown = false;
            }

            if (Input.GetMouseButtonDown(2))
            {
                middleDown = true;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                middleDown = false;
            }


            if (leftDown) // touchCount = 1
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                if (this.leftDown != lDown) // touchPhase began
                {
                    previousTouchPoint = Input.mousePosition;
                }
                else if (this.leftDown == lDown) // moved?
                {
                    var val = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - previousTouchPoint) / Screen.dpi;
                    val *= rotateParam * Mathf.Rad2Deg;
                    previousTouchPoint = Input.mousePosition;

                    var cameraTransform = Camera.main.transform;
                    var rotationPoint = cameraTransform.position + rotationCenterDistance * cameraTransform.forward;

                    cameraTransform.RotateAround(rotationPoint, cameraTransform.right, -val.y);
                    cameraTransform.RotateAround(rotationPoint, cameraTransform.up, val.x);
                }

            }
            else if (middleDown) // touchCount = 2
            {
                if (this.middleDown != mDown) // touchPhase began
                {
                    previousCenterPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
                else if (this.middleDown == mDown) // moved?
                {
                    var center = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    var moveXY = -moveParam * ((center - previousCenterPoint) / Screen.dpi);

                    previousCenterPoint = center;

                    Camera.main.transform.Translate(moveXY.x, moveXY.y, 0);
                }
            }

            // zoom
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                var moveZ = Input.mouseScrollDelta.y * Time.deltaTime;

                Camera.main.transform.Translate(0, 0, moveZ);
            }





#else
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    previousFingerId = -1;
                    return;
                }

                if (touch.phase == TouchPhase.Began)
                {
                    previousTouchPoint = touch.position;
                    previousFingerId = touch.fingerId;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (touch.fingerId != previousFingerId)
                    {
                        previousTouchPoint = touch.position;
                        previousFingerId = touch.fingerId;
                        return;
                    }

                    var val = (touch.position - previousTouchPoint) / Screen.dpi;
                    val *= rotateParam * Mathf.Rad2Deg;
                    previousTouchPoint = touch.position;

                    var cameraTransform = Camera.main.transform;
                    var rotationPoint = cameraTransform.position + rotationCenterDistance * cameraTransform.forward;

                    cameraTransform.RotateAround(rotationPoint, cameraTransform.right, -val.y);
                    cameraTransform.RotateAround(rotationPoint, cameraTransform.up, val.x);
                }
            }
            else if (Input.touchCount >= 2)
            {
                previousFingerId = -1;

                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch2.phase == TouchPhase.Began)
                {
                    previousDist = Vector2.Distance(touch1.position, touch2.position);
                    previousCenterPoint = Vector2.Lerp(touch1.position, touch2.position, 0.5f);
                }
                else if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
                {
                    var newDist = Vector2.Distance(touch1.position, touch2.position);
                    var moveZ = zoomParam * ((newDist - previousDist) / Screen.dpi);

                    previousDist = newDist;

                    var center = Vector2.Lerp(touch1.position, touch2.position, 0.5f);
                    var moveXY = -moveParam * ((center - previousCenterPoint) / Screen.dpi);

                    previousCenterPoint = center;

                    Camera.main.transform.Translate(moveXY.x, moveXY.y, moveZ);
                }
            }
#endif
        }

        /// <summary>
        /// Reset Position
        /// </summary>
        public void ResetPosition()
        {
            Camera.main.transform.SetPositionAndRotation(initialPosition, initialRotation);
            previousOrientation = DeviceOrientation.Unknown;
            RotateCameraByDeviceOrientation();
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

            TofArManager.Logger.WriteLog(LogLevel.Debug, $"Get device orientation: {currentOrientation}");

            if (previousOrientation != currentOrientation)
            {
                previousOrientation = currentOrientation;

                // adujst camera FOV
                switch (currentOrientation)
                {
                    case DeviceOrientation.Portrait:
                    case DeviceOrientation.PortraitUpsideDown:
                        var aspectRatio = (float)Screen.currentResolution.height / Screen.currentResolution.width;
                        if (aspectRatio < 1)
                        {
                            aspectRatio = 1 / aspectRatio;
                        }
                        Camera.main.fieldOfView = Mathf.Atan(Mathf.Tan(landscapeFov / 2 * Mathf.Deg2Rad) * aspectRatio) * 2 * Mathf.Rad2Deg;
                        break;
                    case DeviceOrientation.LandscapeLeft:
                    case DeviceOrientation.LandscapeRight:
                        Camera.main.fieldOfView = landscapeFov;
                        break;
                }

                TofArManager.Logger.WriteLog(LogLevel.Debug, $"FOV set to: {Camera.main.fieldOfView}");
            }
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
