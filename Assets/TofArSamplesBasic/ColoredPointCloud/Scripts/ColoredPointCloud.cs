/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0.Color;
using TofAr.V0.Tof;
using TofAr.V0;
using TofAr.V0.Coordinate;
using Unity.Collections;
using System.Threading;


namespace TofArSamples.ColoredPointCloud
{
    [RequireComponent(typeof(PointCloudMeshVisualizer))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ColoredPointCloud : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private UnityEngine.Mesh mesh;

        private DepthToColorProperty depthToColor = new DepthToColorProperty();

        private FormatConvertProperty colorFormat = new FormatConvertProperty();

        private Camera2ConfigurationProperty depthConfig = new Camera2ConfigurationProperty();

        private NativeArray<byte> textureBytes;

        [SerializeField]
        private Material yuvTextureMaterial, rgbTextureMaterial;


        private SynchronizationContext context;
        private object processLock = new object();
        private object meshLock = new object();

        private Vector2[] meshUVs;

        void Awake()
        {
            context = SynchronizationContext.Current;
            meshRenderer = GetComponent<MeshRenderer>();
            mesh = GetComponent<MeshFilter>().mesh;
        }

        private void OnEnable()
        {
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArTofManager.OnFrameArrived += OnTofFrameArrived;
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
        }

        private void OnDisable()
        {
            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArTofManager.OnFrameArrived -= OnTofFrameArrived;
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
        }

        private void OnTofFrameArrived(object sender)
        {
            lock (processLock)
            {
                //apply the color frame to the point cloud mesh
                depthToColor.depthFrame = TofArTofManager.Instance.DepthData.Data;
                depthToColor = TofArCoordinateManager.Instance.GetProperty<DepthToColorProperty>(depthToColor);
            }
        }

        private void Update()
        {
            if ((TofArColorManager.Instance.IsStreamActive || TofArColorManager.Instance.IsPlaying) && (TofArTofManager.Instance.IsStreamActive || TofArTofManager.Instance.IsPlaying))
            {
                SetMeshRenderer(true);

                lock (processLock)
                {
                    //apply the color frame to the point cloud mesh
                    if (TofArTofManager.Instance.DepthData != null)
                    {
                        depthToColor.depthFrame = TofArTofManager.Instance.DepthData.Data;
                        if (depthConfig.width * depthConfig.height == depthToColor.depthFrame.Length)
                        {
                            depthToColor = TofArCoordinateManager.Instance.GetProperty<DepthToColorProperty>(depthToColor);
                            if (depthToColor == null)
                            {
                                depthToColor = new DepthToColorProperty();
                                return;
                            }
                        }
                        
                    }
                }
                lock (meshLock)
                {
                    int depthWidth = depthConfig.width;
                    int depthHeight = depthConfig.height;
                    int colorwidth = TofArColorManager.Instance.CurrentYWidth;
                    int colorheight = TofArColorManager.Instance.YHeight;
                    var colorPoints = depthToColor.colorPoints;
                    if (colorPoints.Length == depthWidth * depthHeight)
                    {
                        for (int y = 0; y < depthHeight; y++)
                        {
                            for (int x = 0; x < depthWidth; x++)
                            {
                                int depthIndex = (y * depthWidth + x);
                                if (depthIndex >= depthWidth * depthHeight)
                                {
                                    continue;
                                }

                                if (colorPoints[depthIndex].x >= 0 && colorPoints[depthIndex].y >= 0 && colorPoints[depthIndex].x < colorwidth && colorPoints[depthIndex].y < colorheight)
                                {
                                    meshUVs[depthIndex] = (new Vector2((float)colorPoints[depthIndex].x / colorwidth, (float)colorPoints[depthIndex].y / colorheight));
                                }
                            }
                        }
                    }
                    
                    if (meshUVs.Length == mesh.vertexCount)
                    {
                        mesh.SetUVs(0, meshUVs);
                    }
                }
            }
            else
            {
                SetMeshRenderer(false);
            }
        }

        private void SetMaterials()
        {
            switch (colorFormat.format)
            {
                case ColorFormat.YUV420:
                    yuvTextureMaterial.SetTexture("_YTex", TofArColorManager.Instance.YTexture);
                    yuvTextureMaterial.SetTexture("_UVTex", TofArColorManager.Instance.UVTexture);
                    meshRenderer.material = yuvTextureMaterial;
                    break;
                case ColorFormat.RGB:
                case ColorFormat.BGRA:
                case ColorFormat.RGBA:
                    rgbTextureMaterial.mainTexture = TofArColorManager.Instance.ColorTexture;
                    meshRenderer.material = rgbTextureMaterial;
                    break;
            }
        }

        private void SetMeshRenderer(bool state)
        {
            if (meshRenderer.enabled != state)
            {
                meshRenderer.enabled = state;
            }
        }

        private void OnColorStreamStarted(object sender, Texture colorTex)
        {
            lock (processLock)
            {
                colorFormat = TofArColorManager.Instance.GetProperty<FormatConvertProperty>();
                SetMaterials();
            }
        }

        private void OnTofStreamStarted(object sender, Texture depht, Texture conf, PointCloudData pointcloud)
        {
            lock (processLock)
            {
                depthConfig = TofArTofManager.Instance.GetProperty<Camera2ConfigurationProperty>();
                meshUVs = new Vector2[depthConfig.width * depthConfig.height];
            }
        }

    }
}
