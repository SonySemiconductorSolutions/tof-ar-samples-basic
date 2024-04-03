/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TofAr.AppLicense
{
    public class AppLicenseManager : MonoBehaviour
    {
        private const string license_title = "License information";
        private const string eula_title = "End user license agreement";
        private const string pp_title = "Privacy policy";

        private const string license_fileName = "license";
        private const string eula_fileName = "eula";
        private const string pp_fileName = "pp";

        private const string agree_key = "AGREEKEY";

        private AppLicense appLicense;

        public GameObject agreeViewPrefab;

        public GameObject baseButtonPrefab;
        public GameObject borderPrefab;

        public GameObject scrollTextViewPrefab;

        public string agreeViewSubTitleAppName;

        private string licenseText;
        private string eulaText;
        private string ppText;

        void Awake()
        {
            SetCaller();
        }

        void Start()
        {
            //Debug : Reset Check
            //PlayerPrefs.DeleteAll();
            licenseText = eulaText = ppText = "";

            CheckAgreeView();
            ImportTextsFromFile();
            SetButtons();
        }

        public void SetCaller()
        {
            AppLicense[] appLicense = FindObjectsOfType<AppLicense>();

            if (appLicense.Length > 0)
            {
                this.appLicense = appLicense[0];
            }
        }

        private void CheckAgreeView()
        {
            if (PlayerPrefs.GetInt(agree_key, 0) == 0)
            {
                SettAgreeView();
            }
            else
            {
                appLicense.SetAgreeState(true);
            }
        }

        private void SettAgreeView()
        {
            if (appLicense.agreeViewParent != null)
            {
                GameObject agreeView = Instantiate(agreeViewPrefab, appLicense.agreeViewParent.transform);
                agreeView.GetComponent<AgreeView>().SetAboutApp(this);
            }
        }

        public void AgreeStartButton()
        {
            PlayerPrefs.SetInt(agree_key, 1);
            PlayerPrefs.Save();

            appLicense.SetAgreeState(true);
        }

        private void ImportTextsFromFile()
        {
            Debug.Log("importing text files");
            licenseText = TextImporter.MergeImport(license_fileName);
            eulaText = TextImporter.MergeImport(eula_fileName);
            ppText = TextImporter.MergeImport(pp_fileName);
        }

        private void SetButtons()
        {
            if (appLicense.buttonParent != null)
            {
                Instantiate(borderPrefab, appLicense.buttonParent.transform);

                GameObject licenseButton = Instantiate(baseButtonPrefab, appLicense.buttonParent.transform);
                SettingButton(licenseButton, license_title, LicenseView);

                GameObject eulaButton = Instantiate(baseButtonPrefab, appLicense.buttonParent.transform);
                SettingButton(eulaButton, eula_title, EULAView);

                GameObject ppButton = Instantiate(baseButtonPrefab, appLicense.buttonParent.transform);
                SettingButton(ppButton, pp_title, PPView);
            }
        }

        private void SettingButton(GameObject button, string title, UnityAction unityAction)
        {
            button.GetComponentInChildren<Text>().text = title;
            button.GetComponent<Button>().onClick.AddListener(unityAction);
        }

        public void LicenseView()
        {
            SetScrollTextView(license_title, licenseText);
        }

        public void EULAView()
        {
            SetScrollTextView(eula_title, eulaText);
        }

        public void PPView()
        {
            SetScrollTextView(pp_title, ppText);
        }

        private void SetScrollTextView(string title, string msg)
        {
            GameObject scrollTextView = Instantiate(scrollTextViewPrefab, appLicense.agreeViewParent.transform);
            scrollTextView.GetComponent<ScrollTextView>().SetText(title, msg);
        }
    }
}
