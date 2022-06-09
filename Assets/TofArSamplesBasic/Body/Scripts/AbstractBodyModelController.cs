/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using TofAr.V0.Body;
using TofArSettings;
using UnityEngine;

namespace TofArSamples.Body
{
    public abstract class AbstractBodyModelController : ControllerBase
    {
        protected bool isShow = true;
        public virtual bool IsShow
        {
            get { return isShow; }
            set
            {
                if (isShow != value)
                {
                    isShow = value;
                    OnChangeShow?.Invoke(IsShow);
                }
            }
        }

        public static Vector3 OffsetDefault = Vector3.zero;

        Vector3 offset = Vector3.zero;
        public Vector3 Offset
        {
            get { return offset; }
            set
            {
                if (offset != value && CheckOffsetRange(value))
                {
                    offset = value;
                    ApplyOffset();

                    OnChangeOffset?.Invoke(Offset);
                }
            }
        }

        public const float OffsetMin = -1;
        public const float OffsetMax = 1;
        public const float OffsetStep = 0.005f;

        float scale = ScaleDefault;

        public float Scale
        {
            get { return scale; }
            set
            {
                if (scale != value && ScaleMin <= value && value <= ScaleMax)
                {
                    scale = value;
                    ApplyScaling();

                    OnChangeScale?.Invoke(Scale);
                }
            }
        }

        public const float ScaleMin = 0.25f;
        public const float ScaleMax = 4;
        public const float ScaleStep = 0.05f;
        public const float ScaleDefault = 1;

        /// <summary>
        /// Event that is called when display is toggled
        /// </summary>
        public virtual event ChangeToggleEvent OnChangeShow;

        public event ChangeVectorEvent OnChangeOffset;

        public event ChangeValueEvent OnChangeScale;

        protected StringBuilder sb = new StringBuilder();

        protected TofAr.V0.Tof.ReorientRelativeColorCamera reorientSocket;

        protected Matrix4x4 adjustPortrait = new Matrix4x4()
        {
            m01 = 1,
            m10 = -1,
            m22 = 1,
            m33 = 1
        };

        protected Matrix4x4 adjustPortraitUpsideDown = new Matrix4x4()
        {
            m01 = -1,
            m10 = 1,
            m22 = 1,
            m33 = 1
        };


        protected Matrix4x4 adjustLandscapeRight = new Matrix4x4()
        {
            m00 = -1,
            m11 = -1,
            m22 = 1,
            m33 = 1
        };

        protected abstract void ApplyOffset();

        protected abstract void ApplyScaling();

        private void OnEnable()
        {
            TofAr.V0.TofArManager.OnScreenOrientationUpdated += OnScreenOrientationUpdated;
        }

        private void OnDisable()
        {
            TofAr.V0.TofArManager.OnScreenOrientationUpdated -= OnScreenOrientationUpdated;
        }

        private void OnScreenOrientationUpdated(ScreenOrientation previousScreenOrientation, ScreenOrientation newScreenOrientation)
        {
            ApplyOffset();
        }

        /// <summary>
        /// Check if the offset value is within the settable range
        /// </summary>
        /// <param name="newVal">Offset</param>
        /// <returns>True/False</returns>
        bool CheckOffsetRange(Vector3 newVal)
        {
            bool result = true;
            for (int i = 0; i < 3; i++)
            {
                var xyz = newVal[i];
                if (xyz < OffsetMin || OffsetMax < xyz)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}
