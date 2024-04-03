/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofArSamples.Hand;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.MarkRecog
{
    public class MarkRecogViewSettings : HandViewSettings
    {
        MarkRendererController rendererController;
        UI.ItemDropdown itemRendererDropDown;
        UI.ItemToggle itemMarkDisplayToggle;

        protected override void PrepareUI()
        {
            base.PrepareUI();
            var uiList = new List<UnityAction>(uiOrder);
            uiList.Add(MakeUIRenderer);
            uiList.Add(MakeUIMarkDisplay);
            uiOrder = uiList.ToArray();
            rendererController = FindObjectOfType<MarkRendererController>();
            controllers.Add(rendererController);
        }

        /// <summary>
        /// Make Mesh Material UI
        /// </summary>
        void MakeUIRenderer()
        {
            itemRendererDropDown = settings.AddItem("Mark\nRenderer", rendererController.RendererNames,
                rendererController.Index, ChangeMeshMaterial, 0, 0, 330);

            rendererController.OnChangeIndex += (index) =>
            {
                itemRendererDropDown.Index = index;
            };
        }

        /// <summary>
        /// Toggle Material of Mesh
        /// </summary>
        /// <param name="index">Show/Hide</param>
        void ChangeMeshMaterial(int index)
        {
            rendererController.Index = index;
        }

        void MakeUIMarkDisplay()
        {
            itemMarkDisplayToggle = settings.AddItem("Mark Display", rendererController.MarkDisplayVisible, ToggleMarkDisplay);
            rendererController.OnMarkDisplayChanged += (onOff) =>
             {
                 itemMarkDisplayToggle.OnOff = onOff;
             };
        }

        void ToggleMarkDisplay(bool onOff)
        {
            rendererController.MarkDisplayVisible = onOff;
        }
    }
}
