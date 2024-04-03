/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using TofAr.V0.Hand;
using TofArSettings;
using UnityEngine;

namespace TofArSamples.Hand
{
    public abstract class HandModelController : ControllerBase
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

        public virtual event ChangeToggleEvent OnChangeShow;

        public event ChangeVectorEvent OnChangeOffset;

        public event ChangeValueEvent OnChangeScale;

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

        protected StringBuilder sb = new StringBuilder();

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
        /// Get coordinate data of hand as string
        /// </summary>
        /// <param name="lrHand">Hand direction</param>
        /// <param name="localOrWorld">Relative coordinates from camera/Unity world coordinates</param>
        /// <returns>string</returns>
        public string GetPointsDataText(HandStatus lrHand, bool localOrWorld)
        {
            var hm = GetHandModel(lrHand);
            var points = (localOrWorld) ? hm.HandPoints : hm.WorldHandPoints;
            if (points == null)
            {
                return string.Empty;
            }

            sb.Clear();
            for (int i = 0; i < points.Length; i++)
            {
                sb.Append(points[i]);
                if (i < points.Length - 1)
                {
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get HandModel
        /// </summary>
        /// <param name="lrHand">Hand direction</param>
        /// <returns>HandModel</returns>
        protected virtual IHandModel GetHandModel(HandStatus lrHand)
        {
            return null;
        }
        /// <summary>
        /// Apply offset
        /// </summary>
        protected virtual void ApplyOffset()
        {
        }

        /// <summary>
        /// Apply scaling
        /// </summary>
        protected virtual void ApplyScaling()
        {
        }

        /// <summary>
        /// Check if offset is within the settable range
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
