// https://learn.microsoft.com/en-us/javascript/api/overview/azure/communication-chat-readme?view=azure-node-latest

import { ChatClient } from '@azure/communication-chat';
import { AzureCommunicationTokenCredential } from "@azure/communication-common";

const endpointUrl = "https://teamsskypehub.europe.communication.azure.com/";
// Replace with the ACS token generated from the portal or backend
const userAccessToken = "";
// Replace with the chat thread ID created in ACS
const threadId = "19:acsV1_FMzNYYbqdP1Yb-SQ420xdOcyQNRc5CiN9xDnMHjtSvM1@thread.v2";

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

        if(e.sender.communicationUserId === "8:acs:383d9c6a-d810-468d-ae7d-ae9fc100ca4c_00000023-ab0c-29fe-0586-af3a0d003caa") {
            displayMessage("WhatsApp User +***591", e.message);
        }
        else if(e.sender.communicationUserId === "8:acs:383d9c6a-d810-468d-ae7d-ae9fc100ca4c_00000023-ab22-8667-f5f4-ad3a0d001344") {
            displayMessage("Telegram User Karen T", e.message);
        }
        else if(e.sender.communicationUserId === "8:acs:383d9c6a-d810-468d-ae7d-ae9fc100ca4c_00000023-c5f1-9d98-65f0-ad3a0d0056ee") {
            displayMessage("Mattermost User john_smith", e.message);
        }
        else
        {
            displayMessage(e.senderDisplayName, e.message);
        }
    });

    console.log("Chat client initialized.");
}

function displayMessage(sender, content) {
    const chatWindow = document.getElementById("chatWindow");
    console.log("Displaying message: ", sender, content);
    const messageElement = document.createElement("p");
    const stringWithoutHtml = content.replace(/<\/?[^>]+(>|$)/g, "");
    messageElement.textContent = `${sender}: ${stringWithoutHtml}`;
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
