/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.Plane
{
    public class PlaneViewSettings : UI.SettingsBase
    {
        PlaneModelController modelCtrl;

        UI.ItemToggle itemPlane;

        UI.ItemButton itemAddPlane, itemRemovePlane, itemClearPlanes;
        UI.ItemSlider itemInterval, itemDetectInterval, itemKernelSize, itemMinimumSize, itemThreshold;

        protected virtual void Awake()
        {
            modelCtrl = GetComponent<PlaneModelController>();
            uiOrder = new UnityAction[0];
        }

        protected override void Start()
        {
            PrepareUI();
            base.Start();            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            modelCtrl.OnChangeInterval -= UpdatePlaneInterval;
            modelCtrl.OnChangeKernelSize -= UpdateKernelSize;
            modelCtrl.OnChangeMinimumSize -= UpdateMinimumSize;
            modelCtrl.OnChangeThreshold -= UpdateThreshold;
        }

        protected virtual void PrepareUI()
        {
            var list = new List<UnityAction>(uiOrder);
            controllers.Add(modelCtrl);

            list.Add(MakeUIPlane);
            list.Add(MakeUIPlaneInterval);
            list.Add(MakeUIKernelSize);
            list.Add(MakeUIMinimumSize);
            list.Add(MakeUIThreshold);

            //Subscribe events for the slider values
            modelCtrl.OnChangeInterval += UpdatePlaneInterval;
            modelCtrl.OnChangeKernelSize += UpdateKernelSize;
            modelCtrl.OnChangeMinimumSize += UpdateMinimumSize;
            modelCtrl.OnChangeThreshold += UpdateThreshold;

            // Set UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make Plane model UI
        /// </summary>
        void MakeUIPlane()
        {
            itemPlane = settings.AddItem("Show Plane", modelCtrl.IsPlaneShown,
                ChangePlane);

            modelCtrl.OnChangeShow += (onOff) =>
            {
                itemPlane.OnOff = onOff;
            };

            settings.AddItem("Add Plane", AddPlane);
            settings.AddItem("Remove Plane", RemovePlane);
        }

        /// <summary>
        /// Make Plane Interval UI
        /// </summary>
        void MakeUIPlaneInterval()
        {
            itemInterval = settings.AddItem("Plane Interval",
                PlaneModelController.IntervalMin,
                PlaneModelController.IntervalMax,
                PlaneModelController.IntervalStep,
                modelCtrl.Interval,
                ChangePlaneInterval, -4);

            itemInterval.OnChange += (val) =>
            {
                itemInterval.Value = val;
            };
        }

        /// <summary>
        /// Change Plane Interval
        /// </summary>
        /// <param name="val">PlaneInterval</param>
        void ChangePlaneInterval(float val)
        {
            modelCtrl.Interval = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Update the interval value on the slider
        /// </summary>
        /// <param name="val"></param>
        void UpdatePlaneInterval(float val)
        {
            if (itemInterval != null)
            {
                itemInterval.Value = val;
            }
        }

        /// <summary>
        /// Make Detect Interval UI
        /// </summary>
        void MakeUIKernelSize()
        {
            itemKernelSize = settings.AddItem("Kernel Size",
                PlaneModelController.KernelSizeMin,
                PlaneModelController.KernelSizeMax,
                PlaneModelController.KernelSizeStep,
                modelCtrl.KernelSize,
                ChangeKernelSize, -4);

            itemKernelSize.OnChange += (val) =>
            {
                itemKernelSize.Value = val;
            };
        }

        /// <summary>
        /// Change Detect Interval
        /// </summary>
        /// <param name="val">KernelSize</param>
        void ChangeKernelSize(float val)
        {
            modelCtrl.KernelSize = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Update the kernel size value on the slider
        /// </summary>
        /// <param name="val"></param>
        void UpdateKernelSize(float val)
        {
            if (itemKernelSize != null)
            {
                itemKernelSize.Value = val;
            }
        }

        /// <summary>
        /// Make Minimum Size UI
        /// </summary>
        void MakeUIMinimumSize()
        {
            itemMinimumSize = settings.AddItem("Minimum Size",
                PlaneModelController.MinimumSizeMin,
                PlaneModelController.MinimumSizeMax,
                PlaneModelController.MinimumSizeStep,
                modelCtrl.MinimumSize,
                ChangeMinimumSize, -4);

            itemMinimumSize.OnChange += (val) =>
            {
                itemMinimumSize.Value = val;
            };
        }

        /// <summary>
        /// Change Minimum Size
        /// </summary>
        /// <param name="val">MinimumSize</param>
        void ChangeMinimumSize(float val)
        {
            modelCtrl.MinimumSize = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Update the minimum size value on the slider
        /// </summary>
        /// <param name="val"></param>
        void UpdateMinimumSize(float val)
        {
            if (itemMinimumSize != null)
            {
                itemMinimumSize.Value = val;
            }
        }

        /// <summary>
        /// Make Minimum Size UI
        /// </summary>
        void MakeUIThreshold()
        {
            itemThreshold = settings.AddItem("Plane Threshold",
                PlaneModelController.ThresholdMin,
                PlaneModelController.ThresholdMax,
                PlaneModelController.ThresholdStep,
                modelCtrl.PlaneThreshold,
                ChangeThreshold, -4);

            itemThreshold.OnChange += (val) =>
            {
                itemThreshold.Value = val;
            };
        }

        /// <summary>
        /// Change Threshold value
        /// </summary>
        /// <param name="val">PlaneThreshold</param>
        void ChangeThreshold(float val)
        {
            modelCtrl.PlaneThreshold = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Update the threshold value on the slider
        /// </summary>
        /// <param name="val"></param>
        void UpdateThreshold(float val)
        {
            if (itemThreshold != null)
            {
                itemThreshold.Value = val;
            }
        }

        /// <summary>
        /// Toggle Face Model display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangePlane(bool onOff)
        {
            modelCtrl.IsPlaneShown = onOff;
        }

        void AddPlane()
        {
            modelCtrl.AddPlane();
        }

        void RemovePlane()
        {
            modelCtrl.RemovePlane();
        }
    }
}