/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Color;
using TofAr.V0.Tof;
using TofArSamples.Color;
using TofArSamples.Tof;
using TofArSettings;
using UnityEditor;
using UnityEngine;

namespace TofArSamples.ColorDepth
{
    public class ColorDepthMappingController : ControllerBase
    {
        /// <summary>
        /// Opacity of DepthView when superimposed
        /// </summary>
        public float Alpha = 0.5f;

        /// <summary>
        /// Overlay/Don't overlay Depth image on Color image
        /// </summary>
        bool remap = false;
        public bool Remap
        {
            get { return remap; }
            set
            {
                if (remap != value)
                {
                    remap = value;
                }
            }
        }

        [SerializeField]
        QuadFitter fitterColor;
        [SerializeField]
        QuadFitter fitterDepth;
        [SerializeField]
        TofFovAdjuster fovAdjuster;

        void OnEnable()
        {
            fovAdjuster.OnChangeFov += OnChangeFov;
        }

        void OnDisable()
        {
            fovAdjuster.OnChangeFov -= OnChangeFov;
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Event that is called when Camera FoV is changed
        /// </summary>
        /// <param name="fov">Camera FoV</param>
        /// <param name="aspect">Camera aspect ratio</param>
        void OnChangeFov(float fov, float aspect)
        {
            fitterColor.Fitting();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ColorDepthMappingController))]
        class ColorDepthMappingControllerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                Instance.Remap = EditorGUILayout.Toggle("Remap", Instance.Remap);
            }

            ColorDepthMappingController Instance
            {
                get { return target as ColorDepthMappingController; }
            }
        }
#endif

    }
}
