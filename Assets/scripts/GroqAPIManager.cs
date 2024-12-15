using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GroqAPIManager : MonoBehaviour
{
    public AvatarController avatarController; // Reference to control NPC animations
    public ChatUIManager chatUIManager;       // Reference to update chat display
    public string apiUrl = "https://api.groq.com/openai/v1/chat/completions"; // Groq API endpoint
    public string apiKey = "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";    // Replace with your Groq API key
    private List<GroqMessage> conversationHistory = new List<GroqMessage>(); // Stores the conversation history
    public void SendMessageToAPI(string userMessage)
    {
        // Add the user message to the conversation history
        conversationHistory.Add(new GroqMessage { role = "user", content = userMessage });
        StartCoroutine(PostMessageToAPI());
    }

    private void Start()
    {

        if (chatUIManager == null)
            chatUIManager = FindAnyObjectByType<ChatUIManager>();
        if (avatarController == null)
            avatarController = FindAnyObjectByType<AvatarController>();
    }


    public void StartNewChat()
    {
        // Clear conversation history when starting a new chat
        conversationHistory.Clear();
        // Add a system message for initial instructions (optional)
        ResetConversation();
    }

    private IEnumerator PostMessageToAPI()
    {
        // Set NPC to Thinking animation
        avatarController.SetThinking();

        // Create JSON payload with the entire conversation history
        string jsonData = JsonUtility.ToJson(new GroqRequest(conversationHistory));

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response
            string responseText = request.downloadHandler.text;
            GroqResponse response = JsonUtility.FromJson<GroqResponse>(responseText);

            if (response != null && response.choices.Length > 0)
            {
                string npcResponse = response.choices[0].message.content;

                // Add the assistant's response to the conversation history
                conversationHistory.Add(new GroqMessage { role = "assistant", content = npcResponse });

                // Display NPC's response in the chat
                chatUIManager.DisplayNPCResponse(npcResponse);
            }
        }
        else
        {
            Debug.LogError("API Error: " + request.error);
            chatUIManager.DisplayNPCResponse("I'm having trouble responding. Please try again.");
        }

        // Set NPC back to Idle animation
    }

    public void ResetConversation()
    {


        // Add a system message to define the NPC's role or context
        conversationHistory.Add(new GroqMessage
        {
            role = "system",
            content = "Hi"
        });
    }
}

// Helper Classes for API Requests and Responses
[System.Serializable]
public class GroqRequest
{
    public string model = "llama3-8b-8192";
    public List<GroqMessage> messages; // Dynamic list of messages

    public GroqRequest(List<GroqMessage> conversation)
    {
        messages = conversation;
    }
}

[System.Serializable]
public class GroqMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class GroqResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}
