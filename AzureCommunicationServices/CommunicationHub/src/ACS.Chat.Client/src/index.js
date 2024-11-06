// https://learn.microsoft.com/en-us/javascript/api/overview/azure/communication-chat-readme?view=azure-node-latest

import { ChatClient } from '@azure/communication-chat';
import { AzureCommunicationTokenCredential } from "@azure/communication-common";

const endpointUrl = "https://xxx.europe.communication.azure.com/";
// Replace with the ACS token generated from the portal or backend
const userAccessToken = "";
// Replace with the chat thread ID created in ACS
const threadId = "";

let chatClient, chatThreadClient;

async function initializeChatClient() {
    const tokenCredential = new AzureCommunicationTokenCredential(userAccessToken);
    chatClient = new ChatClient(endpointUrl, tokenCredential);
    chatThreadClient = await chatClient.getChatThreadClient(threadId);

    // open notifications channel
    await chatClient.startRealtimeNotifications();
    // subscribe to new notification
    chatClient.on("chatMessageReceived", (e) => {
        console.log("Notification chatMessageReceived!");
        // your code here
        displayMessage(e.senderDisplayName, e.message);
    });

    console.log("Chat client initialized.");
}

function displayMessage(sender, content) {
    const chatWindow = document.getElementById("chatWindow");
    const messageElement = document.createElement("p");
    messageElement.textContent = `${sender}: ${content}`;
    chatWindow.appendChild(messageElement);
    chatWindow.scrollTop = chatWindow.scrollHeight;
}

async function sendMessage() {
    if (!chatThreadClient) {
        console.error("Chat client is not initialized yet.");
        return;
    }

    const messageInput = document.getElementById("messageInput");
    const message = messageInput.value.trim();

    if (message) {
        await chatThreadClient.sendMessage({ content: message });
        displayMessage("You", message);
        messageInput.value = "";
    }
}

window.initializeChatClient = initializeChatClient;
window.sendMessage = sendMessage;
