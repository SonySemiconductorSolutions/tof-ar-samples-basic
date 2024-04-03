/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */


using System;
using TofAr.V0.Body;
using TofAr.V0.Body.SV2;

namespace TofArSamples.Recorder
{
    public class BodyRecorder : CsvRecorder
    {
        private TofArSettings.Body.SV2Controller sv2Controller;
        private RecogMode recogMode;

        public override DataType dataType
        {
            get
            {
                return DataType.Body;
            }
        }

        private void OnEnable()
        {
            // Position + Rotaion + 1 points + DataSource
            dataLength = 3 + 4 + 3 * Enum.GetNames(typeof(JointIndices)).Length + 1;

            sv2Controller = FindObjectOfType<TofArSettings.Body.SV2Controller>();

            TofArBodyManager.OnBodyPoseEstimated += OnBodyPoseEstimated;
        }

        public override void StartRectoring()
        {
            recogMode = sv2Controller.RecogMode;
            base.StartRectoring();
        }


        private void OnBodyPoseEstimated(BodyResults bodyResults)
        {
            if (bodyResults.results.Length == 0) { return; }
            var result = "";
            var body = bodyResults.results[0];
            result += body.pose.Position.x + ",";
            result += body.pose.Position.y + ",";
            result += body.pose.Position.z + ",";

            result += body.pose.rotation.x + ",";
            result += body.pose.rotation.y + ",";
            result += body.pose.rotation.z + ",";
            result += body.pose.rotation.w + ",";

            foreach (var joint in body.joints)
            {
                result += joint.anchorPose.Position.x + ",";
                result += joint.anchorPose.Position.y + ",";
                result += joint.anchorPose.Position.z + ",";
            }

            result += (int)bodyResults.frameDataSource + ",";
            result += (int)recogMode;
            SetData(result);
        }

        protected override string CreateHeader()
        {
            var result = "Timestamp,Position_X, Position_Y, Position_Z, Rotaion_X, Rotaion_Y, Rotaion_Z, Rotaion_W";
            for (int i = 0; i < Enum.GetNames(typeof(JointIndices)).Length - 1; i++)
            {
                result += string.Format(",{0}_X", (JointIndices)i);
                result += string.Format(",{0}_Y", (JointIndices)i);
                result += string.Format(",{0}_Z", (JointIndices)i);
            }
            result += ",FrameDataSource,RecogMode";
            return result;
        }
    }
}
