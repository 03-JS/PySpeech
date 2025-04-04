using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PySpeech.Patches;
using PySpeech.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PySpeech
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.PySpeech";
        private const string modName = "PySpeech";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        public static Plugin Instance;
        internal static ManualLogSource mls;

        // Config
        public static ConfigEntry<bool> logging;
        public static ConfigEntry<Languages> language;
        public static ConfigEntry<Models> model;

        void Awake()
        {
            if (Instance == null) Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            logging = Config.Bind(
                "General", // Config section
                "Show logs", // Key of this config
                true, // Default value
                "Shows the speech recognition output" // Description
            );

            model = Config.Bind(
                "General", // Config section
                "Model", // Key of this config
                Models.Tiny, // Default value
                "Whisper model to be used for speech recognition. You should use what the mod developers recommend.\n" +
                "\nTiny: low delay, lower accuracy" +
                "\nBase: medium delay, higher accuracy" +
                "\nSmall: high delay, very high accuracy" // Description
            );
            model.SettingChanged += async (obj, args) =>
            {
                await Engine.Restart();
            };

            language = Config.Bind(
                "General", // Config section
                "Language", // Key of this config
                Languages.English, // Default value
                "Language to be used for speech recognition" // Description
            );
            language.SettingChanged += async (obj, args) =>
            {
                await Engine.Restart();
            };

            Speech.phrases = new List<string>();

            harmony.PatchAll(typeof(GameNetworkManagerPatch));
        }
    }
}
