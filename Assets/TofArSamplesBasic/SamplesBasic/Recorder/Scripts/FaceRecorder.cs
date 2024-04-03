/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using TofAr.V0.Face;

namespace TofArSamples.Recorder
{
    public class FaceRecorder : CsvRecorder
    {
        public override DataType dataType
        {
            get
            {
                return DataType.Face;
            }
        }

        private void OnEnable()
        {
            TofArFaceManager.OnFaceEstimated += OnFaceEstimated;
        }


        public void OnFaceEstimated(FaceResults faceResults)
        {
            if (faceResults.results.Length == 0) { return; }
            var faceResult = faceResults.results[0];

            // Position + Rotaion + vertices
            dataLength = 3 + 4 + faceResult.vertices.Length * 3;

            var data = "";
            for (int i = 0; i < 3; i++)
            {
                data += ((Vector3)faceResult.pose.position)[i] + ",";
            }

            for (int i = 0; i < 4; i++)
            {
                data += ((Quaternion)faceResult.pose.rotation)[i] + ",";
            }

            foreach (var vertices in faceResult.vertices)
            {
                for (int i = 0; i < 3; i++)
                {
                    data += ((Vector3)vertices)[i] + ",";
                }
            }

            SetData(data.Remove(data.Length - 1));
        }
    }
}
