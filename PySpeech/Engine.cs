using PySpeech.Util;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace PySpeech
{
    internal class Engine
    {
        public static event EventHandler<SpeechEventArgs> SpeechRecognized;
        private static string[] models = { "tiny", "base", "small" };

        internal static async Task Start()
        {
            GameObject dispatcher = new GameObject("PySpeech Dispatcher");
            dispatcher.AddComponent<UnityMainThreadDispatcher>();
            UnityEngine.Object.DontDestroyOnLoad(dispatcher);

            Plugin.mls.LogInfo("Starting Speech Recognition engine");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Plugin.Instance.Info.Location.TrimEnd("PySpeech.dll".ToCharArray()) + "pyexec/pyspeech.exe",
                Arguments = $"\"{Speech.languages[(int)Plugin.language.Value]}\" \"{models[(int)Plugin.model.Value]}\"", // Pass the language as an argument
                RedirectStandardOutput = true, // Capture output
                RedirectStandardError = true, // Capture errors
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process pyProcess = new Process { StartInfo = psi, EnableRaisingEvents = true })
            {
                pyProcess.OutputDataReceived += (sender, args) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            string recognized = args.Data.TrimStart(' ');
                            if (Plugin.logging.Value) Plugin.mls.LogDebug($"Recognized: {recognized}");
                            if (Speech.phrases.Count > 0) Speech.GetBestMatch(recognized);
                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                SpeechRecognized?.Invoke(Plugin.Instance, new SpeechEventArgs(recognized));
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Plugin.mls.LogError(e.Message + e.StackTrace);
                    }
                };

                pyProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data)) Plugin.mls.LogError($"Python Error: {args.Data}");
                };

                pyProcess.Start();
                pyProcess.BeginOutputReadLine();
                pyProcess.BeginErrorReadLine();

                // Wait for the app to close
                await Task.Run(() => pyProcess.WaitForExit());
            }
        }

        internal static async Task Restart()
        {
            Process[] processes = Process.GetProcessesByName("pyspeech");
            foreach (var process in processes)
            {
                process.Kill();
            }

            await Start();
        }
    }
}
