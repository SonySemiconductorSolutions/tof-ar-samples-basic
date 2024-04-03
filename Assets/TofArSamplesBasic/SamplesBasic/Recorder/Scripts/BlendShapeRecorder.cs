/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Face;

namespace TofArSamples.Recorder
{
    public class BlendShapeRecorder : CsvRecorder
    {
        public override DataType dataType
        {
            get
            {
                return DataType.BlendShape;
            }
        }

        private void OnEnable()
        {
            dataLength = 52;
            TofArFaceManager.OnFaceEstimated += OnFaceEstimated;
        }

        private void OnFaceEstimated(FaceResults faceResults)
        {
            if (faceResults.results.Length == 0) { return; }
            SetData(string.Join(",", faceResults.results[0].blendShapes));
        }
    }
}
