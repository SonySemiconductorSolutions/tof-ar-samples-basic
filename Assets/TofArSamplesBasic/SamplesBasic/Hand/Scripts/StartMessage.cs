/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0;
using TofAr.V0.Hand;
using UnityEngine;

namespace TofArSamples.Hand
{
    public class StartMessage : MonoBehaviour
    {
#if UNITY_ANDROID
        class PositiveButtonListner : AndroidJavaProxy
        {
            public PositiveButtonListner()
             : base("android.content.DialogInterface$OnClickListener") { }

            public void onClick(AndroidJavaObject obj, int value) { }
        }

        void Start()
        {
            TofArHandManager.OnReplacedModelFile += ShowAndroidDialog;
        }

        void OnDestroy()
        {
            TofArHandManager.OnReplacedModelFile -= ShowAndroidDialog;
        }

        /// <summary>
        /// Show Android dialog
        /// </summary>
        /// <param name="msg">Display text</param>
        void ShowAndroidDialog(string msg)
        {
            if ((Application.platform == RuntimePlatform.WindowsEditor) ||
                (Application.platform == RuntimePlatform.WindowsPlayer) ||
                (Application.platform == RuntimePlatform.OSXEditor) ||
                (Application.platform == RuntimePlatform.OSXPlayer))
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, msg);
                return;
            }

#if UNITY_EDITOR
            TofArManager.Logger.WriteLog(LogLevel.Debug, msg);
#else
            using (var unityPlayer = new AndroidJavaClass(
                "com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>(
                    "currentActivity"))
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        // Create an AlertDialog.Builder object
                        using (var alertDialogBuilder = new AndroidJavaObject(
                            "android/app/AlertDialog$Builder", activity))
                        {
                            // Call setMessage on the builder
                            alertDialogBuilder.Call<AndroidJavaObject>(
                                "setMessage", msg);

                            // Call setCancelable on the builder
                            alertDialogBuilder.Call<AndroidJavaObject>(
                                "setCancelable", true);

                            // Call setPositiveButton and set the message along with the listner
                            // Listner is a proxy class
                            alertDialogBuilder.Call<AndroidJavaObject>(
                                "setPositiveButton", "OK",
                                new PositiveButtonListner());

                            // Finally get the dialog instance and show it
                            var dialog = alertDialogBuilder.Call<AndroidJavaObject>("create");
                            dialog.Call("show");
                        }
                    }));
                }
            }
#endif
        }
#endif
    }
}
