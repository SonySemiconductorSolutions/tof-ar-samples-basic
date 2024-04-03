/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TofAr.AppLicense
{
    public class AgreeView : MonoBehaviour
    {
        public Text subTitle;
        public GameObject safeArea;
        public Toggle l_Toggle;
        public Toggle p_Toggle;
        public Button startButton;

        private AppLicenseManager aboutApp;

        private DeviceOrientation postDeviceOrientation;

        void Start()
        {
            subTitle.text = $"Please try the { aboutApp.agreeViewSubTitleAppName} application.";

            l_Toggle.isOn = false;
            p_Toggle.isOn = false;
        }

        void Update()
        {
            CheckToggle();
            CheckSafeArea();
        }

        private void CheckToggle()
        {
            if (l_Toggle.isOn && p_Toggle.isOn)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
        }

        public void EULATextButton()
        {
            aboutApp.EULAView();
        }

        public void PPTextButton()
        {
            aboutApp.PPView();
        }

        public void SetAboutApp(AppLicenseManager aboutApp)
        {
            this.aboutApp = aboutApp;
        }

        public void StartButton()
        {
            aboutApp.AgreeStartButton();
            Destroy(this.gameObject);
        }

        private void CheckSafeArea()
        {
            if (Application.isEditor)
            {
                return;
            }

            if (Input.deviceOrientation != DeviceOrientation.Unknown && postDeviceOrientation == Input.deviceOrientation)
            {
                return;
            }

            postDeviceOrientation = Input.deviceOrientation;

            var rect = safeArea.GetComponent<RectTransform>();
            var area = Screen.safeArea;
            var resolition = Screen.currentResolution;

            rect.sizeDelta = Vector2.zero;
            rect.anchorMax = new Vector2(area.xMax / resolition.width, area.yMax / resolition.height);
            rect.anchorMin = new Vector2(area.xMin / resolition.width, area.yMin / resolition.height);
        }
    }
}
