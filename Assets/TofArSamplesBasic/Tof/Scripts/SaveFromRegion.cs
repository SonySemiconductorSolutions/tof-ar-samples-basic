/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Tof;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples.Tof
{
    public class SaveFromRegion : MonoBehaviour
    {
        [System.Serializable]
        private class FixedInternalParameters
        {
            public string fx = null;
            public string fy = null;
            public string cx = null;
            public string cy = null;
        }

        [System.Serializable]
        private class SettingsJson
        {
            public string id = null;
            public string depthRotation = null;
            public string depthMirrorHorizontal = null;
            public string depthMirorVertical = null;
            public string defaultResolutionWidth = null;
            public string defaultResolutionHeight = null;
            public string useMaxFilter = null;
            public string useDepthCorrection = null;
            public string depthCorrectionGain = null;
            public string depthCorrectionOffset = null;
            public string useDepthInvalidValueConvert = null;
            public string depthInvalidValueFrom = null;
            public string depthInvalidValueTo = null;
            public FixedInternalParameters fixedInternalParameters = null;
            public string isDepthPrivate = null;
        }

        [System.Serializable]
        private class ParametersJsonData
        {
            public string name = null;
            public SettingsJson[] cameraSettings = null;
        }
        [System.Serializable]
        private class ParametersJsonDataOld
        {
            public string name = null;
            public string depthRotation = null;
            public string depthMirrorHorizontal = null;
            public string depthMirorVertical = null;
            public string defaultResolutionWidth = null;
            public string defaultResolutionHeight = null;
            public string useMaxFilter = null;
            public string useDepthCorrection = null;
            public string depthCorrectionGain = null;
            public string depthCorrectionOffset = null;
            public string useDepthInvalidValueConvert = null;
            public string depthInvalidValueFrom = null;
            public string depthInvalidValueTo = null;
            public FixedInternalParameters fixedInternalParameters = null;
            public string isDepthPrivate = null;
        }
        public int SaveFrames { get; set; } = 30;
        public int RegionWidth { get; set; } = 11;
        public int DepthWidth { get; set; } = 320;
        public int DepthHeight { get; set; } = 240;
        [HideInInspector]
        public float DepthAverage;

        private bool isFullScreenRegion = false;
        public bool IsFullScreenRegion { 
            get => isFullScreenRegion;
            set
            {
                isFullScreenRegion = value;
                touchPointImage.gameObject.SetActive(!value);
            }
        } 
        private bool saving = false;
        private int nframesSaved = 0;
        private string directoryName;
        private string dataPath;
        private SynchronizationContext context;

        [System.Serializable]
        public class StringEvent : UnityEngine.Events.UnityEvent<string> { }

        public StringEvent ShowDialog;

        // point display variables

        private Vector2 screenPosition = new Vector2(0f, 0f);
        private Vector2 pixelPosition = new Vector2(0.5f, 0.5f);
        [SerializeField]
        private Text screenPosText = null;
        [SerializeField]
        private Text pixelPosText = null;
        [SerializeField]
        private Text depthAvText = null;
        [SerializeField]
        private RectTransform depthImage = null;
        [SerializeField]
        private RectTransform touchPointImage = null;


        private int invalidValue;

        // Use this for initialization
        void Start()
        {
            OnDepthStreamStarted(null, null, null, null);

            pixelPosition = new Vector2(DepthWidth / 2, DepthHeight / 2);

            TofArTofManager.OnStreamStarted += OnDepthStreamStarted;
            TofArTofManager.OnFrameArrived += Depth_FrameArrived;
            dataPath = Application.persistentDataPath + "/depthData";
            if (!System.IO.Directory.Exists(dataPath))
            {
                System.IO.Directory.CreateDirectory(dataPath);
            }
            context = SynchronizationContext.Current;

            DeviceCapabilityProperty deviceCapability = TofArManager.Instance.GetProperty<DeviceCapabilityProperty>();
            var cameraConfig = TofArTofManager.Instance.GetProperty<CameraConfigurationProperty>();

            bool useOlderVersion = true;


            var jsonValues = JsonUtility.FromJson<ParametersJsonData>(deviceCapability.TrimmedDeviceAttributesString);
            if (jsonValues != null)
            {
                var cameraSettings = jsonValues.cameraSettings;
                if (cameraSettings != null)
                {
                    useOlderVersion = false;
                    foreach (var setting in cameraSettings)
                    {
                        if (setting.id.Equals(cameraConfig.cameraId))
                        {
                            if (setting.useDepthInvalidValueConvert == "true")
                            {
                                invalidValue = int.Parse(setting.depthInvalidValueTo, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                invalidValue = int.Parse(setting.depthInvalidValueFrom, System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }

            }


            if (useOlderVersion)
            {
                var jsonValuesOld = JsonUtility.FromJson<ParametersJsonDataOld>(deviceCapability.TrimmedDeviceAttributesString);
                if (jsonValuesOld != null)
                {
                    if (jsonValuesOld.useDepthInvalidValueConvert == "true")
                    {
                        invalidValue = int.Parse(jsonValuesOld.depthInvalidValueTo, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        invalidValue = int.Parse(jsonValuesOld.depthInvalidValueFrom, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    invalidValue = 32001;
                }
            }
        }

        private void OnDestroy()
        {
            TofArTofManager.OnStreamStarted -= OnDepthStreamStarted;
            TofArTofManager.OnFrameArrived -= Depth_FrameArrived;
        }

        private void OnDepthStreamStarted(object sender, Texture2D depth, Texture2D conf, PointCloudData pc)
        {
            var config = TofArTofManager.Instance.GetProperty<CameraConfigurationProperty>();
            DepthWidth = config.width;
            DepthHeight = config.height;
        }

        private void SaveData(int rx, int ry, short[] depth, short[] confidence)
        {
            bool depthOk = depth.Length > 0;
            bool confidenceOk = confidence.Length > 0;

            //Debug.LogFormat("rx {0} ry {1} touchPoint {2} image {3},{4}", rx, ry, touchedPoint, DepthWidth, DepthHeight);
            var workingAreaDepth = new short[this.IsFullScreenRegion ? DepthWidth * DepthHeight : RegionWidth * RegionWidth];
            var workingAreaConfidence = new short[workingAreaDepth.Length];
            //can't buffer copy due to the striding
            var regionWidth = this.IsFullScreenRegion ? this.DepthWidth : this.RegionWidth;
            var regionHeight = this.IsFullScreenRegion ? this.DepthHeight : this.RegionWidth;
            for (int j = 0; j < regionHeight; j++)
            {
                for (int i = 0; i < regionWidth; i++)
                {
                    if (depthOk)
                    {
                        workingAreaDepth[j * regionWidth + i] = depth[(ry + j) * DepthWidth + i + rx];
                    }
                    if (confidenceOk)
                    {
                        workingAreaConfidence[j * regionWidth + i] = confidence[(ry + j) * DepthWidth + i + rx];
                    }
                }
            }
            long totalDepth = 0;
            int totalDepthPoints = 0;
            foreach (short s in workingAreaDepth)
            {
                if (s != invalidValue)
                {
                    totalDepthPoints++;
                    totalDepth += s;
                }
            }
            DepthAverage = totalDepthPoints == 0 ? 0 : ((float)totalDepth) / (totalDepthPoints); //set to 0 if every point is invalid
            //TofArManager.Logger.WriteLog(LogLevel.Debug, $"TotalDepth:{TotalDepth} workingArea.Length:{workingArea.Length} DepthAverage:{DepthAverage}");
            if (saving)
            {
                if (++nframesSaved > SaveFrames)
                {
                    saving = false;
                    nframesSaved = 0;
                }
                else
                {
                    string depthPath = dataPath + directoryName + "/" + nframesSaved + ".raw";
                    string confPath = dataPath + directoryName + "/" + nframesSaved + "_conf.raw";

                    context.Post((s) =>
                    {
                        var saveSuccess = this.SaveDepthData(workingAreaDepth, depthPath);
                        saveSuccess &= this.SaveConfidenceData(workingAreaConfidence, confPath);
                        if (saveSuccess)
                        {
                            if (nframesSaved == SaveFrames)
                            {
                                ShowDialog.Invoke("successfully saved data to " + depthPath);
                            }
                        }
                        else
                        {
                            ShowDialog.Invoke("failed to save data to " + depthPath);
                        }
                    }, null);
                }
            }
        }

        private void Depth_FrameArrived(object sender)
        {
            var depth = TofArTofManager.Instance?.DepthData?.Data;
            if (depth == null)
            {
                return;
            }
            
            var confidence = TofArTofManager.Instance?.ConfidenceData?.Data;
            if (confidence == null)
            {
                return;
            }
            
            int rx = this.IsFullScreenRegion ? 0 : (int)pixelPosition.x - RegionWidth / 2;
            int ry = this.IsFullScreenRegion ? 0 : (int)pixelPosition.y - RegionWidth / 2;
            if (rx < 0)
            {
                rx = 0;
            }

            if (ry < 0)
            {
                ry = 0;
            }

            if (rx + RegionWidth >= DepthWidth)
            {
                rx = DepthWidth - RegionWidth - 1;
            }

            if (ry + RegionWidth >= DepthHeight)
            {
                ry = DepthHeight - RegionWidth - 1;
            }

            SaveData(rx, ry, depth, confidence);
        }


        public void TouchImageInWorldSpace(Vector2 point)
        {
            MoveTouchPointImage(point);
            screenPosition = new Vector2((int)(point.x), (int)point.y);
        }

        public void TouchImageInScreenSpace(Vector2 point)
        {
            pixelPosition = new Vector2(DepthWidth * point.x, DepthHeight * (point.y));
        }

        private void MoveTouchPointImage(Vector2 point)
        {
            touchPointImage.position = new Vector3(point.x, point.y, 0);
        }
        private void Update()
        {
            try
            {
                if (screenPosition != null)
                {
                    screenPosText.text = string.Format("({0}, {1})", screenPosition.x, screenPosition.y);
                }
                if (pixelPosition != null)
                {
                    pixelPosText.text = string.Format("({0}, {1})", (int)pixelPosition.x, (int)pixelPosition.y);
                }
                if (depthAvText != null)
                {
                    depthAvText.text = string.Format("{0:f1}mm", DepthAverage);
                }
            }
            catch (InvalidOperationException e)
            {
                Debug.LogWarning(e);
            }
            FixScaling();
        }

        private void FixScaling()
        {
            ScaleTouchImage();
            Rect parentShape = depthImage.rect;
            Vector2 newScreenPosition;
            if (parentShape.width / parentShape.height > DepthHeight / DepthWidth)
            {
                newScreenPosition = (pixelPosition * parentShape.width) / DepthWidth;
            }
            else
            {
                newScreenPosition = (pixelPosition * parentShape.height) / DepthHeight;
            }
            newScreenPosition.y *= -1;
            touchPointImage.anchoredPosition = newScreenPosition;
        }

        public void ScaleTouchImage()
        {
            float scale;
            Rect parentShape = depthImage.rect;
            if (parentShape.width / parentShape.height > DepthHeight / DepthWidth)
            {
                scale = RegionWidth * parentShape.width / DepthWidth;
            }
            else
            {
                scale = RegionWidth * parentShape.height / DepthHeight;
            }
            scale = Mathf.Max(scale, 16);
            touchPointImage.sizeDelta = new Vector2(scale, scale);
        }



        public void SavePoint()
        {
            if (TofArTofManager.Instance.IsStreamActive)
            {
                if (!saving) //don't try to save until you finished the last one
                {
                    directoryName = "/Region" + (this.IsFullScreenRegion ? "Full" : RegionWidth.ToString()) + "_" + System.DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
                    saving = true;
                }
            }
        }


        bool SaveDepthData(short[] data, string rawPath)
        {
            bool saveSuccess = false;
            try
            {
                if (!System.IO.Directory.Exists(dataPath + directoryName))
                {
                    System.IO.Directory.CreateDirectory(dataPath + directoryName);
                }
                using (var depthFile = System.IO.File.Open(rawPath, System.IO.FileMode.Create))
                {
                    {
                        byte[] outarr = new byte[data.Length * 2];
                        System.Buffer.BlockCopy(data, 0, outarr, 0, outarr.Length);

                        depthFile.Write(outarr, 0, outarr.Length);
                    }
                }
                saveSuccess = true;
            }
            catch (System.IO.IOException)
            {
                saveSuccess = false;
            }
            return saveSuccess;
        }

        bool SaveConfidenceData(short[] data, string rawPath)
        {
            bool saveSuccess = false;
            try
            {
                if (!System.IO.Directory.Exists(dataPath + directoryName))
                {
                    System.IO.Directory.CreateDirectory(dataPath + directoryName);
                }
                using (var confidenceFile = System.IO.File.Open(rawPath, System.IO.FileMode.Create))
                {
                    {
                        byte[] outarr = new byte[data.Length * 2];
                        System.Buffer.BlockCopy(data, 0, outarr, 0, outarr.Length);

                        confidenceFile.Write(outarr, 0, outarr.Length);
                    }
                }
                saveSuccess = true;
            }
            catch (System.IO.IOException)
            {
                saveSuccess = false;
            }
            return saveSuccess;
        }

    }
}
