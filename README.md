# PySpeech

A mod that uses C# and Python to allow developers to use OpenAI's Whisper in Lethal Company for automatic speech recognition.

# What does this do?

This is an API that other developers can use to build their own features, such as adding voice commands or real time speech transcription.
Clients can choose the model they want to be used as well as the language to recognize in the config.

# How does this work?

Essentially, when the game starts it runs a Python script on a separate thread to capture and feed microphone audio to whatever model the player chose. The output of the script is then captured by the mod, which has a couple of methods other developers can use to access that data and add their own features.

If you'd like to know how you can implement PySpeech in your project head to the [developer guide](https://github.com/03-JS/PySpeech/wiki/Developer-guide).
