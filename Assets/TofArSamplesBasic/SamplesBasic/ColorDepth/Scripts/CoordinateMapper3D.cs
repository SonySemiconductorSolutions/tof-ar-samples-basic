/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using TofAr.V0.Tof;
using TofAr.V0.Coordinate;
using System;
using TofAr.V0;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace TofArSamples
{
    public class CoordinateMapper3D : MonoBehaviour
    {
        private Renderer thisRenderer;

        private TofArCoordinateManager coordinateManager;
        private TofArTofManager tofManager;

        private Texture2D depthTexture;
        private short[] depthTextureBuffer = new short[0];
        private short[] depthBuffer = new short[0];

        private void Awake()
        {
            tofManager = TofArTofManager.Instance;
            coordinateManager = TofArCoordinateManager.Instance;
            depthTexture = new Texture2D(0, 0, TextureFormat.RGBA4444, false);
            depthTexture.filterMode = FilterMode.Point;
        }

        void OnEnable()
        {
            thisRenderer = this.GetComponent<Renderer>();
            TofArTofManager.OnStreamStarted += OnStreamStarted;
            
            this.StartCoroutine(this.Process());
            
        }

        void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnStreamStarted;

            StopCoroutine(Process());
        }

        private IEnumerator Process()
        {
            //process is called once before start is called so we have to wait
            yield return new WaitForEndOfFrame();
            while (isActiveAndEnabled)
            {

                ProcessCoordinate();

                yield return null;
            }
        }

        private void OnStreamStarted(object sender, Texture2D depthTexture, Texture2D confidenceTexture, PointCloudData pointCloudData)
        {
            if (thisRenderer != null)
            {
                //this.StartCoroutine(this.SetTextureAndMaterial());
            }
        }

        private bool MapToDepth(DepthPointProperty[] depthPoints, int width, int height, int depthWidth)
        {
            try
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int colorIndex = (y * width) + x;

                        if (depthPoints == null)
                        {
                            continue;
                        }

                        var depthPoint = depthPoints[colorIndex];

                        if (depthPoint.x == -1 || depthPoint.y == -1)
                        {
                            depthTextureBuffer[colorIndex] = 32001;
                        }
                        else
                        {
                            var depthIndex = depthPoint.x + depthPoint.y * depthWidth;

                            if (depthBuffer == null || depthIndex >= depthBuffer.Length)
                            {
                                depthTextureBuffer[colorIndex] = 32001;
                            }
                            else
                            {
                                depthTextureBuffer[colorIndex] = depthBuffer[depthIndex];
                            }
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, string.Format("{0} : {1}\n{2}", e.GetType().Name, e.Message, e.StackTrace));
                return false;
            }

            return true;
        }

        private void ProcessCoordinate()
        {
            if (tofManager == null || coordinateManager == null)
            {
                return;
            }
            if (!tofManager.IsStreamActive && !tofManager.IsPlaying)
            {
                return;
            }
            if (tofManager.DepthData == null || tofManager.DepthData.Data == null || tofManager.CalibrationSettings == null)
            {
                return;
            }

            if (!thisRenderer.enabled)
            {
                return;
            }

            int depthPointCount = tofManager.DepthData.Data.Length;

            if (depthPointCount != depthBuffer.Length)
            {
                Array.Resize(ref depthBuffer, depthPointCount);
            }
            Buffer.BlockCopy(tofManager.DepthData.Data, 0, depthBuffer, 0, depthPointCount * 2);

            var colorToDepthConfig = new ColorToDepthProperty
            {
                depthFrame = depthBuffer
            };

            var settingsProperty = tofManager.CalibrationSettings;
            var height = settingsProperty.colorHeight;
            var width = settingsProperty.colorWidth;
            var depthWidth = settingsProperty.depthWidth;
            var depthHeight = settingsProperty.depthHeight;

            if (tofManager.DepthData.Data.Length != depthWidth * depthHeight)
            {
                return;
            }
            var colorToDepth = coordinateManager.GetProperty<ColorToDepthProperty>(colorToDepthConfig);
            if (colorToDepth == null || colorToDepth.depthPoints == null)
            {
                return;
            }
            DepthPointProperty[] depthPoints = (DepthPointProperty[])colorToDepth.depthPoints.Clone();

            if (depthTextureBuffer.Length != width * height)
            {
                Array.Resize(ref depthTextureBuffer, width * height);
            }

            if (!MapToDepth(depthPoints, width, height, depthWidth))
            {
                return;
            }

            if (depthTextureBuffer.Length == width * height)
            {
                thisRenderer.material.mainTexture = ShortToTexture2D(depthTextureBuffer, width, height);
            }
        }

        private Texture2D ShortToTexture2D(short[] tex, int width, int height)
        {
            if (depthTexture.width != width || depthTexture.height != height)
            {
#if UNITY_2021_1_OR_NEWER
                depthTexture.Reinitialize(width, height);
#else
                depthTexture.Resize(width, height);
#endif
            }
            GCHandle handle = GCHandle.Alloc(tex, GCHandleType.Pinned);
            try
            {
                depthTexture.LoadRawTextureData(handle.AddrOfPinnedObject(), width * height * sizeof(short));
                depthTexture.Apply();
            }
            catch (UnityException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, string.Format("{0} : {1}\n{2}", e.GetType().Name, e.Message, e.StackTrace));
            }
            finally
            {
                handle.Free();
            }
            return depthTexture;
        }
    }
}
