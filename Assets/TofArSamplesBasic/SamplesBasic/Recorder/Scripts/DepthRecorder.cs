/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using TofAr.V0.Tof;

namespace TofArSamples.Recorder
{
    public class DepthRecorder : BinaryRecorder
    {
        public override DataType dataType
        {
            get
            {
                return DataType.Depth;
            }
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            TofArTofManager.OnFrameArrived += OnFrameArrived;
        }

        private void OnFrameArrived(object sender)
        {
            if (TofArTofManager.Instance.DepthData == null) { return; }
            var datetime = DateTime.Now;
            var data = TofArTofManager.Instance.DepthData.Data;
            var bytes = new byte[data.Length * 2];
            Buffer.BlockCopy(data, 0, bytes, 0, data.Length * 2);
            SetData(bytes, datetime);
        }
    }
}
