using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class TTSManager : MonoBehaviour
{
    private const string apiUrl = "https://api.play.ht/api/v2/tts/stream";
    private const string authorization = "b40c0c270ce64ab09209a2512fcf5f4e"; // Replace with your AUTHORIZATION key
    private const string userId = "ufvi6r55uNYhURYnaG4gNv7UGc13"; // Replace with your X-USER-ID

    public AudioSource audioSource; // Assign an AudioSource in the Inspector
    public string audioSavePath = "GeneratedAudio.mp3"; // Path to save the audio file

    public void GenerateAndPlayAudio(string textToSpeak)
    {
        StartCoroutine(PostTTSRequest(textToSpeak));
    }

    private IEnumerator PostTTSRequest(string text)
    {
        Debug.Log(text);

        // Prepare JSON payload
        TTSRequestPayload payload = new TTSRequestPayload
        {
            voice = "s3://voice-cloning-zero-shot/d82d246c-148b-457f-9668-37b789520891/adolfosaad/manifest.json",
            output_format = "mp3",
            text = text
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("JSON Payload: " + jsonData);

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("content-type", "application/json");
        request.SetRequestHeader("AUTHORIZATION", authorization);
        request.SetRequestHeader("X-USER-ID", userId);

        // Send request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("TTS request successful! Downloading MP3 stream...");
            byte[] audioData = request.downloadHandler.data; // Get raw audio data
            SaveAudioToFile(audioData, audioSavePath);
            StartCoroutine(PlayAudioFromFile(audioSavePath));
        }
        else
        {
            Debug.LogError("TTS request failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    private void SaveAudioToFile(byte[] audioData, string filePath)
    {
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogError("Invalid audio data received.");
            return;
        }

        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        File.WriteAllBytes(fullPath, audioData);
        Debug.Log("Audio saved to: " + fullPath);
    }

    private IEnumerator PlayAudioFromFile(string filePath)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        Debug.Log("Attempting to play audio from: " + fullPath);

        if (!File.Exists(fullPath))
        {
            Debug.LogError("Audio file not found: " + fullPath);
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + fullPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log("Audio playing...");
                }
                else
                {
                    Debug.LogError("Failed to load audio clip from file.");
                }
            }
            else
            {
                Debug.LogError("Failed to play audio from file: " + www.error);
            }
        }
    }
}

[System.Serializable]
public class TTSRequestPayload
{
    public string voice;
    public string output_format;
    public string text;
}
