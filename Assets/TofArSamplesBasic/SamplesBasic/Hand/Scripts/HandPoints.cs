/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;
using UI = TofArSettings.UI;

namespace TofArSamples.Hand
{
    public class HandPoints : MonoBehaviour
    {
        [SerializeField]
        HandStatus lrHand = HandStatus.RightHand;

        UI.Info uiInfoLocal, uiInfoWorld;

        HandModelController[] ctrls;

        void Start()
        {
            // Get UI
            foreach (var ui in GetComponentsInChildren<UI.Info>())
            {
                if (ui.name.Contains("Local"))
                {
                    uiInfoLocal = ui;
                }
                else if (ui.name.Contains("World"))
                {
                    uiInfoWorld = ui;
                }
            }

            // Add hand orientation information to title
            string leftOrRight = (lrHand == HandStatus.LeftHand) ? "Left" : "Right";
            uiInfoLocal.TitleText = $"{leftOrRight} Hand {uiInfoLocal.TitleText}";
            uiInfoWorld.TitleText = $"{leftOrRight} Hand {uiInfoWorld.TitleText}";

            ctrls = FindObjectsOfType<HandModelController>();
        }

        void Update()
        {
            // Use information of the displayed HandModel
            HandModelController ctrl = null;
            for (int i = 0; i < ctrls.Length; i++)
            {
                var c = ctrls[i];
                if (c.IsShow)
                {
                    ctrl = c;
                    break;
                }
            }

            // Show
            string left = "Please show any hand model.";
            string right = left;
            if (ctrl)
            {
                left = ctrl.GetPointsDataText(lrHand, true);
                right = ctrl.GetPointsDataText(lrHand, false);
            }

            uiInfoLocal.InfoText = left;
            uiInfoWorld.InfoText = right;
        }
    }
}
