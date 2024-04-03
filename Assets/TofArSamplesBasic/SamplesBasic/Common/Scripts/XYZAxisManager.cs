/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using TofArSettings.UI;
using TofAr.V0;

namespace TofArSamples
{
    public class XYZAxisManager : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler = null;
        [SerializeField] private GameObject XYZModel;
        [SerializeField] private float realBarWidth = 8;

        private GameObject mainCamera;
        public RectTransform SafeAreaRt { get; private set; }
        
        public Vector2 SafeAreaSize
        {
            get
            {
                return (SafeAreaRt) ? new Vector2(SafeAreaRt.rect.width,
                    SafeAreaRt.rect.height) : Vector2.zero;
            }
        }

        protected Rect area;
        protected Vector2 baseReso;
        protected Toolbar toolbar;
        protected Vector2 latestSafeAreaSize;

        private float dpi;

        private void Awake()
        {
            // Get UI
            if (!canvasScaler)
            {
                canvasScaler = FindObjectOfType<CanvasScaler>();
            }

            foreach (var rt in canvasScaler.GetComponentsInChildren<RectTransform>())
            {
                if (rt.name.Contains("SafeArea"))
                {
                    SafeAreaRt = rt;
                    break;
                }
            }

            baseReso = canvasScaler.referenceResolution;
        }

        private void Start()
        {
            toolbar = FindObjectOfType<Toolbar>();
            dpi = GetDPI();

            AdjustSafeArea(Screen.safeArea);

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Update()
        {
            AdjustSafeArea(Screen.safeArea);

            RotateWithCamera();
        }

        private void RotateWithCamera()
        {
            XYZModel.transform.rotation = Quaternion.Inverse(mainCamera.transform.rotation);
        }

        private void AdjustSafeArea(Rect newArea)
        {
            // Do not do anything if SafeArea has not changed
            if (area == newArea && latestSafeAreaSize == SafeAreaSize)
            {
                return;
            }

            if (Application.isEditor)
            {
                return;
            }

            area = newArea;

            float scWidth = Screen.width;
            float scHeight = Screen.height;

            // Calculate the actual saize per pixel from the screen width and CanvasScaler's ReferenceResolution
            float realScWidth = (scWidth < scHeight) ? scWidth : scHeight;

            realScWidth *= 25.4f / dpi;
            float pixelSize = realScWidth / baseReso.x;

            // Scale the UI so that the toolbar width matches the actual size
            float barWidth = toolbar.BarWidth * pixelSize;
            float ratio = realBarWidth / barWidth;
            canvasScaler.referenceResolution = baseReso / ratio;

            // Adjust the UI area to fit within the SafeArea
            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= scWidth;
            anchorMin.y /= scHeight;
            anchorMax.x /= scWidth;
            anchorMax.y /= scHeight;
            SafeAreaRt.anchoredPosition = Vector2.zero;
            SafeAreaRt.anchorMin = anchorMin;
            SafeAreaRt.anchorMax = anchorMax;

            latestSafeAreaSize = SafeAreaSize;
        }

        private float GetDPI()
        {
            if (TofArManager.Instance != null)
            {
                var deviceCapability = TofArManager.Instance.GetProperty<DeviceCapabilityProperty>();
                string modelName = deviceCapability.modelName;

                if (modelName.Equals("iPhone15,4")) //iPhone 15 6.1inch 1179x2556
                {
                    return 461;
                }
                else if (modelName.Equals("iPhone15,5")) //iPhone 15 Plus 6.7inch 1290x2796
                {
                    return 460;
                }
                else if (modelName.Equals("iPhone16,1")) //iPhone 15 Pro 6.1inch 1179x2556
                {
                    return 461;
                }
                else if (modelName.Equals("iPhone16,2")) //iPhone 15 Pro Max 6.7inch 1290x2796
                {
                    return 460;
                }
                else
                {
                    return Screen.dpi;
                }
            }
            else
            {
                return Screen.dpi;
            }
        }
    }
}
