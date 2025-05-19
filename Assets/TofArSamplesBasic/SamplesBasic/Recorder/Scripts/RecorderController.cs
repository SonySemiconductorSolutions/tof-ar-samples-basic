/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2024 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using TofAr.V0;
using TofAr.V0.Hand;
using TofArSamples.Body;
using TofArSettings.Hand;
using UnityEngine;

namespace TofArSamples.Recorder
{
    /// <summary>
    /// RecorderController
    /// </summary>
    public class RecorderController : MonoBehaviour
    {
        private RecogModeController recogModeController = null;

        private void Start()
        {
            var bodyStreamOrder = FindObjectOfType<BodyStreamOrder>();
            if (bodyStreamOrder)
            {
                var distribution = TofArManager.Instance.RuntimeSettings.distribution;
                bodyStreamOrder.enableStartStream = (distribution == Distribution.Basic);
            }

            this.recogModeController = FindObjectOfType<RecogModeController>();
            if (this.recogModeController)
            {
                recogModeController.OnUpdateList += RecogModeController_OnUpdateList;
            }
        }

        private void RecogModeController_OnUpdateList(string[] list, int index)
        {
            this.recogModeController.OnUpdateList -= RecogModeController_OnUpdateList;
            this.StartCoroutine(this.SetHandRecogMode());
        }

        private IEnumerator SetHandRecogMode()
        {
            yield return null;

            var distribution = TofArManager.Instance.RuntimeSettings.distribution;
            for (var i = 0; i < recogModeController.ModeList.Length; i++)
            {
                if (recogModeController.ModeList[i] == ((distribution == Distribution.Basic) ?
                    RecogMode.Face2Face : RecogMode.HeadMount + 2))
                {
                    recogModeController.Index = i;
                    break;
                }
            }
            yield break;
        }
    }
}