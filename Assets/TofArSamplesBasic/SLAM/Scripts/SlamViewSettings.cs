/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.Slam
{
    public class SlamViewSettings : ImageViewSettings
    {
        SlamObjectController slamObjectController;

        UI.ItemToggle itemCube, itemPlane;

        protected override void Awake()
        {
            slamObjectController = FindObjectOfType<SlamObjectController>();
            base.Awake();
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();
            var list = new List<UnityAction>(uiOrder);
            list.Add(MakeUISlamObj);
            controllers.Add(slamObjectController);
            // Set UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make UI for Slam object
        /// </summary>
        void MakeUISlamObj()
        {
            itemCube = settings.AddItem("Cube", slamObjectController.IsCube,
                ChangeCube);

            slamObjectController.OnChangeCube += (onOff) =>
            {
                itemCube.OnOff = onOff;
            };

            itemPlane = settings.AddItem("Plane", slamObjectController.IsPlane,
                ChangePlane);

            slamObjectController.OnChangePlane += (onOff) =>
            {
                itemPlane.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle cube display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeCube(bool onOff)
        {
            slamObjectController.IsCube = onOff;
        }

        /// <summary>
        /// Toggle plane display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangePlane(bool onOff)
        {
            slamObjectController.IsPlane = onOff;
        }
    }
}
