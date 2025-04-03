using HarmonyLib;
using PySpeech.Util;
using System.Diagnostics;
using UnityEngine;

namespace PySpeech.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static async void SetupRecognitionEngine()
        {
            await Engine.Start();
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnApplicationQuit")]
        static void KillPythonProcess()
        {
            Process[] processes = Process.GetProcessesByName("pyspeech");
            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}
