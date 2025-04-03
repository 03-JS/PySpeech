import sys
import io
import speech_recognition as sr
import numpy as np
from faster_whisper import WhisperModel

# Change console encoding
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# Load Whisper model
# model_size = "tiny"  # Choose from: tiny, base, small, medium, large
model_size = sys.argv[2]  # Choose from: tiny, base, small, medium, large
model = WhisperModel(model_size, device="cpu", compute_type="int8")

# Initialize SpeechRecognition
recognizer = sr.Recognizer()
mic = sr.Microphone(sample_rate=16000)  # Whisper expects 16kHz audio

# Get language
language = "en"
multilingual = False
if len(sys.argv) > 1:
    multilingual = sys.argv[1] == ""
    if not multilingual:
        language = sys.argv[1]

with mic as source:
    # recognizer.adjust_for_ambient_noise(source)  # Reduce noise
    while True:
        audio = recognizer.listen(source)

        # Convert SpeechRecognition audio to NumPy array
        np_audio = np.frombuffer(audio.frame_data, dtype=np.int16).astype(np.float32) / 32768.0  # Normalize

        # Transcribe with Whisper
        segments, _ = model.transcribe(np_audio, language=language, multilingual=multilingual)

        # Print transcription
        for segment in segments:
            # confidence = 100 - (segment.avg_logprob * (-100))
            # confidence = np.clip(confidence, 0, 100)
            # data = {
            #     "recognized": segment.text,
            #     "confidence": float(confidence)
            # }
            # json_string = json.dumps(data, indent=4)
            # print(data)
            print(segment.text, flush=True)
            sys.stdout.flush()