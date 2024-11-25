// https://learn.microsoft.com/en-us/javascript/api/overview/azure/communication-chat-readme?view=azure-node-latest

import { ChatClient } from '@azure/communication-chat';
import { AzureCommunicationTokenCredential } from "@azure/communication-common";

const endpointUrl = "https://teamsskypehub.europe.communication.azure.com/";
// Replace with the ACS token generated from the portal or backend
const userAccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjU3Qjg2NEUwQjM0QUQ0RDQyRTM3OTRBRTAyNTAwRDVBNTE5MjA1RjUiLCJ4NXQiOiJWN2hrNExOSzFOUXVONVN1QWxBTldsR1NCZlUiLCJ0eXAiOiJKV1QifQ.eyJza3lwZWlkIjoiYWNzOjM4M2Q5YzZhLWQ4MTAtNDY4ZC1hZTdkLWFlOWZjMTAwY2E0Y18wMDAwMDAyMy1hYWNjLTE1MWQtYzE4Ny1hZjNhMGQwMDI5YzkiLCJzY3AiOjE3OTIsImNzaSI6IjE3MzIxNzIxOTkiLCJleHAiOjE3MzIyNTg1OTksInJnbiI6ImVtZWEiLCJhY3NTY29wZSI6InZvaXAsY2hhdCIsInJlc291cmNlSWQiOiIzODNkOWM2YS1kODEwLTQ2OGQtYWU3ZC1hZTlmYzEwMGNhNGMiLCJyZXNvdXJjZUxvY2F0aW9uIjoiZXVyb3BlIiwiaWF0IjoxNzMyMTcyMTk5fQ.MefkPVYltJ5MadsPDvjQkyNEaT3sy3-DZcqqNJM7gm_Fy5-7j8645ouMEAGBpPgCY9VjLKveEW0CoKCx595xHaIWA5Ctq0y9dHUIKCUP1rtgtuKJVtLwnf8aOXZ742K4ZNEmRKiI1rXScB9G5OlScjOXzvo2z-CSBVuqtP0wLBh5omyqI7ATYzzWPcDJYXONY8IoOCxdUtuKRmGdU19llsRZ2Y7l7IMOaAJTd4QiekJ_j8W00HP5X1OqAVfsI2FAFWFA9KFgR8NRmUwJduUCyUQ7EcIBF6RGHh2KR_HmKh8LReUf3xM_1c-0kQeRWoEHQMFSTpY1vA6a4SwGl6KLvw";
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
