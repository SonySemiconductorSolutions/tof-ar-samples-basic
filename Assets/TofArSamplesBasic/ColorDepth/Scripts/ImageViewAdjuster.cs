/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using TofArSamples.Color;
using TofArSamples.Tof;
using TofArSettings;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples.BasicStream
{
    public class ImageViewAdjuster : MonoBehaviour
    {
        public RectTransform ImageViewArea;

        TofArSettings.UI.CanvasScaleController canvasScCtrl;
        ScreenRotateController scRotCtrl;
        TofArSettings.UI.Toolbar toolbar;

        ColorRawImageFitter[] colorViewFitters;
        DepthRawImageFitter[] depthViewFitters;

        GridLayoutGroup[] grids;
        RectTransform[] gridRts;
        Vector2 viewAreaSize = Vector2.zero;
        int splitCountParent;
        int[] splitCountChild;

        void Awake()
        {
            canvasScCtrl = FindObjectOfType<TofArSettings.UI.CanvasScaleController>();
            scRotCtrl = canvasScCtrl.GetComponent<ScreenRotateController>();
            toolbar = FindObjectOfType<TofArSettings.UI.Toolbar>();

            colorViewFitters = GetComponentsInChildren<ColorRawImageFitter>();
            depthViewFitters = GetComponentsInChildren<DepthRawImageFitter>();

            grids = ImageViewArea.GetComponentsInChildren<GridLayoutGroup>();
            gridRts = new RectTransform[grids.Length];
            for (int i = 0; i < grids.Length; i++)
            {
                gridRts[i] = grids[i].GetComponent<RectTransform>();
            }

            splitCountParent = grids.Length;
            splitCountChild = new int[grids.Length];
        }

        void OnEnable()
        {
            canvasScCtrl.OnChangeSafeArea += UpdateViewAreaSize;
            scRotCtrl.OnRotateScreen += OnRotateScreen;

            UpdateViewAreaSize(canvasScCtrl.SafeAreaSize);
        }

        void OnDisable()
        {
            canvasScCtrl.OnChangeSafeArea -= UpdateViewAreaSize;
            scRotCtrl.OnRotateScreen -= OnRotateScreen;
        }

        /// <summary>
        /// Toggle display of View
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="onOff">On/Off</param>
        public void ShowView(string viewName, bool onOff)
        {
            GameObject viewObj = null;
            for (int i = 0; i < grids.Length; i++)
            {
                var grid = grids[i];
                for (int j = 0; j < grid.transform.childCount; j++)
                {
                    var tr = grid.transform.GetChild(j);
                    if (tr.name.Contains(viewName))
                    {
                        viewObj = tr.gameObject;
                        break;
                    }
                }

                if (viewObj)
                {
                    break;
                }
            }

            if (viewObj != null && viewObj.activeSelf != onOff)
            {
                viewObj.SetActive(onOff);
            }

            CountSplit();
            Adjust();
        }

        /// <summary>
        /// Event that is called when SafeArea size is changed
        /// </summary>
        /// <param name="safeAreaSize">SafeArea size</param>
        void UpdateViewAreaSize(Vector2 safeAreaSize)
        {
            viewAreaSize = safeAreaSize + ImageViewArea.offsetMin +
                ImageViewArea.offsetMax;
            CountSplit();
            Adjust();
        }

        void OnRotateScreen(ScreenOrientation ori)
        {
            Adjust();
        }

        /// <summary>
        /// Count the number of splits
        /// </summary>
        void CountSplit()
        {
            splitCountParent = grids.Length;
            if (splitCountChild.Length != splitCountParent)
            {
                Array.Resize(ref splitCountChild, splitCountParent);
            }

            for (int i = 0; i < gridRts.Length; i++)
            {
                var rt = gridRts[i];
                int activeCount = 0;
                for (int j = 0; j < rt.childCount; j++)
                {
                    var tr = rt.GetChild(j);
                    if (tr.gameObject.activeSelf)
                    {
                        activeCount++;
                    }
                }

                splitCountChild[i] = activeCount;
                if (activeCount <= 0)
                {
                    splitCountParent--;
                }
            }
        }

        /// <summary>
        /// Adjust the size and position of the UI
        /// </summary>
        void Adjust()
        {
            if (splitCountParent <= 0)
            {
                return;
            }

            bool isPortrait = scRotCtrl.IsPortrait;

            // Calculate the size of the display area (subtract the height of the toolbar and divide vertically/horizontally)
            var viewSize = viewAreaSize;
            if (isPortrait)
            {
                viewSize.y -= toolbar.BarWidth;
                viewSize.x /= splitCountParent;
            }
            else
            {
                viewSize.x -= toolbar.BarWidth;
                viewSize.y /= splitCountParent;
            }

            // Set the position and size of View
            int skipCount = 0;
            for (int i = 0; i < grids.Length; i++)
            {
                int splitCount = splitCountChild[i];
                if (splitCount <= 0)
                {
                    skipCount++;
                    continue;
                }

                var grid = grids[i];
                var rt = gridRts[i];
                var cellSize = viewSize;
                var pos = Vector2.zero;

                // Calculate the size of each View (divide vertically/horizontally by the number of Views to be displayed)
                if (isPortrait)
                {
                    cellSize.y = viewSize.y / splitCount;
                    pos.x = viewSize.x * (i - skipCount);
                }
                else
                {
                    cellSize.x = viewSize.x / splitCount;
                    pos.y = -viewSize.y * (i - skipCount);
                }

                rt.anchoredPosition = pos;
                rt.sizeDelta = viewSize;
                grid.cellSize = cellSize;
            }

            StartCoroutine(AdjustViews());
        }

        /// <summary>
        /// Adjust View of child
        /// </summary>
        IEnumerator AdjustViews()
        {
            // Adjust child after size of parent has been changed
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < colorViewFitters.Length; i++)
            {
                colorViewFitters[i].Adjust();
            }

            for (int i = 0; i < depthViewFitters.Length; i++)
            {
                depthViewFitters[i].Adjust();
            }
        }
    }
}
