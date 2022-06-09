/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TofArSamples.Tof
{
    [System.Serializable]
    public class Vector2Event : UnityEvent<Vector2> { };

    /// <summary>
    /// Get touched point
    /// </summary>
    public class GetTouchedPoint : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField]
        private Vector2Event onTouchedInScreenSpace = null;

        [SerializeField]
        private Vector2Event onTouchedInWorldSpace = null;

        private RectTransform rect;

        void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        private void Touch(Vector2 touchedPoint)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            Vector2 origin = corners[1];
            Vector2 e0 = corners[2] - corners[1];
            Vector2 e1 = corners[0] - corners[1];

            var touchedX = Vector2.Dot(touchedPoint - origin, e0) / e0.sqrMagnitude;
            var touchedY = Vector2.Dot(touchedPoint - origin, e1) / e1.sqrMagnitude;

            if (touchedX < 0 || 1 < touchedX || touchedY < 0 || 1 < touchedY)
            {
                return;
            }

            if (onTouchedInWorldSpace != null)
            {
                onTouchedInWorldSpace.Invoke(touchedPoint);
            }

            var touchedPointInScreenSpace = new Vector2(touchedX, touchedY);
            if (onTouchedInScreenSpace != null)
            {
                onTouchedInScreenSpace.Invoke(touchedPointInScreenSpace);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Touch(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Touch(eventData.position);
        }
    }
}
