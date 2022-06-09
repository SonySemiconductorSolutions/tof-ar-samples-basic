/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofArCustom
{
    public class URPHandAlphaController : MonoBehaviour
    {
        public string PropertyName = "_BaseColor";

        private SkinnedMeshRenderer[] renderers;
        private Transform handRoot;
        private TofAr.V0.Hand.AbstractHandModel hbr;

        [SerializeField]
        private float alphaStartDistance = 0.3f;
        [SerializeField]
        private float alphaEndDistance = 0.1f;

        [SerializeField]
        private float dist;

        private Color[][] colorOne;
        private Color[][] colorZero;
        private int[] matCount;

        private int propID;
        private bool[] hasColorProperty;

        [SerializeField]
        private float currentAlpha;
        [SerializeField]
        private float currentMultiply;

        private int handCounter = 0;
        public int fadeTerm = 15;
        private bool fade = true;

        void Start()
        {
            hbr = GetComponent<TofAr.V0.Hand.AbstractHandModel>();
            handRoot = transform.GetChild(0);

            // common over renderers...
            propID = Shader.PropertyToID(PropertyName);

            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            matCount = new int[renderers.Length];
            colorOne = new Color[renderers.Length][];
            colorZero = new Color[renderers.Length][];
            hasColorProperty = new bool[renderers.Length];

            for (int j = 0; j < renderers.Length; j++)
            {
                matCount[j] = renderers[j].materials.Length;
                colorOne[j] = new Color[matCount[j]];
                colorZero[j] = new Color[matCount[j]];

                currentMultiply = 1;

                for (int i = 0; i < matCount[j]; i++)
                {
                    SetupMaterialWithBlendMode(renderers[j].materials[i], true);
                }

                hasColorProperty[j] = true;

                for (int i = 0; i < matCount[j]; i++)
                {
                    if (!renderers[j].materials[0].HasProperty(propID))
                    {
                        hasColorProperty[j] = false;
                        break;
                    }
                    colorOne[j][i] = renderers[j].materials[i].GetColor(propID);
                    colorZero[j][i] = new Color(colorOne[j][i].r, colorOne[j][i].g, colorOne[j][i].b, 0);
                }
            }
        }

        void Update()
        {
            if (hbr.IsHandDetected)
            {
                if (handCounter < fadeTerm)
                {
                    handCounter++;
                }
            }
            else
            {
                if (handCounter > 0)
                {
                    handCounter--;
                }
            }
            currentMultiply = (float)handCounter / fadeTerm;

            dist = Vector3.Distance(handRoot.position, transform.position);

            if (dist > alphaStartDistance)
            {
                currentAlpha = 1;
            }
            else if (dist < alphaEndDistance)
            {
                currentAlpha = 0;
            }
            else
            {
                currentAlpha = ratioInMinMax(dist, alphaEndDistance, alphaStartDistance);
            }
            setAlpha(currentAlpha * currentMultiply);
        }

        private static float ratioInMinMax(float value, float min, float max)
        {
            if (max > min)
            {
                return (value - min) / (max - min);
            }
            return (value - max) / (min - max);
        }

        private void setAlpha(float value)
        {
            bool isFade = (value < 1);

            bool fadeStatusChange = (fade != isFade);
            fade = isFade;

            for (int j = 0; j < renderers.Length; j++)
            {
                if (hasColorProperty[j])
                {
                    for (int i = 0; i < matCount[j]; i++)
                    {
                        if (fadeStatusChange)
                        {
                            SetupMaterialWithBlendMode(renderers[j].materials[i], isFade);
                        }
                        renderers[j].materials[i].SetColor(propID, Color.Lerp(colorZero[j][i], colorOne[j][i], value));
                    }
                }
                else
                {
                    for (int i = 0; i < matCount[j]; i++)
                    {
                        renderers[j].gameObject.SetActive((value > 0.5f));
                    }
                }
            }
        }
        /// <summary>
        /// Change the BlendMode of the material
        /// </summary>
        /// <param name="m">Material</param>
        /// <param name="isFade">Transparent/not transparent</param>
        public void SetupMaterialWithBlendMode(Material m, bool isFade)
        {
            TofArCustom.URPMaterialChanger.ChangeTrans(m, isFade);
        }
    }
}
