/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Body;
using UnityEngine;
using System.Linq;
using TofArSettings;

namespace TofArSamples.Body
{
    public class BodyModelController : AbstractBodyModelController
    {
        public override bool IsShow
        {
            get
            {
                return (BodyIndex < bodyModels.Length && bodyModels[BodyIndex].gameObject.activeInHierarchy);
            }

            set
            {
                isShow = value;
                if (IsShow != value)
                {
                    if (BodyIndex < bodyModels.Length)
                    {
                        bodyModels[BodyIndex].gameObject.SetActive(value);
                    }

                    OnChangeShow?.Invoke(IsShow);
                }
            }
        }

        private int BodyIndex { get => latestIndex < 1 ? bodyModels.Length : latestIndex - 1; }

        int latestIndex = 0;
        public int Index
        {
            get
            {
                return (IsShow) ? latestIndex : 0;
            }

            set
            {
                if (value != Index &&
                    0 <= value && value < ModelNames.Length)
                {
                    if (BodyIndex < bodyModels.Length)
                    {
                        bodyModels[BodyIndex].gameObject.SetActive(false);
                    }

                    latestIndex = value;
                    IsShow = (value > 0);

                    // Switch type if showing
                    if (IsShow)
                    {
                        // Apply scaling again because size change is reset
                        ApplyScaling();
                    }

                    OnChangeIndex?.Invoke(Index);
                }
            }
        }

        public string[] ModelNames { get; private set; }

        public override event ChangeToggleEvent OnChangeShow;

        ModelVisualizer[] bodyModels;

        protected override void Start()
        {
            bodyModels = FindObjectsOfType<ModelVisualizer>(true);
            var bodyNames = bodyModels.Select((b) => b.name).ToList();
            bodyNames.Insert(0, "-");
            ModelNames = bodyNames.ToArray();
            for (int i = 0; i < ModelNames.Length; i++)
            {
                if (ModelNames[i] == "BodySkeleton")
                {
                    Index = i;
                    break;
                }
            }

            reorientSocket = FindObjectOfType<TofAr.V0.Tof.ReorientRelativeColorCamera>();

            base.Start();
        }

        protected override void ApplyOffset()
        {
            for (int i = 0; i < bodyModels.Length; i++)
            {
                Vector3 offsetAdjusted = Offset;

                if (reorientSocket != null && reorientSocket.enabled && reorientSocket.enableRotateInWorld && bodyModels[i].transform.IsChildOf(reorientSocket.transform))
                {

                    int screenOrientation = TofAr.V0.TofArManager.Instance.GetScreenOrientation();

                    switch (screenOrientation)
                    {
                        case 90:
                            offsetAdjusted = adjustPortrait.MultiplyPoint(Offset);
                            break;
                        case 180:
                            offsetAdjusted = adjustLandscapeRight.MultiplyPoint(Offset);
                            break;
                        case 270:
                            offsetAdjusted = adjustPortraitUpsideDown.MultiplyPoint(Offset);
                            break;
                    }
                }

                bodyModels[i].transform.localPosition = offsetAdjusted;
            }
        }

        protected override void ApplyScaling()
        {
            for (int i = 0; i < bodyModels.Length; i++)
            {
                bodyModels[i].transform.localScale = Vector3.one * Scale;
            }
        }

        public event ChangeIndexEvent OnChangeIndex;
    }
}
