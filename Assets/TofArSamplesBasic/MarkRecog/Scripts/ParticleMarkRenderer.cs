/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.MarkRecog;
using UnityEngine;

namespace TofArSamples.MarkRecog
{
    public class ParticleMarkRenderer : MonoBehaviour, IMarkRenderer
    {
        private Vector3 lastPosition;

        private ParticleSystem particle;

        public void StartDrawing()
        {
            particle.Clear();
            particle.Play();
        }

        public void StopDrawing()
        {
            particle.Stop();
        }

        /// <summary>
        /// Drawing
        /// </summary>
        /// <param name="position">position</param>
        public void UpdateDrawing(Vector3 position)
        {
            particle.transform.localPosition = position;

            Vector3 velocity = position - lastPosition;
            if (velocity.magnitude < 0.001f)
            {
                // TODO reduce emission
            }

            lastPosition = position;
        }

        void Start()
        {
            particle = GetComponent<ParticleSystem>();
        }

    }
}
