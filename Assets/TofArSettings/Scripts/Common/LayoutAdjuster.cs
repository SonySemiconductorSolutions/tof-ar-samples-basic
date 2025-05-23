/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class LayoutAdjuster : MonoBehaviour
    {
        HorizontalOrVerticalLayoutGroup layout;
        int bottom;

        ScreenRotateController scRotCtrl;
        Toolbar toolbar;

        void Awake()
        {
            layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
            bottom = layout.padding.bottom;

            scRotCtrl = FindAnyObjectByType<ScreenRotateController>();
            scRotCtrl.OnRotateScreen += OnRotateScreen;
            toolbar = FindAnyObjectByType<Toolbar>();
        }

        /// <summary>
        /// Event that is called when screen is rotated
        /// </summary>
        /// <param name="ori">Screen orientation</param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            layout.padding.bottom = (scRotCtrl.IsPortraitScreen) ?
                bottom + Mathf.RoundToInt(toolbar.BarWidth) : bottom;
        }
    }
}
