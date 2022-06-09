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
    public class RealHandController : HandModelController
    {
        public override bool IsShow
        {
            get
            {
                return (Index != 0);
            }
            set
            {
                if (IsShow != value)
                {
                    Index = (value) ? latestIndex : 0;
                    OnChangeShow?.Invoke(IsShow);
                }
            }
        }

        int latestIndex = 0;
        public int Index
        {
            get
            {
                return (isShow) ? handModel.CurrentHandModelIndex + 1 : 0;
            }

            set
            {
                if (value != Index &&
                    0 <= value && value < HandNames.Length)
                {
                    isShow = (value > 0);

                    // Show/Hide
                    handModel.ShowRealHandToggleChanged(isShow);

                    // Switch type if showing
                    if (isShow)
                    {
                        latestIndex = value;
                        int newIndex = value - 1;
                        handModel.ChangeHandMaterial(newIndex);

                        // Reapply due to size change being reset
                        ApplyScaling();
                    }

                    OnChangeIndex?.Invoke(Index);
                }
            }
        }

        public string[] HandNames { get; private set; }

        public override event ChangeToggleEvent OnChangeShow;

        public event ChangeIndexEvent OnChangeIndex;

        RealHandModel handModel;

        protected override void Start()
        {
            handModel = FindObjectOfType<RealHandModel>();

            var handNames = handModel.GetObjectNames();
            handNames.Insert(0, "-");
            HandNames = handNames.ToArray();

            // Hide
            isShow = false;
            handModel.ShowRealHandToggleChanged(isShow);

            reorientSocket = FindObjectOfType<TofAr.V0.Tof.ReorientRelativeColorCamera>();

            base.Start();
        }

        protected override IHandModel GetHandModel(HandStatus lrHand)
        {
            return (lrHand == HandStatus.LeftHand) ?
                handModel.LeftHandModel : handModel.RightHandModel;
        }

        protected override void ApplyOffset()
        {
            Vector3 offsetAdjusted = Offset;

            if (reorientSocket != null && reorientSocket.enabled && reorientSocket.enableRotateInWorld && handModel.transform.IsChildOf(reorientSocket.transform))
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

            handModel.transform.localPosition = offsetAdjusted;
        }

        private Vector3 ScaleHand(Vector3 handScale, float value)
        {
            var scaleNorm = new Vector3(
                Mathf.Abs(handScale.x) / handScale.x,
                Mathf.Abs(handScale.y) / handScale.y,
                Mathf.Abs(handScale.z) / handScale.z
                );

            return scaleNorm * value;
        }

        protected override void ApplyScaling()
        {
            handModel.handArmatureLeft.localScale = ScaleHand(handModel.handArmatureLeft.localScale, Scale);
            handModel.handArmatureRight.localScale = ScaleHand(handModel.handArmatureRight.localScale, Scale);
        }
    }
}
