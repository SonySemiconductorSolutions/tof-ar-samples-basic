/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofArSamples.Color;
using TofArSamples.Tof;
using TofArSettings;
using UnityEngine;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples
{
    public class ImageViewSettings : UI.SettingsBase
    {
        [Header("Use Component")]

        /// <summary>
        /// Use/do not use color component
        /// </summary>
        [SerializeField]
        bool color = true;

        /// <summary>
        /// Use/do not use Tof component
        /// </summary>
        [SerializeField]
        bool tof = true;

        ColorViewController colorViewCtrl;
        DepthViewController depthViewCtrl;

        UI.ItemDropdown itemModeColor, itemAnchorColor,
            itemModeDepth, itemAnchorDepth;

        protected virtual void Awake()
        {
            colorViewCtrl = GetComponent<ColorViewController>();
            if (colorViewCtrl != null)
            {
                colorViewCtrl.enabled = color;
            }
            else
            {
                color = false;
            }
            depthViewCtrl = GetComponent<DepthViewController>();
            if (depthViewCtrl != null)
            {
                depthViewCtrl.enabled = tof;
            }
            else
            {
                tof = false;
            }
        }

        protected override void Start()
        {
            PrepareUI();
            base.Start();
        }

        /// <summary>
        /// Prepare for UI creation
        /// </summary>
        protected virtual void PrepareUI()
        {
            var list = new List<UnityAction>();
            if (color)
            {
                list.Add(() =>
                {
                    MakeUIViewMode("Color", colorViewCtrl, out itemModeColor,
                        out itemAnchorColor, ChangeModeColor, ChangeAnchorColor,
                        OnChangeModeColor, OnChangeAnchorColor);
                });

                controllers.Add(colorViewCtrl);
            }

            if (tof)
            {
                list.Add(() =>
                {
                    MakeUIViewMode("Tof", depthViewCtrl, out itemModeDepth,
                        out itemAnchorDepth, ChangeModeDepth, ChangeAnchorDepth,
                        OnChangeModeDepth, OnChangeAnchorDepth);
                });

                controllers.Add(depthViewCtrl);
            }

            // Set UI order
            uiOrder = list.ToArray();
        }

        protected override void MakeUI()
        {
            base.MakeUI();

            // Set UI
            SwitchViewAnchorInteractable(colorViewCtrl, itemAnchorColor);
            SwitchViewAnchorInteractable(depthViewCtrl, itemAnchorDepth);
        }

        /// <summary>
        /// Create UI for View display method
        /// </summary>
        /// <param name="title">Title prefix of View UI</param>
        /// <param name="viewCtrl">ImageViewController</param>
        /// <param name="itemMode">ViewMode Selection UI</param>
        /// <param name="itemAnchor">ViewAnchor Selection UI</param>
        /// <param name="changeMode">Event that is called when ViewMode Selection UI is used</param>
        /// <param name="changeAnchor">Event that is called when ViewAnchor Selection UI is used</param>
        /// <param name="onChangeMode">Event that is called when ViewMode is changed</param>
        /// <param name="onChangeAnchor">Event that is called when ViewAnchor is changed</param>
        void MakeUIViewMode(string title,
            ImageViewController viewCtrl,
            out UI.ItemDropdown itemMode,
            out UI.ItemDropdown itemAnchor,
            UI.ItemDropdown.ChangeEvent changeMode,
            UI.ItemDropdown.ChangeEvent changeAnchor,
            ControllerBase.ChangeIndexEvent onChangeMode,
            ControllerBase.ChangeIndexEvent onChangeAnchor)
        {
            // View
            itemMode = settings.AddItem($"{title} View", viewCtrl.ModeNames,
                viewCtrl.ModeIndex, changeMode, 0, 200);

            viewCtrl.OnChangeMode += onChangeMode;

            // Display Anchor settings UI if picture-in-picture is configurable
            bool existPictInPict = false;
            for (int i = 0; i < viewCtrl.ModeList.Length; i++)
            {
                if (viewCtrl.ModeList[i] == ImageViewController.Mode.PictureInPicture)
                {
                    existPictInPict = true;
                    break;
                }
            }

            if (existPictInPict)
            {
                // Picture-in-picture
                itemAnchor = settings.AddItem("PictInPict Anchor",
                    viewCtrl.AnchorNames, viewCtrl.AnchorIndex,
                    changeAnchor, 0, 0, 200, lineAlpha);

                viewCtrl.OnChangeAnchor += onChangeAnchor;
            }
            else
            {
                itemAnchor = null;
            }
        }

        /// <summary>
        /// Change View display method
        /// </summary>
        /// <param name="index">Display method index</param>
        void ChangeModeColor(int index)
        {
            colorViewCtrl.ModeIndex = index;
        }

        /// <summary>
        /// Change View's picture-in-picture display position
        /// </summary>
        /// <param name="index">View display position</param>
        void ChangeAnchorColor(int index)
        {
            colorViewCtrl.AnchorIndex = index;
        }

        /// <summary>
        /// Event that is called when UI is used
        /// </summary>
        /// <param name="index">Display method index</param>
        void OnChangeModeColor(int index)
        {
            itemModeColor.Index = index;
            SwitchViewAnchorInteractable(colorViewCtrl, itemAnchorColor);
        }

        /// <summary>
        /// Event that is called when UI is used
        /// </summary>
        /// <param name="index">View display position</param>
        void OnChangeAnchorColor(int index)
        {
            if (itemAnchorColor)
            {
                itemAnchorColor.Index = index;
            }
        }

        /// <summary>
        /// Change View display method
        /// </summary>
        /// <param name="index">Display method index</param>
        void ChangeModeDepth(int index)
        {
            depthViewCtrl.ModeIndex = index;
        }

        /// <summary>
        /// Change View's picture-in-picture display position
        /// </summary>
        /// <param name="index">View display position</param>
        void ChangeAnchorDepth(int index)
        {
            depthViewCtrl.AnchorIndex = index;
        }

        /// <summary>
        /// Event that is called when UI is used
        /// </summary>
        /// <param name="index">Display method index</param>
        void OnChangeModeDepth(int index)
        {
            itemModeDepth.Index = index;
            SwitchViewAnchorInteractable(depthViewCtrl, itemAnchorDepth);
        }

        /// <summary>
        /// Event that is called when UI is used
        /// </summary>
        /// <param name="index">View display position</param>
        void OnChangeAnchorDepth(int index)
        {
            if (itemAnchorDepth)
            {
                itemAnchorDepth.Index = index;
            }
        }

        /// <summary>
        /// Enable/disable picture-in-picture display position UI
        /// </summary>
        /// <param name="viewCtrl">ImageViewController</param>
        /// <param name="itemAnchor">ViewAnchor selection UI</param>
        void SwitchViewAnchorInteractable(ImageViewController viewCtrl,
            UI.ItemDropdown itemAnchor)
        {
            // Enable only when picture-in-picture
            if (!itemAnchor)
            {
                return;
            }

            itemAnchor.Interactable = (viewCtrl.ViewMode ==
                ImageViewController.Mode.PictureInPicture);
        }
    }
}
