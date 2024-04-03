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
using UnityEngine.UI;

public class RemoveButtonHandler : MonoBehaviour
{
    public UnityEvent OnEnter, OnExit;

    public Sprite spriteOpen, spriteClose;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = spriteOpen;

        OnEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = spriteClose;

        OnExit.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
