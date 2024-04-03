/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Hand;

namespace TofArSamples.Recorder
{
    public class HandRecorder : CsvRecorder
    {
        [SerializeField]
        private HandType handType;

        public override DataType dataType
        {
            get
            {
                return DataType.Hand;
            }
        }

        private List<HandPointIndex> ignoreIndexs = new List<HandPointIndex>
    {
        HandPointIndex.WristPinkySide,
        HandPointIndex.WristThumbSide,
        HandPointIndex.HandCenter,
        HandPointIndex.ArmCenter
    };

        private void OnEnable()
        {
            TofArHandManager.OnFrameArrived += OnFrameArrived;
        }


        private void OnFrameArrived(object sender)
        {
            if (TofArHandManager.Instance.HandData == null) { return; }
            var handData = TofArHandManager.Instance.HandData;
            if (handData.Data.handStatus == HandStatus.NoHand) { return; }
            handData.GetPoseIndex(out var left, out var right);
            var pose = handType == HandType.Left ? (int)left : (int)right;
            var points = handType == HandType.Left ? handData.Data.featurePointsLeft : handData.Data.featurePointsRight;

            var result = "";
            for (int i = 0; i < points.Length; i++)
            {
                if (ignoreIndexs.Contains((HandPointIndex)i)) { continue; }
                result += points[i].x.ToString("f3") + ",";
                result += points[i].y.ToString("f3") + ",";
                result += points[i].z.ToString("f3") + ",";
            }
            result += pose;
            SetData(result);
        }

        protected override string CreateHeader()
        {
            var result = "Timestamp";
            for (int i = 0; i < 21; i++)
            {
                result += string.Format(",Joint{0}_X", i);
                result += string.Format(",Joint{0}_Y", i);
                result += string.Format(",Joint{0}_Z", i);
            }
            result += ",PoseID";
            return result;
        }
    }
}
