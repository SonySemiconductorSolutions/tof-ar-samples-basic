/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Tof;
using TofAr.V0.Coordinate;
using TofAr.V0.Segmentation;
using UnityEngine.Rendering;
using TofAr.V0;
using TofAr.V0.Color;
using System.Runtime.InteropServices;

namespace TofArSamples.HumanPointCloud
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HumanPointCloud : MonoBehaviour
    {
        [SerializeField]
        private byte threshold = 128;
        public byte Threshold
        {
            get => threshold;
            set => threshold = value;
        }

        [SerializeField]
        private bool segmentHuman = true;
        public bool SegmentHuman
        {
            get => segmentHuman;
            set => segmentHuman = value;
        }

        [SerializeField]
        private bool colorDisplay = true;
        public bool ColorDisplay
        {
            get => colorDisplay;
            set
            {
                colorDisplay = value;
                SetColorDisplayMaterials();
            }
        }


        [SerializeField]
        private Material solidColorMaterial, yuvTextureMaterial, rgbTextureMaterial;


        private UnityEngine.Mesh mesh;

        private int latestPointNum = 0;
        private object meshLock = new object();
        private int[] indices = new int[0];
        private bool updated = false;
        private DepthToColorProperty depthToColor = new DepthToColorProperty();
        private object processLock = new object();
        private Camera2ConfigurationProperty depthConfig;
        private ResolutionProperty colorConfig;
        private byte[] segmentationBytes;
        private ColorPointProperty[] colorPoints;
        private Vector3[] pointCloud;
        private int segWidth = TofAr.V0.Segmentation.Human.HumanDetector.Height;
        private int segHeight = TofAr.V0.Segmentation.Human.HumanDetector.Width;
        List<Vector3> humanPoints = new List<Vector3>();
        List<Vector2> uvPoints = new List<Vector2>();
        private MeshRenderer meshRenderer;
        private FormatConvertProperty colorFormat = new FormatConvertProperty();

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.Clear();
        }

        void OnEnable()
        {
            TofArTofManager.OnFrameArrived += OnTofFrameArrived;
            TofArTofManager.OnStreamStarted += OnTofStreamStarted;
            TofArSegmentationManager.OnFrameArrived += OnSegmentationFrameArrived;
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;
        }
        private void OnDisable()
        {
            TofArTofManager.OnFrameArrived -= OnTofFrameArrived;
            TofArTofManager.OnStreamStarted -= OnTofStreamStarted;
            TofArSegmentationManager.OnFrameArrived -= OnSegmentationFrameArrived;
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
        }

        private void SetColorDisplayMaterials()
        {
            if (colorDisplay)
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
            else
            {
                meshRenderer.material = solidColorMaterial;
            }
        }

        private void RenderSegmentedPointCloud(int colorWidth, int colorHeight, int depthWidth, int depthHeight)
        {
            humanPoints.Clear();


            float segRatio = (float)segWidth / (float)segHeight;
            float imgRatio = (float)colorWidth / (float)colorHeight;

            float segHeightScale = 1f;
            float segWidthScale = 1f;
            int vOffset = 0;
            int uOffset = 0;

            // add offset and scale to match segmentation ratio
            if (segRatio > imgRatio)
            {
                int colorHeightAdjusted = (int)(colorWidth / segRatio);
                vOffset = (colorHeightAdjusted - colorHeight) / 2;

                segHeightScale = (float)colorHeight / (float)colorHeightAdjusted;
            }
            else if (segRatio < imgRatio)
            {
                int colorWidthAdjusted = (int)(colorHeight * segRatio);
                uOffset = (colorWidthAdjusted - colorWidth) / 2;

                segWidthScale = (float)colorWidth / (float)colorWidthAdjusted;
            }

            float heightRatio = (float)(segHeight * segHeightScale) / (float)colorHeight;
            float widthRatio = (float)(segWidth * segWidthScale) / (float)colorWidth;

            for (int y = 0; y < depthHeight; y++)
            {
                for (int x = 0; x < depthWidth; x++)
                {
                    int depthIndex = (y * depthWidth + x);
                    if (depthIndex >= pointCloud.Length)
                    {
                        continue;
                    }

                    int u = colorPoints[depthIndex].x + uOffset;
                    int v = colorPoints[depthIndex].y + vOffset;

                    int colorIndex = (int)(v * heightRatio) * segWidth + (int)(u * widthRatio);
                    if (colorIndex >= 0 && colorIndex < segmentationBytes.Length && segmentationBytes[colorIndex] >= threshold)
                    {
                        humanPoints.Add(pointCloud[depthIndex]);
                        uvPoints.Add(new Vector2((float)colorPoints[depthIndex].x / colorWidth, (float)colorPoints[depthIndex].y / colorHeight));
                    }
                }
            }
        }

        private void RenderNormalPointCloud(int colorWidth, int colorHeight, int depthWidth, int depthHeight)
        {
            if (colorDisplay)
            {
                humanPoints.Clear();
                for (int y = 0; y < depthHeight; y++)
                {
                    for (int x = 0; x < depthWidth; x++)
                    {
                        int depthIndex = (y * depthWidth + x);
                        if (depthIndex >= pointCloud.Length)
                        {
                            continue;
                        }

                        if (colorPoints[depthIndex].x >= 0 && colorPoints[depthIndex].y >= 0 && colorPoints[depthIndex].x < colorWidth && colorPoints[depthIndex].y < colorHeight)
                        {
                            humanPoints.Add(pointCloud[depthIndex]);
                            uvPoints.Add(new Vector2((float)colorPoints[depthIndex].x / colorWidth, (float)colorPoints[depthIndex].y / colorHeight));
                        }
                    }
                }
            }
            else
            {
                humanPoints = new List<Vector3>(pointCloud);
            }
        }

        private void Update()
        {
            if (depthConfig == null || colorConfig == null)
            {
                return;
            }
            if ((TofArColorManager.Instance.IsStreamActive || TofArColorManager.Instance.IsPlaying) && (TofArTofManager.Instance.IsStreamActive || TofArTofManager.Instance.IsPlaying))
            {
                SetMeshRenderer(true);

                if (updated)
                {
                    if (segmentHuman || colorDisplay)
                    {
                        lock (processLock)
                        {
                            if (TofArTofManager.Instance.DepthData != null)
                            {
                                //apply the color frame to the point cloud mesh
                                depthToColor.depthFrame = TofArTofManager.Instance.DepthData.Data;
                                if (depthConfig.width * depthConfig.height == depthToColor.depthFrame.Length)
                                {
                                    depthToColor = TofArCoordinateManager.Instance.GetProperty<DepthToColorProperty>(depthToColor);
                                }
                            }
                            
                            if (depthToColor == null)
                            {
                                depthToColor = new DepthToColorProperty();
                                return;
                            }
                            if (colorPoints == null || colorPoints.Length != depthToColor.colorPoints.Length)
                            {
                                colorPoints = new ColorPointProperty[depthToColor.colorPoints.Length];
                            }
                            System.Array.Copy(depthToColor.colorPoints, colorPoints, depthToColor.colorPoints.Length);
                        }
                    }
                    lock (meshLock)
                    {
                        updated = false;
                        int depthWidth = depthConfig.width;
                        int depthHeight = depthConfig.height;
                        int colorwidth = TofArColorManager.Instance.CurrentYWidth;
                        int colorheight = TofArColorManager.Instance.YHeight;
                        uvPoints.Clear();
                        if (pointCloud != null)
                        {
                            if (segmentHuman && segmentationBytes != null)
                            {
                                RenderSegmentedPointCloud(colorwidth, colorheight, depthWidth, depthHeight);
                            }
                            else
                            {
                                RenderNormalPointCloud(colorwidth, colorheight, depthWidth, depthHeight);
                            }
                        }
                    }

                    UpdateMesh();
                }
            }
            else
            {
                SetMeshRenderer(false);
            }
        }

        private void UpdateMesh()
        {
            if (humanPoints.Count > latestPointNum)
            {
                System.Array.Resize(ref indices, humanPoints.Count);
                for (var i = latestPointNum; i < humanPoints.Count; i++)
                {
                    indices[i] = i;
                }
            }

            if (humanPoints.Count < latestPointNum)
            {
                for (int i = humanPoints.Count; i < latestPointNum; i++)
                {
                    humanPoints.Add(Vector3.zero);
                    uvPoints.Add(Vector2.zero);
                }
            }

            latestPointNum = humanPoints.Count;

            mesh.SetVertices(humanPoints);
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            if (colorDisplay)
            {
                mesh.SetUVs(0, uvPoints);
            }
        }

        private void SetMeshRenderer(bool state)
        {
            if (meshRenderer.enabled != state)
            {
                meshRenderer.enabled = state;
            }
        }

        private void OnSegmentationFrameArrived(object sender)
        {
            lock (meshLock)
            {
                foreach (var humanSegmentation in TofArSegmentationManager.Instance.SegmentationData.Data.results)
                {
                    if (humanSegmentation.name == TofAr.V0.Segmentation.Human.HumanSegmentationDetector.ResultNameMask)
                    {
                        switch (humanSegmentation.dataStructureType)
                        {
                            case DataStructureType.MaskBufferByte:
                                segmentationBytes = humanSegmentation.maskBufferByte;
                                break;
                            case DataStructureType.RawPointer:
                                if (segmentationBytes == null || segmentationBytes.Length != humanSegmentation.maskBufferSize)
                                {
                                    segmentationBytes = new byte[humanSegmentation.maskBufferSize];
                                }
                                Marshal.Copy((System.IntPtr)humanSegmentation.rawPointer, segmentationBytes, 0, (int)humanSegmentation.maskBufferSize);
                                break;
                            default:
                                break;
                        }
                        segWidth = humanSegmentation.maskBufferWidth;
                        segHeight = humanSegmentation.maskBufferHeight;
                        break;
                    }
                }
            }
        }

        private void OnTofFrameArrived(object stream)
        {
            if (!TofArTofManager.Instantiated)
            {
                return;
            }

            var pointCloudData = TofArTofManager.Instance.PointCloudData;
            if (pointCloudData == null || pointCloudData.Points == null)
            {
                return;
            }
            lock (meshLock)
            {
                if(this.pointCloud == null || this.pointCloud.Length != pointCloudData.Points.Length)
                {
                    this.pointCloud = new Vector3[pointCloudData.Points.Length];
                }
                System.Array.Copy(pointCloudData.Points, pointCloud, pointCloudData.Points.Length);
                updated = true;
            }
        }

        private void OnTofStreamStarted(object sender, Texture depht, Texture conf, PointCloudData pointcloud)
        {
            lock (processLock)
            {
                depthConfig = TofArTofManager.Instance.GetProperty<Camera2ConfigurationProperty>();
            }
        }

        private void OnColorStreamStarted(object sender, Texture colorTex)
        {
            lock (processLock)
            {
                colorFormat = TofArColorManager.Instance.GetProperty<FormatConvertProperty>();
                SetColorDisplayMaterials();
                colorConfig = TofArColorManager.Instance.GetProperty<ResolutionProperty>();
            }
        }
    }
}

