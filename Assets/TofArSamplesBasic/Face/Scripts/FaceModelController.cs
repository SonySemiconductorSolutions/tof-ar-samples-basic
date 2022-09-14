/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Face;
using UnityEngine;
using System.Collections;

namespace TofArSamples.Face
{
    public class FaceModelController : AbstractFaceModelController
    {
        public override bool IsShow
        {
            get
            {
                bool isShow = false;
                for (int i = 0; i < faceModels.Length; i++)
                {
                    if (faceModels[i].gameObject.activeSelf)
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
                    for (int i = 0; i < faceModels.Length; i++)
                    {
                        faceModels[i].gameObject.SetActive(value);
                    }

                    OnChangeShow?.Invoke(IsShow);
                }
            }
        }

        public override event ChangeToggleEvent OnChangeShow;

        public bool IsShowGaze
        {
            get
            {
                bool isShow = false;
                for (int i = 0; i < faceModels.Length; i++)
                {
                    if (faceModels[i].ShowGaze)
                    {
                        isShow = true;
                        break;
                    }
                }

                return isShow;
            }

            set
            {
                if (IsShowGaze != value)
                {
                    for (int i = 0; i < faceModels.Length; i++)
                    {
                        faceModels[i].ShowGaze = value;
                    }

                    OnChangeShowGaze?.Invoke(IsShowGaze);
                }
            }
        }

        public event ChangeToggleEvent OnChangeShowGaze;

        FaceModel[] faceModels;
        Transform faceTransform;

        private void OnEnable()
        {
            TofArFaceManager.OnStreamStarted += TofArFaceManager_OnStreamStarted;
        }

        private void OnDisable()
        {
            TofArFaceManager.OnStreamStarted -= TofArFaceManager_OnStreamStarted;
        }

        private void TofArFaceManager_OnStreamStarted(object sender)
        {
            if (TofArFaceManager.Instance.DetectorType != FaceDetectorType.Internal_ARKit)
            {
                IsShowGaze = false;
            }
        }

        protected override void Start()
        {
            faceTransform = GameObject.Find("FaceObjects")?.transform;
            if (faceTransform != null)
            {
                faceModels = faceTransform.GetComponentsInChildren<FaceModel>(true);
            }

            if (faceModels == null)
            {
                faceModels = FindObjectsOfType<FaceModel>(true);
            }

            reorientSocket = FindObjectOfType<TofAr.V0.Tof.ReorientRelativeColorCamera>();

            base.Start();
        }

        protected override void ApplyOffset()
        {
            if (faceTransform != null)
            {
                Vector3 offsetAdjusted = Offset;

                if (reorientSocket != null && reorientSocket.enabled && reorientSocket.enableRotateInWorld && faceTransform.IsChildOf(reorientSocket.transform))
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
                
                faceTransform.localPosition = offsetAdjusted;
            }
        }

        protected override void ApplyScaling()
        {
            for (int i = 0; i < faceModels.Length; i++)
            {
                faceModels[i].transform.localScale = Vector3.one * Scale;
            }
        }

    }
}
