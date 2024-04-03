/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using TofArSettings.Color;
using UnityEngine;

namespace TofArSamples.Recorder
{
    public class MessagePanel : MonoBehaviour
    {
        private ColorFormatController formatCtrl;

        public GameObject colorFormatAlert;

        private void OnEnable()
        {
            var mgrCtrl = FindObjectOfType<ColorManagerController>();
            formatCtrl = mgrCtrl.GetComponent<ColorFormatController>();
            formatCtrl.OnChange += ChangeFormat;
        }

        private void OnDisable()
        {
            formatCtrl.OnChange -= ChangeFormat;
        }

        void ChangeFormat(int index)
        {
            if (formatCtrl.Format != ColorFormat.RGB)
            {
                colorFormatAlert.SetActive(true);
            }
            else
            {
                colorFormatAlert.SetActive(false);
            }
        }

    }
}
