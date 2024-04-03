/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */


using System;
using System.Linq;
using UnityEngine;
using UI = TofArSettings.UI;

namespace TofArSamples.Recorder
{
    public class DataSettings : UI.SettingsBase
    {
        DataController dataCtrl;
        TofArSettings.Hand.GestureTypeController gestureTypeCtrl;
        TofArSettings.Hand.GestureController gestureCtrl;

        [SerializeField]
        UI.ToolButton startButton;
        [SerializeField]
        UI.ToolButton stopButton;
        [SerializeField]
        private UnityEngine.UI.Slider byteSizeSlider;

        UI.ItemDropdown recodeMode;

        UI.ItemToggle hand;
        UI.ItemToggle body;
        UI.ItemToggle face;
        UI.ItemToggle blendShape;
        UI.ItemToggle slam;
        UI.ItemToggle depth;
        UI.ItemToggle color;
        UI.ItemDropdown colorScale;
        UI.ItemSlider colorSkip;


        private void Awake()
        {
            dataCtrl = GetComponent<DataController>();
            dataCtrl.enabled = true;
            dataCtrl.finishedRecordDelegate += (() =>
            {
                startButton.gameObject.SetActive(true);
                stopButton.gameObject.SetActive(false);
            });

            gestureTypeCtrl = FindObjectOfType<TofArSettings.Hand.GestureTypeController>();
            gestureTypeCtrl.enabled = true;

            gestureCtrl = FindObjectOfType<TofArSettings.Hand.GestureController>();
            gestureCtrl.enabled = true;
        }

        protected override void Start()
        {
            base.Start();

            startButton.OnClick += OnClickStart;
            stopButton.OnClick += OnClickStart;

            byteSizeSlider.minValue = 0;
            byteSizeSlider.maxValue = dataCtrl.MaxByteSize;
            byteSizeSlider.value = 0;
        }

        private void Update()
        {
            byteSizeSlider.value = dataCtrl.TotalByteSize;
        }

        protected override void MakeUI()
        {
            settings.AddItem("Timer", 0, 10, 1, dataCtrl.timer, (float value) =>
            {
                dataCtrl.timer = (int)value;
            });

            recodeMode = settings.AddItem("Record Mode", Enum.GetNames(typeof(RecordMode)).ToArray(), (int)dataCtrl.recordMode, (int index) =>
            {
                dataCtrl.recordMode = (RecordMode)index;
            });


            MakeDataTypeUI();

            base.MakeUI();
        }


        private void OnClickStart(bool onOff)
        {
            if (onOff)
            {
                startButton.OnOff = false;
                startButton.gameObject.SetActive(false);

                stopButton.gameObject.SetActive(true);
                stopButton.OnOff = true;

                dataCtrl.StartRecord();
            }
            else
            {
                startButton.gameObject.SetActive(true);
                stopButton.gameObject.SetActive(false);

                dataCtrl.StopRecord();
            }
        }


        private void MakeDataTypeUI()
        {
            depth = settings.AddItem("Save Depth", dataCtrl.Depth, (bool onOff) =>
            {
                dataCtrl.Depth = onOff;
                dataCtrl.UpdateRecorder(DataType.Depth, onOff);
            });

            color = settings.AddItem("Save Color", dataCtrl.Color, (bool onOff) =>
            {
                dataCtrl.Color = onOff;
                colorScale.Interactable = onOff;
                colorSkip.Interactable = onOff;
                dataCtrl.UpdateRecorder(DataType.Color, onOff);
            });

            colorScale = settings.AddItem("Color Scale", dataCtrl.colorScales.Select((s) => (100 / s) + "%").ToArray(), dataCtrl.colorScaleIndex, (int index) =>
            {
                dataCtrl.colorScaleIndex = index;
            });

            colorSkip = settings.AddItem("Color Skip", 0, 10, 1, dataCtrl.colorSkip, (float value) =>
            {
                dataCtrl.colorSkip = (int)value;
            });


            hand = settings.AddItem("Save Hand", dataCtrl.Hand, (bool onOff) =>
            {
                dataCtrl.Hand = onOff;
                dataCtrl.UpdateRecorder(DataType.Hand, onOff);
            });

            body = settings.AddItem("Save Body", dataCtrl.Body, (bool onOff) =>
            {
                dataCtrl.Body = onOff;
                dataCtrl.UpdateRecorder(DataType.Body, onOff);
            });

            face = settings.AddItem("Save Face", dataCtrl.Face, (bool onOff) =>
            {
                dataCtrl.Face = onOff;
                dataCtrl.UpdateRecorder(DataType.Face, onOff);
            });

            blendShape = settings.AddItem("Save BlendShape", dataCtrl.BlendShape, (bool onOff) =>
            {
                dataCtrl.BlendShape = onOff;
                dataCtrl.UpdateRecorder(DataType.BlendShape, onOff);
            });

            slam = settings.AddItem("Save SLAM", dataCtrl.Slam, (bool onOff) =>
            {
                dataCtrl.Slam = onOff;
                dataCtrl.UpdateRecorder(DataType.Slam, onOff);
            });
        }
    }
}
