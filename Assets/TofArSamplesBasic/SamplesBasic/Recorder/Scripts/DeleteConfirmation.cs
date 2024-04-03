/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TofArSamples.Recorder
{
    public class DeleteConfirmation : MonoBehaviour
    {
        [SerializeField]
        private Button okButton;
        [SerializeField]
        private Button cancelButton;
        [SerializeField]
        private Text messageText;
        [SerializeField]
        private TofArSettings.UI.ToolButton toolButton;

        public string message
        {
            set
            {
                messageText.text = value;
            }
        }

        public UnityAction okAction
        {
            set
            {
                okButton.onClick.AddListener(value);
            }
        }

        public UnityAction cancelAction
        {
            set
            {
                cancelButton.onClick.AddListener(value);
            }
        }

        private void Start()
        {
            Hide();
            cancelButton.onClick.AddListener(Hide);
            okButton.onClick.AddListener(Hide);
            toolButton.OnClick += ((bool onOff) =>
            {
                this.gameObject.SetActive(onOff);
            });
        }

        private void Hide()
        {
            this.gameObject.SetActive(false);
            toolButton.OnOff = false;
        }
    }
}
