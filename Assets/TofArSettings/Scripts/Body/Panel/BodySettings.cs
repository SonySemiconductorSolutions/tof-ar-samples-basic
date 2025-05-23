/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */
using TofAr.V0.Body;

using UnityEngine;
using UnityEngine.Events;

namespace TofArSettings.Body
{
    public class BodySettings : UI.SettingsBase
    {
        BodyRuntimeController runtimeController;
        BodyManagerController managerController;
        SV2Controller sv2Controller;

        UI.ItemDropdown itemMode, itemRuntimeModeSV2, itemRecogModeSV2, itemNoiseReduction, itemHumanTrackingMode;
        UI.ItemSlider itemSV2Thread;
        UI.ItemToggle itemStartStream;
        UI.ItemSlider itemNotRecogInterval, itemFrameNoBody;

        UI.ItemText itemRecognizerVersion;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIStartStream,
                MakeUIDetectorType,
                MakeUIRuntime,
                MakeUIIntervalFrameNotRecog,
                MakeUIFramesForDetectNoBody,
                MakeUIRecognizerVersion
            };

            managerController = FindAnyObjectByType<BodyManagerController>();
            controllers.Add(managerController);
            runtimeController = managerController.GetComponent<BodyRuntimeController>();
            controllers.Add(runtimeController);
            sv2Controller = managerController.GetComponent<SV2Controller>();
            controllers.Add(sv2Controller);

            base.Start();

