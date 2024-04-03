/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TofArSamples.Plane
{
    [System.Serializable]
    public class Vector2Event : UnityEvent<Vector2> { };

    public class GetTouchedPoint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField]
        public Vector2Event onTouched, OnDragging, OnDragFinish;

        [SerializeField]
        public UnityEvent OnDragStart;

        private RectTransform rect;

        private bool isDown = false;

        void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        private Vector2 NormalizeCoordinates(Vector2 point)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            Vector2 origin = corners[1];
            Vector2 e0 = corners[2] - corners[1];
            Vector2 e1 = corners[0] - corners[1];

            var touchedX = Vector2.Dot(point - origin, e0) / e0.sqrMagnitude;
            var touchedY = Vector2.Dot(point - origin, e1) / e1.sqrMagnitude;

            return new Vector2(touchedX, touchedY);
        }

        private void Touch(Vector2 touchedPoint)
        {
            var touchedPointInScreenSpace = NormalizeCoordinates(touchedPoint);

            if (touchedPointInScreenSpace.x < 0 || 1 < touchedPointInScreenSpace.x
                || touchedPointInScreenSpace.y < 0 || 1 < touchedPointInScreenSpace.y)
            {
                return;
            }

            isDown = true;

            if (onTouched != null)
            {
                onTouched.Invoke(touchedPointInScreenSpace);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Touch(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (OnDragStart != null && isDown)
            {
                OnDragStart.Invoke();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var touchedPointInScreenSpace = NormalizeCoordinates(eventData.position);

            /*if (touchedPointInScreenSpace.x < 0 || 1 < touchedPointInScreenSpace.x
                || touchedPointInScreenSpace.y < 0 || 1 < touchedPointInScreenSpace.y)
            {
                return;
            }*/


            OnDragFinish.Invoke(touchedPointInScreenSpace);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragging.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
        }
    }
}

