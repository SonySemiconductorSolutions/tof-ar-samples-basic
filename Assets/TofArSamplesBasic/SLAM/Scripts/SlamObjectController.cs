/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text;
using TofArSettings;
using UnityEngine;

namespace TofArSamples.Slam
{
    public class SlamObjectController : ControllerBase
    {
        [SerializeField]
        GameObject cube, plane;

        [SerializeField]
        private Vector3 cubeSpawnOffset;

        public event ChangeToggleEvent OnChangeCube, OnChangePlane;

        public bool IsCube
        {
            get => cube.activeInHierarchy;
            set
            {
                if(value != IsCube)
                {
                    if(value)
                    {
                        cube.transform.position = Camera.main.transform.TransformPoint(cubeSpawnOffset);
                    }
                    cube.SetActive(value);
                    OnChangeCube?.Invoke(value);
                }
            }
        }

        public bool IsPlane
        {
            get => plane.activeInHierarchy;
            set
            {
                if (value != IsPlane)
                {
                    plane.SetActive(value);
                    OnChangePlane?.Invoke(value);
                }
            }
        }
    }
}
