using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    public TMP_InputField chatInputField;  // Input field for user messages
    public Button sendButton;         // Send button
    public Text chatText;             // Display for chat
    public GroqAPIManager groqAPIManager; // Reference to GroqAPIManager
    public float typingSpeed = 0.05f; // Speed of typing effect

    public TTSManager ttsManager;

    private List<string> conversationHistory = new List<string>(); // Stores conversation history

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendMessage);
        if (groqAPIManager == null)
            groqAPIManager = FindAnyObjectByType<GroqAPIManager>();
        if (ttsManager == null)
            ttsManager = FindAnyObjectByType<TTSManager>();
    }

    private void OnSendMessage()
    {
        string userMessage = chatInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            // Add user message to history
            conversationHistory.Add("You> " + userMessage);

            // Display the updated chat
            UpdateChatDisplay();

            // Clear the input field
            chatInputField.text = "";

            // Send the message to the API Manager
            groqAPIManager.SendMessageToAPI(userMessage);
        }
    }

    public void DisplayNPCResponse(string npcResponse)
    {
        // Add the NPC's response to conversation history
        conversationHistory.Add("Ai> " + npcResponse);

        // Start typing effect for the NPC's response
        StartCoroutine(TypeText("Ai> " + npcResponse));
        ttsManager.GenerateAndPlayAudio(npcResponse);

    }

    private IEnumerator TypeText(string fullText)
    {
        // Add text letter by letter
        foreach (char c in fullText)
        {
            chatText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        chatText.text += "\n"; // Add a newline after the full message
    }

    private void UpdateChatDisplay()
    {
        // Clear the chat display
        chatText.text = "";

        // Re-display the entire conversation
        foreach (string message in conversationHistory)
        {
            chatText.text += message + "\n";
        }
    }

    public void ClearChatHistory()
    {
        // Clear memory and update the display
        conversationHistory.Clear();
        groqAPIManager.StartNewChat();
        chatText.text = "";

    }
}
