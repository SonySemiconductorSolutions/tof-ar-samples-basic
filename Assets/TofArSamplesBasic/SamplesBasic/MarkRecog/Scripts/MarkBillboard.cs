/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.MarkRecog;
using UnityEngine;

namespace TofArSamples.MarkRecogComponent
{
    public class MarkBillboard : MonoBehaviour
    {
        public HandBrush brush;

        private Vector3 markTop, markLeft, markRight, markBottom, markFar;

        public Camera subCamera;

        void Start()
        {
            brush.DrawStarted += DrawStarted;
            brush.DrawStopped += DrawStopped;
            brush.DrawPointAdded += DrawPointAdded;

            GetComponent<Renderer>().enabled = false;
        }

        private void DrawStarted()
        {
            markTop = Vector3.one * -9999f;
            markBottom = Vector3.one * 9999f;
            markLeft = Vector3.one * 9999f;
            markRight = Vector3.one * -9999f;
            markFar = Vector3.one * -9999f;


            GetComponent<Renderer>().enabled = false;
        }

        private void DrawStopped()
        {
            GetComponent<Renderer>().enabled = true;

            // project points to screen
            var markTopScreen = subCamera.WorldToScreenPoint(markTop);
            var markBottomScreen = subCamera.WorldToScreenPoint(markBottom);
            var markLeftScreen = subCamera.WorldToScreenPoint(markLeft);
            var markRightScreen = subCamera.WorldToScreenPoint(markRight);

            float z = markFar.z;

            markTopScreen.z = markBottomScreen.z = markLeftScreen.z = markRightScreen.z = z;

            var markTopWorld = subCamera.ScreenToWorldPoint(markTopScreen);
            var markBottomWorld = subCamera.ScreenToWorldPoint(markBottomScreen);
            var markLeftWorld = subCamera.ScreenToWorldPoint(markLeftScreen);
            var markRightWorld = subCamera.ScreenToWorldPoint(markRightScreen);

            Vector3 pos = new Vector3();
            pos.x = (markLeftWorld.x + markRightWorld.x) / 2f;
            pos.y = (markTopWorld.y + markBottomWorld.y) / 2f;
            pos.z = z;

            float scaleH = markRightWorld.x - markLeftWorld.x;
            float scaleV = markTopWorld.y - markBottomWorld.y;

            Vector3 scale = new Vector3(scaleH, scaleV, 1f) * 1.2f;

            this.transform.localPosition = pos;
            this.transform.localScale = scale;
        }


        private void DrawPointAdded(Vector3 point)
        {
            if (point.x < markLeft.x) markLeft = point;
            if (point.x > markRight.x) markRight = point;
            if (point.y < markBottom.y) markBottom = point;
            if (point.y > markTop.y) markTop = point;
            if (point.z > markFar.z) markFar = point;

        }
    }

}
