using System;
using UnityEngine;

public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] wavFile, string clipName = "GeneratedAudio")
    {
        try
        {
            // WAV header information
            int channels = wavFile[22]; // Mono = 1, Stereo = 2
            int sampleRate = BitConverter.ToInt32(wavFile, 24);
            int byteRate = BitConverter.ToInt32(wavFile, 28);
            int blockAlign = wavFile[32];
            int bitsPerSample = wavFile[34];

            // Start of data
            int dataStartIndex = Array.IndexOf(wavFile, (byte)'d', 36) + 4;
            int dataSize = BitConverter.ToInt32(wavFile, dataStartIndex - 4);

            // Audio data
            float[] audioData = new float[dataSize / (bitsPerSample / 8)];
            for (int i = 0; i < audioData.Length; i++)
            {
                short sample = BitConverter.ToInt16(wavFile, dataStartIndex + i * 2);
                audioData[i] = sample / 32768f; // Convert to -1.0 to 1.0
            }

            // Create AudioClip
            AudioClip clip = AudioClip.Create(clipName, audioData.Length, channels, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
        catch (Exception e)
        {
            Debug.LogError("WAV conversion error: " + e.Message);
            return null;
        }
    }
}
