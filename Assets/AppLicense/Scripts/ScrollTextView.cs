/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TofAr.AppLicense
{
    public class ScrollTextView : MonoBehaviour
    {
        public Text title;
        public GameObject textPrefab;
        public GameObject textParent;

        public void SetText(string title, string text)
        {
            ResetText();

            this.title.text = title;

            string[] textSplits = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            int count = 0;
            string mixText = string.Empty;
            foreach (string textSplit in textSplits)
            {
                if (count == 0)
                {
                    mixText = textSplit;
                }
                else
                {
                    mixText = mixText + "\n" + textSplit;
                }

                count++;

                if (count == 10)
                {
                    SetTextPrefab(mixText);

                    count = 0;
                    mixText = string.Empty;
                }
            }

            if (count > 0)
            {
                SetTextPrefab(mixText);
            }
        }

        private void SetTextPrefab(string text)
        {
            GameObject i_text = GameObject.Instantiate(textPrefab, textParent.transform);
            i_text.GetComponent<Text>().text = text;
        }

        public void CloseButton()
        {
            Destroy(this.gameObject);
        }

        private void ResetText()
        {
            foreach (Transform text in textParent.transform)
            {
                Destroy(text.gameObject);
            }
        }

        private string[] GetTextSplit(string str, int count)
        {
            var list = new List<string>();
            int length = (int)Math.Ceiling((double)str.Length / count);

            for (int i = 0; i < length; i++)
            {
                int start = count * i;
                if (str.Length <= start)
                {
                    break;
                }
                if (str.Length < start + count)
                {
                    list.Add(str.Substring(start));
                }
                else
                {
                    list.Add(str.Substring(start, count));
                }
            }

            return list.ToArray();
        }
    }
}
