/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using TofArSettings;
using UnityEngine;

namespace TofArSamples.Mesh
{
    public class OcclusionObjectController : ControllerBase
    {

        [SerializeField]
        GameObject occlusionCube, occlusionPlane;

        public event ChangeToggleEvent OnChangeCube, OnChangePlane;

        public bool IsCube
        {
            get => occlusionCube.activeInHierarchy;
            set
            {
                if(value != IsCube)
                {
                    occlusionCube.SetActive(value);
                    OnChangeCube?.Invoke(value);
                }
            }
        }

        public bool IsPlane
        {
            get => occlusionPlane.activeInHierarchy;
            set
            {
                if (value != IsPlane)
                {
                    occlusionPlane.SetActive(value);
                    OnChangePlane?.Invoke(value);
                }
            }
        }
    }
}
