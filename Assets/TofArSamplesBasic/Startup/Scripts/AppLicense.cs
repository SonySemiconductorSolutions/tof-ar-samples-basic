/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TofAr.AppLicense
{
    public class AppLicense : MonoBehaviour
    {
        private const string loadPrefabName = "AppLicenseManager";

        public GameObject agreeViewParent;
        public GameObject buttonParent;

        public GridLayoutGroup gridLayoutGroup;
        public VerticalLayoutGroup verticalLayoutGroup;

        private bool agreeState;

        void Awake()
        {
            CheckLoadLicenseManager();
        }

        private void CheckLoadLicenseManager()
        {
            GameObject appLicenseManagerPrefab = (GameObject)Resources.Load(loadPrefabName);

            if (appLicenseManagerPrefab != null)
            {
                GameObject appLicenseManager = GameObject.Instantiate(appLicenseManagerPrefab);
                SettingInfoView();
            }
            else
            {
                SetAgreeState(true);
            }
        }

        private void SettingInfoView()
        {
            gridLayoutGroup.cellSize = new Vector2(gridLayoutGroup.cellSize.x, 430);
            verticalLayoutGroup.spacing = 10;
        }

        public bool GetAgreeState()
        {
            return this.agreeState;
        }

        public void SetAgreeState(bool state)
        {
            this.agreeState = state;
        }
    }
}

