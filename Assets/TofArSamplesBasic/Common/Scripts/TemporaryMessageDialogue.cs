/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSamples
{
    public class TemporaryMessageDialogue : MonoBehaviour
    {
        public float displayTime = 2;

        public Text displayText;

        public GameObject displayPanel;

        /// <summary>
        /// View TextSetting
        /// </summary>
        /// <param name="value"></param>
        public void DisplayMessage(string value)
        {
            displayText.text = value;
            StartCoroutine(Display());
        }

        private IEnumerator Display()
        {
            displayPanel.SetActive(true);
            yield return new WaitForSeconds(displayTime);
            displayPanel.SetActive(false);
        }
    }
}
