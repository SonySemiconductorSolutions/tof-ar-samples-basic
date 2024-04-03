/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TofArSamples.Tof
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class ScreenAdjustedGridLayoutGroup : MonoBehaviour
    {
        GridLayoutGroup gridLayout;
        RectTransform rect;

        private void Awake()
        {
            this.gridLayout = GetComponent<GridLayoutGroup>();
            this.rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            bool isLandscape = rect.rect.width > rect.rect.height;
            int nChildren = gridLayout.transform.childCount;
            if(isLandscape)
            {
                this.gridLayout.cellSize = new Vector2(rect.rect.width / nChildren, rect.rect.height);
            }
            else
            {
                this.gridLayout.cellSize = new Vector2(rect.rect.width, rect.rect.height / nChildren);
            }
        }
    }
}
