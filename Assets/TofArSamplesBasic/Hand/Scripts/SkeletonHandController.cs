/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSamples.Hand
{
    public class SkeletonHandController : HandModelController
    {
        public override bool IsShow
        {
            get
            {
                bool isShow = false;
                for (int i = 0; i < handModels.Length; i++)
                {
                    if (handModels[i].enabled)
                    {
                        isShow = true;
                        break;
                    }
                }

                return isShow;
            }

            set
            {
                if (IsShow != value)
                {
                    for (int i = 0; i < handModels.Length; i++)
                    {
                        handModels[i].enabled = value;
                    }

                    OnChangeShow?.Invoke(IsShow);
                }
            }
        }

        public override event ChangeToggleEvent OnChangeShow;

        HandModel[] handModels;

        protected override void Start()
        {
            handModels = FindObjectsOfType<HandModel>();

            reorientSocket = FindObjectOfType<TofAr.V0.Tof.ReorientRelativeColorCamera>();

            base.Start();
        }

        protected override IHandModel GetHandModel(HandStatus lrHand)
        {
            HandModel result = null;
            for (int i = 0; i < handModels.Length; i++)
            {
                var hm = handModels[i];
                if (hm.LRHand == lrHand)
                {
                    result = hm;
                    break;
                }
            }

            return result;
        }

        protected override void ApplyOffset()
        {
            for (int i = 0; i < handModels.Length; i++)
            {
                Vector3 offsetAdjusted = Offset;

                if (reorientSocket != null && reorientSocket.enabled && reorientSocket.enableRotateInWorld && handModels[i].transform.IsChildOf(reorientSocket.transform))
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

                handModels[i].transform.localPosition = offsetAdjusted;
            }
        }

        protected override void ApplyScaling()
        {
            for (int i = 0; i < handModels.Length; i++)
            {
                handModels[i].transform.localScale = Vector3.one * Scale;
            }
        }
    }
}
