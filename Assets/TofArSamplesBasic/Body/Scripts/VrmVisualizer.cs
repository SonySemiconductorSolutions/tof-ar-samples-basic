/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections.Generic;
using TofAr.V0;
using TofAr.V0.Body;
using TofAr.V0.Body.SV2;
using UnityEngine;

namespace TofArSamples.Body
{
    public class VrmVisualizer : ModelVisualizer
    {
        private class JointInfo
        {
            /// <summary>
            /// Transform of the model control object
            /// </summary>
            public Transform Trans;

            /// <summary>
            /// Initial rotation
            /// Assumed T-pose and this angle is targetted when no arms or leg ends are detected
            /// </summary>
            public Quaternion InitRotation;

            /// <summary>
            /// Target position of recognition result
            /// </summary>
            public Vector3 Target;

            /// <summary>
            /// Rotation of previous frame
            /// </summary>
            public Quaternion PreLocalRotation;

            /// <summary>
            /// Flag for whether detected or not
            /// </summary>
            public bool IsDetected;
        }

        private Animator anim;
        private Dictionary<HumanBodyBones, JointInfo> joints = new Dictionary<HumanBodyBones, JointInfo>();
        private SkinnedMeshRenderer[] renderers;

        public Transform RootTransform;

        public float nrValue = 0.4f;

        /// <summary>
        /// Maximum rotation value per second
        /// </summary>
        public float maxDegreeSpeed = 240;

        private float heightVrm = 1f;
        private float heightReal = 1f;
        private float widthVrm = 1f;
        private float widthReal = 1f;


        [SerializeField]
        protected uint bodyIndex = 0;

        private int currentRotation = 0;

        private Dictionary<JointIndices, HumanBodyBones> jointsMapping = new Dictionary<JointIndices, HumanBodyBones>
        {
            { JointIndices.Spine1, HumanBodyBones.Hips},
            { JointIndices.Spine4, HumanBodyBones.Spine},
            { JointIndices.LeftUpLeg, HumanBodyBones.LeftUpperLeg},
            { JointIndices.LeftLeg, HumanBodyBones.LeftLowerLeg},
            { JointIndices.LeftFoot, HumanBodyBones.LeftFoot},
            { JointIndices.RightUpLeg, HumanBodyBones.RightUpperLeg},
            { JointIndices.RightLeg, HumanBodyBones.RightLowerLeg},
            { JointIndices.RightFoot, HumanBodyBones.RightFoot},
            { JointIndices.LeftForearm, HumanBodyBones.LeftLowerArm},
            { JointIndices.LeftHand, HumanBodyBones.LeftHand},
            { JointIndices.RightHand, HumanBodyBones.RightHand},
            { JointIndices.RightForearm, HumanBodyBones.RightLowerArm},
            { JointIndices.Neck1, HumanBodyBones.Neck},
            { JointIndices.RightShoulder1, HumanBodyBones.RightUpperArm},
            { JointIndices.LeftShoulder1, HumanBodyBones.LeftUpperArm},

        };

        public void Awake()
        {
            anim = GetComponent<Animator>();
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            RootTransform.rotation = Quaternion.identity;

            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone < 0 || bone >= HumanBodyBones.LastBone)
                {
                    continue;
                }
                joints[bone] = new JointInfo();

                Transform t = anim.GetBoneTransform(bone);
                if (t != null)
                {
                    joints[bone].Trans = t;
                    joints[bone].InitRotation = t.localRotation;
                }
            }

            foreach (SkinnedMeshRenderer sk in this.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                sk.updateWhenOffscreen = true;
            }

            this.heightVrm = ((anim.GetBoneTransform(HumanBodyBones.Spine).position - anim.GetBoneTransform(HumanBodyBones.Hips).position).magnitude + (anim.GetBoneTransform(HumanBodyBones.Neck).position - anim.GetBoneTransform(HumanBodyBones.Spine).position).magnitude);

