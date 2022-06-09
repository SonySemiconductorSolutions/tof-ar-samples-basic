/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArSamples.Startup
{
    public static class Utils
    {
        /// <summary>
        /// Get the number of fingers touching the screen
        /// </summary>
        /// <returns>Number of fingers touching the screen</returns>
        public static int GetTouchCount()
        {
            var result = 0;
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase != TouchPhase.Ended &&
                    touch.phase != TouchPhase.Canceled)
                {
                    result++;
                }
            }

            return result;
        }
    }
}