            settings.OnChangeStart += OnChangePanel;
        }

        /// <summary>
        /// Make Body dictionary UI
        /// </summary>
        void MakeUIDetectorType()
        {
            itemMode = settings.AddItem("Detector Type", runtimeController.DetectorTypeNames,
                runtimeController.DetectorTypeIndex, ChangeMode, 0, 0, 200);

            runtimeController.OnChangeDetectorType += (index) =>
            {
                itemMode.Index = index;
                SetSV2Interactability();
            };
        }

        /// <summary>
        /// Change Body dictionary
        /// </summary>
        /// <param name="index">Body dictionary index</param>
        void ChangeMode(int index)
        {
            runtimeController.DetectorTypeIndex = index;
        }

        /// <summary>
        /// Make Runtime UI
        /// </summary>
        void MakeUIRuntime()
        {
            settings.AddItem("SV2", FontStyle.Bold);
            itemRuntimeModeSV2 = settings.AddItem(" Runtime", sv2Controller.RuntimeModeNames,
                sv2Controller.RuntimeModeIndex, ChangeRuntimeModeSV2, 0, 0, 300, lineAlpha);
            itemRecogModeSV2 = settings.AddItem(" RecogMode", sv2Controller.RecogModeNames,
                sv2Controller.RecogModeIndex, ChangeRecogModeSV2, 0, 0, 280, lineAlpha);
            itemSV2Thread = settings.AddItem(" Threads",
                SV2Controller.ThreadMin, SV2Controller.ThreadMax,
                SV2Controller.ThreadStep, sv2Controller.ModeThreads,
                ChangeSV2Threads, 0, 0, lineAlpha);
            itemNoiseReduction = settings.AddItem("Noise Reduction Level", sv2Controller.NoiseReductionLevelNames,
                sv2Controller.NoiseReductionIndex, ChangeNoiseReductionSV2, -4, 0, 260, lineAlpha);
            itemHumanTrackingMode = settings.AddItem("Human Tracking Mode", sv2Controller.HumanTrackingModeNames,
                sv2Controller.HumanTrackingModeIndex, ChangeHumanTrackingModeSV2, -4, 0, 260, lineAlpha);

            sv2Controller.OnChangeRuntimeMode += (index) =>
            {
                itemRuntimeModeSV2.Index = index;
                itemSV2Thread.Interactable = sv2Controller.IsInteractableModeThreads;
            };

            sv2Controller.OnUpdateRuntimeModeList += (list, runtimeModeIndex) =>
            {
                itemRuntimeModeSV2.Options = list;
                itemRuntimeModeSV2.Index = runtimeModeIndex;
            };

            sv2Controller.OnChangeRecogMode += (index, conf) =>
            {
                itemRecogModeSV2.Index = index;
            };

            sv2Controller.OnUpdateRecogModeList += (list, recogModeIndex) =>
            {
                itemRecogModeSV2.Options = list;
                itemRecogModeSV2.Index = recogModeIndex;
            };

            sv2Controller.OnChangeNoiseReductionLevel += (index) =>
            {
                itemNoiseReduction.Index = index;
            };

            sv2Controller.OnUpdateNoiseReductionList += (list, index) =>
            {
                itemNoiseReduction.Options = list;
                itemNoiseReduction.Index = index;
            };

            sv2Controller.OnUpdateHumanTrackingModeList += (list, index) =>
            {
                itemHumanTrackingMode.Options = list;
                itemHumanTrackingMode.Index = index;
            };

            sv2Controller.OnChangeModeThreads += (val) =>
            {
                itemSV2Thread.Value = val;
            };

            SetSV2Interactability();
        }

        /// <summary>
        /// Enabled or disable the interactibility of sv2 UI based on the runtime controller
        /// </summary>
        void SetSV2Interactability()
        {
            var interactible = runtimeController.DetectorType == TofAr.V0.Body.BodyPoseDetectorType.Internal_SV2;
            itemRuntimeModeSV2.Interactable = interactible;
            itemRecogModeSV2.Interactable = interactible;
            itemSV2Thread.Interactable = interactible && sv2Controller.IsInteractableModeThreads;
            itemNoiseReduction.Interactable = interactible;
        }

        /// <summary>
        /// Change RuntimeMode2
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRuntimeModeSV2(int index)
        {
            sv2Controller.RuntimeModeIndex = index;
        }

        /// <summary>
        /// Change RecogMode
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeRecogModeSV2(int index)
        {
            sv2Controller.RecogModeIndex = index;
        }

        /// <summary>
        /// Change NoiseReductionLevel
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeNoiseReductionSV2(int index)
        {
            sv2Controller.NoiseReductionIndex = index;
        }

        /// <summary>
        /// Change HumanTrackingMode
        /// </summary>
        /// <param name="index">RuntimeMode index</param>
        void ChangeHumanTrackingModeSV2(int index)
        {
            sv2Controller.HumanTrackingModeIndex = index;
        }

        /// <summary>
        /// Change thread count of RuntimeMode2
        /// </summary>
        /// <param name="val">Thread count</param>
        void ChangeSV2Threads(float val)
        {
            sv2Controller.ModeThreads = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make StartStream UI
        /// </summary>
        void MakeUIStartStream()
        {
            itemStartStream = settings.AddItem("Start Stream", TofArBodyManager.Instance.autoStart, ChangeStartStream);
            managerController.OnStreamStartStatusChanged += (val) =>
            {
                itemStartStream.OnOff = val;
            };
        }

        /// <summary>
        /// If stream oocurs or not
        /// </summary>
        /// <param name="val">Stream started or not</param>
        void ChangeStartStream(bool val)
        {
            if (val)
            {
                managerController.StartStream();
            }
            else
            {
                managerController.StopStream();
            }
        }

        /// <summary>
        /// Event called when the state of the panel changes
        /// </summary>
        /// <param name="onOff">open/close</param>
        void OnChangePanel(bool onOff)
        {
            if (onOff)
            {
                itemStartStream.OnOff = TofArBodyManager.Instance.IsStreamActive;
            }
        }

        /// <summary>
        /// Make IntervalFrameNotRecognized UI
        /// </summary>
        void MakeUIIntervalFrameNotRecog()
        {
            itemNotRecogInterval = settings.AddItem("IntervalFrame\nNotRecognized",
                SV2Controller.IntervalFrameNotRecognizedMin,
                SV2Controller.IntervalFrameNotRecognizedMax,
                SV2Controller.IntervalFrameNotRecognizedStep,
                sv2Controller.IntervalFrameNotRecognized,
                ChangeIntervalFrameNotRecog, -4);

            sv2Controller.OnChangeIntervalFrameNotRecognized += (val) =>
            {
                itemNotRecogInterval.Value = val;
            };
        }

        /// <summary>
        /// Change IntervalFrameNotRecognized
        /// </summary>
        /// <param name="val">IntervalFrameNotRecognized</param>
        void ChangeIntervalFrameNotRecog(float val)
        {
            sv2Controller.IntervalFrameNotRecognized = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make FramesForDetectNoBody UI
        /// </summary>
        void MakeUIFramesForDetectNoBody()
        {
            itemFrameNoBody = settings.AddItem("FramesFor\nDetectNoBody",
                SV2Controller.FramesForDetectNoBodyMin,
                SV2Controller.FramesForDetectNoBodyMax,
                SV2Controller.FramesForDetectNoBodyStep,
                sv2Controller.FramesForDetectNoBody,
                ChangeFramesForDetectNoBody, -4);

            sv2Controller.OnChangeFramesForDetectNoBody += (val) =>
            {
                itemFrameNoBody.Value = val;
            };
        }

        /// <summary>
        /// Change FramesForDetectNoBody
        /// </summary>
        /// <param name="val">FramesForDetectNoBody</param>
        void ChangeFramesForDetectNoBody(float val)
        {
            sv2Controller.FramesForDetectNoBody = Mathf.RoundToInt(val);
        }

        /// <summary>
        /// Make RecognizerVersion Text UI
        /// </summary>
        void MakeUIRecognizerVersion()
        {
            string version = string.Empty;

            if (TofArBodyManager.Instance != null)
            {
                var recognizerVersion = TofArBodyManager.Instance.GetProperty<TofAr.V0.Body.RecognizerVersionProperty>();
                version = recognizerVersion.versionString;
            }

            itemRecognizerVersion = settings.AddItem("Recognizer Version : " + version, FontStyle.Normal, false, -6);
        }
    }
}