            this.widthVrm = ((anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position - anim.GetBoneTransform(HumanBodyBones.Neck).position).magnitude + (anim.GetBoneTransform(HumanBodyBones.Neck).position - anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position).magnitude);
        }

        private void OnEnable()
        {
            TofArBodyManager.OnStreamStarted += OnBodyStreamStarted;
            TofArBodyManager.OnStreamStopped += OnBodyStreamStopped;
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationUpdated;

            isStreamActive = TofArBodyManager.Instance.IsStreamActive;
        }

        private void OnDisable()
        {
            TofArBodyManager.OnStreamStarted -= OnBodyStreamStarted;
            TofArBodyManager.OnStreamStopped -= OnBodyStreamStopped;
            TofArManager.OnScreenOrientationUpdated += OnScreenOrientationUpdated;
        }

        private void OnScreenOrientationUpdated(ScreenOrientation previousScreenOrientation, ScreenOrientation newScreenOrientation)
        {
            UpdateOrientation();
        }

        private void OnBodyStreamStarted(object sender)
        {
            UpdateOrientation();
            isStreamActive = true;
        }

        private void OnBodyStreamStopped(object sender)
        {
            isStreamActive = false;
            foreach (var rend in renderers)
            {
                rend.enabled = false;
            }
        }

        private void UpdateOrientation()
        {
            if (TofArBodyManager.Instance.IsPlaying)
            {
                var prop = TofArBodyManager.Instance.GetProperty<CameraOrientationProperty>();

                var orientation = prop.cameraOrientation;

                int screenRotation = 0;
                switch (orientation)
                {
                    case CameraOrientation.Portrait:
                        screenRotation = 270; break;
                    case CameraOrientation.LandscapeRight:
                        screenRotation = 180; break;
                    case CameraOrientation.PortraitUpsideDown:
                        screenRotation = 90; break;
                }
                int currentScreenOrientation = TofArManager.Instance.GetScreenOrientation();

                var detectorProp = TofArBodyManager.Instance.GetProperty<DetectorTypeProperty>();

                if (detectorProp.detectorType == BodyPoseDetectorType.External)
                {
                    this.currentRotation = (currentScreenOrientation - screenRotation);
                }
                else
                {
                    this.currentRotation = (screenRotation - currentScreenOrientation);
                }
            }
            else
            {
                this.currentRotation = 0;
            }
        }

        //private bool isFullbody;

        private Vector3 rootPosition = Vector3.zero;
        private Quaternion rootRotation = Quaternion.identity;

        private bool isStreamActive = false;

        private void Update()
        {
            if (isStreamActive && (0 <= this.bodyIndex) && (this.bodyIndex < TofArBodyManager.Instance?.BodyData?.Data?.results?.Length))
            {
                this.Apply(TofArBodyManager.Instance?.BodyData?.Data?.results[this.bodyIndex]);
            }
        }

        private Vector3 anchorPoint;
        private Quaternion anchorRotation;

        private bool SetJoint(HumanBodyBones b, HumanBodyJoint joint)
        {
            JointInfo j;
            if (!joints.TryGetValue(b, out j))
            {
                return false;
            }

            if (joint == null || !joint.tracked)
            {
                j.IsDetected = false;
                return false;
            }
            else
            {
                j.IsDetected = true;
                j.Target = (anchorRotation * joint.anchorPose.position.GetVector3()) + anchorPoint;
                return true;
            }
        }

        private bool isTracked = false;
        private bool isSV2 = false;

        public override void Apply(BodyResult bodyResult)
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }
            foreach (var rend in renderers)
            {
                rend.enabled = false;
            }
            isTracked = bodyResult.trackingState != TrackingState.None && bodyResult.joints.Length > 0 && bodyResult.joints[0] != null;
            if (!isTracked)
            {
                return;
            }

            anchorPoint = bodyResult.pose.position.GetVector3();
            anchorRotation = bodyResult.pose.rotation.GetQuaternion();

            foreach (var rend in renderers)
            {
                rend.enabled = true;
            }

            float scale = RootTransform.localScale.x;
            RootTransform.localScale = new Vector3(1, 1, 1);
            var availableJoints = new Dictionary<HumanBodyBones, HumanBodyJoint>();


            foreach (var joint in bodyResult.joints)
            {
                if (!joint.tracked)
                {
                    continue;
                }
                if (jointsMapping.ContainsKey((JointIndices)joint.index))
                {
                    var bone = jointsMapping[(JointIndices)joint.index];

                    availableJoints.Add(bone, joint);
                }
            }
            isSV2 = TofArBodyManager.Instance.BodyData.Data.frameDataSource == FrameDataSource.TofArSV2BodySkeleton;

            SetRootPosition(availableJoints, scale);

            foreach (var joint in bodyResult.joints)
            {
                if (jointsMapping.ContainsKey((JointIndices)joint.index))
                {
                    var bone = jointsMapping[(JointIndices)joint.index];

                    SetJoint(bone, joint);
                }
            }

            SetRootRotation(availableJoints);

            RootTransform.localScale = new Vector3(scale, scale, scale);
        }

        private void SetRootPosition(Dictionary<HumanBodyBones, HumanBodyJoint> availableJoints, float scale)
        {
            if (isSV2)
            {
                //SV2 doesn't detect hips
                if (!(availableJoints.ContainsKey(HumanBodyBones.Neck) && SetJoint(HumanBodyBones.Neck, availableJoints[HumanBodyBones.Neck])))
                {
                    RootTransform.localScale = new Vector3(scale, scale, scale);
                    return;
                }

                rootPosition = RootTransform.localPosition * (1 - nrValue) + this.transform.localRotation * (joints[HumanBodyBones.Neck].Target + this.heightVrm * Vector3.down) * nrValue;

            }
            else
            {
                //if (isFullbody)
                {
                    if (!(availableJoints.ContainsKey(HumanBodyBones.Hips) && SetJoint(HumanBodyBones.Hips, availableJoints[HumanBodyBones.Hips])))
                    {
                        RootTransform.localScale = new Vector3(scale, scale, scale);
                        return;
                    }

                    rootPosition = RootTransform.localPosition * (1 - nrValue) + this.transform.localRotation * joints[HumanBodyBones.Hips].Target * nrValue;
                }
                /*else
                {
                    if (!(availableJoints.ContainsKey(HumanBodyBones.Spine) && SetJoint(HumanBodyBones.Spine, availableJoints[HumanBodyBones.Spine])))
                    {
                        RootTransform.localScale = new Vector3(scale, scale, scale);
                        return;
                    }
                }*/
            }
        }

        private void SetRootRotation(Dictionary<HumanBodyBones, HumanBodyJoint> availableJoints)
        {
            if (isSV2)
            {
                bool b12 = (availableJoints.ContainsKey(HumanBodyBones.LeftUpperLeg));

                bool b9 = (availableJoints.ContainsKey(HumanBodyBones.RightUpperLeg));

                // For full body, determine body orientation from Hips and Spine
                if ((availableJoints.ContainsKey(HumanBodyBones.Neck) && b9 && b12))
                {
                    Vector3 vup = joints[HumanBodyBones.Neck].Target -
                        ((joints[HumanBodyBones.RightUpperLeg].Target + joints[HumanBodyBones.LeftUpperLeg].Target) / 2);
                    Vector3 vside = joints[HumanBodyBones.RightUpperLeg].Target - joints[HumanBodyBones.LeftUpperLeg].Target;
                    Vector3 forward = Vector3.Cross(vup, -vside);

                    rootRotation = Quaternion.LookRotation(forward, vup);
                    rootRotation = Quaternion.Slerp(RootTransform.localRotation, this.transform.localRotation * rootRotation, nrValue);
                }

            }
            else
            {
                //if (isFullbody)
                {
                    bool b12 = (availableJoints.ContainsKey(HumanBodyBones.LeftUpperLeg));

                    bool b9 = (availableJoints.ContainsKey(HumanBodyBones.RightUpperLeg));

                    // For full body, determine body orientation from Hips and Spine
                    if ((availableJoints.ContainsKey(HumanBodyBones.Spine) && b9 && b12))
                    {
                        Vector3 vup = joints[HumanBodyBones.Spine].Target - joints[HumanBodyBones.Hips].Target;
                        Vector3 vside = joints[HumanBodyBones.RightUpperLeg].Target - joints[HumanBodyBones.LeftUpperLeg].Target;
                        Vector3 forward = Vector3.Cross(vup, -vside);

                        rootRotation = Quaternion.LookRotation(forward, vup);
                        rootRotation = Quaternion.Slerp(RootTransform.localRotation, this.transform.localRotation * rootRotation, nrValue);
                    }
                } /*else {
                    // For upper body, determine body orientation from Shoulders and Spine
                    float distanceSpineToHip = 0.3f;

                    // Set Pelvis position
                    joints[HumanBodyBones.Hips].Target =
                        (joints[HumanBodyBones.Spine].Target - joints[HumanBodyBones.Neck].Target) * distanceSpineToHip
                        + joints[HumanBodyBones.Spine].Target;

                    rootPosition = RootTransform.localPosition * (1 - nrValue) + this.transform.localRotation * joints[HumanBodyBones.Hips].Target * nrValue;

                    Vector3 vup = joints[HumanBodyBones.Neck].Target - joints[HumanBodyBones.Spine].Target;
                    Vector3 vside = joints[HumanBodyBones.RightUpperArm].Target - joints[HumanBodyBones.LeftUpperArm].Target;
                    Vector3 forward = Vector3.Cross(vup, -vside);

                    rootRotation = Quaternion.LookRotation(forward, vup);
                    rootRotation = Quaternion.Slerp(RootTransform.localRotation, this.transform.localRotation * rootRotation, nrValue);
                }*/
            }
        }

        public void FixedUpdate()
        {
            if (rootPosition != Vector3.zero)
            {
                RootTransform.localPosition = rootPosition;
                RootTransform.localRotation = rootRotation;
            }
        }

        public void LateUpdate()
        {
            RootTransform.localScale = new Vector3(1, 1, 1);

            if (rootPosition != Vector3.zero)
            {
                RootTransform.localPosition = rootPosition;
                RootTransform.localRotation = rootRotation;
            }

            // Extend arm if end of the arm cannot be found
            if (!joints[HumanBodyBones.LeftHand].IsDetected)
            {
                joints[HumanBodyBones.LeftLowerArm].Trans.localRotation = joints[HumanBodyBones.LeftLowerArm].InitRotation;
            }
            if (!joints[HumanBodyBones.RightHand].IsDetected)
            {
                joints[HumanBodyBones.RightLowerArm].Trans.localRotation = joints[HumanBodyBones.RightLowerArm].InitRotation;
            }

            NrRotation(HumanBodyBones.RightUpperArm);
            NrRotation(HumanBodyBones.RightLowerArm);
            NrRotation(HumanBodyBones.LeftUpperArm);
            NrRotation(HumanBodyBones.LeftLowerArm);

            // Extend leg if the end of the leg cannot be found
            if (!joints[HumanBodyBones.LeftFoot].IsDetected)
            {
                joints[HumanBodyBones.LeftLowerLeg].Trans.localRotation = joints[HumanBodyBones.LeftLowerLeg].InitRotation;
            }
            if (!joints[HumanBodyBones.RightFoot].IsDetected)
            {
                joints[HumanBodyBones.RightLowerLeg].Trans.localRotation = joints[HumanBodyBones.RightLowerLeg].InitRotation;
            }

            NrRotation(HumanBodyBones.RightUpperLeg);
            NrRotation(HumanBodyBones.RightLowerLeg);
            NrRotation(HumanBodyBones.LeftUpperLeg);
            NrRotation(HumanBodyBones.LeftLowerLeg);

            this.widthReal = Mathf.Lerp(this.widthReal, ((joints[HumanBodyBones.Neck].Target - joints[HumanBodyBones.LeftUpperArm].Target).magnitude + (joints[HumanBodyBones.Neck].Target - joints[HumanBodyBones.RightUpperArm].Target).magnitude), 0.3f);
            // adjust height of vrm model to match real body height
            if (isSV2)
            {
                this.heightReal = Mathf.Lerp(this.heightReal, (joints[HumanBodyBones.Neck].Target -
                    ((joints[HumanBodyBones.RightUpperLeg].Target + joints[HumanBodyBones.LeftUpperLeg].Target) / 2)).magnitude, 0.3f);
            }
            else
            {
                this.heightReal = Mathf.Lerp(this.heightReal, ((joints[HumanBodyBones.Spine].Target - joints[HumanBodyBones.Hips].Target).magnitude + (joints[HumanBodyBones.Neck].Target - joints[HumanBodyBones.Spine].Target).magnitude), 0.3f);
            }

            float scale = ((this.heightReal / this.heightVrm) + (this.widthReal / this.widthVrm)) / 2;
            if (scale != 0)
            {
                RootTransform.localScale = new Vector3(scale, scale, scale);
            }

        }

        void NrRotation(HumanBodyBones bone)
        {
            Quaternion qOld = joints[bone].PreLocalRotation;
            Quaternion qNew = joints[bone].Trans.localRotation;

            Quaternion qd = qNew * Quaternion.Inverse(qOld);

            float c;
            Vector3 v;
            qd.ToAngleAxis(out c, out v);

            float maxDegreeSpeed = this.maxDegreeSpeed * Time.deltaTime;

            if (!joints[bone].IsDetected)
            {
                maxDegreeSpeed /= 2;
            }

            if (c > maxDegreeSpeed)
            {
                c = maxDegreeSpeed;
            }

            qd = Quaternion.AngleAxis(c, v);

            joints[bone].Trans.localRotation = qd * qOld;
            joints[bone].PreLocalRotation = joints[bone].Trans.localRotation;
        }

        private void Deform()
        {
            for (int j = 0; j < /*(this.isFullbody ? 2 : 1)*/ 2; j++)
            {
                bool useArm = j == 0;

                for (int i = 0; i < 2; i++)
                {
                    bool isLeft = i == 0;
                    var lower = useArm ? (isLeft ? joints[HumanBodyBones.LeftLowerArm] : joints[HumanBodyBones.RightLowerArm])
                        : (isLeft ? joints[HumanBodyBones.LeftLowerLeg] : joints[HumanBodyBones.RightLowerLeg]);
                    var upper = useArm ? (isLeft ? joints[HumanBodyBones.LeftUpperArm] : joints[HumanBodyBones.RightUpperArm])
                        : (isLeft ? joints[HumanBodyBones.LeftUpperLeg] : joints[HumanBodyBones.RightUpperLeg]);
                    var end = useArm ? (isLeft ? joints[HumanBodyBones.LeftHand] : joints[HumanBodyBones.RightHand])
                        : (isLeft ? joints[HumanBodyBones.LeftFoot] : joints[HumanBodyBones.RightFoot]);

                    Vector3 v1 = lower.Target - upper.Target;
                    Vector3 v2 = end.Target - lower.Target;

                    // Change distance to distance of model
                    float upperToLower = Vector3.Distance(upper.Trans.position, lower.Trans.position);
                    float lowerToEnd = Vector3.Distance(lower.Trans.position, end.Trans.position);
                    // Change distance to elbow (and shift the wrist/ankle position accordingly)
                    Vector3 posElbow = upper.Target + v1.normalized * upperToLower;
                    end.Target = end.Target + posElbow - lower.Target;
                    lower.Target = posElbow;
                    // Change distance to wrist/ankle
                    end.Target = lower.Target + v2.normalized * lowerToEnd;

                    // Align shoulders before setting to anim
                    Vector3 v3 = upper.Trans.position - Quaternion.Euler(0, 0, this.currentRotation) * upper.Target;

                    var ikHint = useArm ? (isLeft ? AvatarIKHint.LeftElbow : AvatarIKHint.RightElbow)
                        : (isLeft ? AvatarIKHint.LeftKnee : AvatarIKHint.RightKnee);
                    var ikEnd = useArm ? (isLeft ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand)
                        : (isLeft ? AvatarIKGoal.LeftFoot : AvatarIKGoal.RightFoot);

                    Vector3 hintLower = Quaternion.Euler(0, 0, this.currentRotation) * lower.Target + v3;
                    Vector3 hintEnd = Quaternion.Euler(0, 0, this.currentRotation) * end.Target + v3;

                    anim.SetIKHintPositionWeight(ikHint, 0.5f);
                    anim.SetIKHintPosition(ikHint, hintLower);
                    anim.SetIKPositionWeight(ikEnd, 1.0f);
                    anim.SetIKPosition(ikEnd, hintEnd);
                }
            }
        }

        public void OnAnimatorIK(int layerIndex)
        {
            // End if no recognition result (do not update)
            if (!isTracked)
            {
                return;
            }

            float scale = RootTransform.localScale.x;
            RootTransform.localScale = new Vector3(1, 1, 1);

            // Model arms and legs
            Deform();

            RootTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
