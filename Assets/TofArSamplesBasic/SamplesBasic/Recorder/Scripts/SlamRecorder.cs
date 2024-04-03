/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;

namespace TofArSamples.Recorder
{
    public class SlamRecorder : CsvRecorder
    {
        public override DataType dataType
        {
            get
            {
                return DataType.Slam;
            }
        }

        private void OnEnable()
        {
            // Position + Rotation
            dataLength = 3 + 4;

            TofArSlamManager.OnFrameArrived += OnFrameArrived;
        }

        private void OnFrameArrived(object sender)
        {
            if (TofArSlamManager.Instance.SlamData.Data == null) { return; }
            var slam = TofArSlamManager.Instance.SlamData.Data;
            var data = "";
            for (int i = 0; i < 3; i++)
            {
                data += slam.Position[i] + ",";
            }

            for (int i = 0; i < 4; i++)
            {
                data += slam.Rotation[i] + ",";
            }
            SetData(data.Remove(data.Length - 1));
        }

        protected override string CreateHeader()
        {
            return "Timestamp,Position_X,Position_Y,Position_Z,Rotation_X,Rotation_Y,Rotation_Z,Rotation_W";
        }
    }
}