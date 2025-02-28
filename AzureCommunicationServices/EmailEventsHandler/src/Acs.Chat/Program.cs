using Azure.Communication.Chat;
using Azure.Communication;

var acsEndpointUrl = "https://xxx.communication.azure.com/";
var userAccessTokenForChat = "";

var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
var chatClient = new ChatClient(new Uri(acsEndpointUrl), communicationTokenCredential);
var list = chatClient.GetChatThreads();
foreach (var item in list)
{
    Console.WriteLine(item.Id);
}

//var result1 = await chatClient.CreateChatThreadAsync("Sample 1", []);
//var result2 = await chatClient.CreateChatThreadAsync("Sample 2", []);
//return;

var id1 = "";
var id2 = "";

var chatThreadClient1 = chatClient.GetChatThreadClient(id1);
var chatThreadClient2 = chatClient.GetChatThreadClient(id2);

async Task SendMessage(ChatThreadClient client, SendChatMessageOptions sendChatMessageOptions)
{
    var sendChatMessageResult = await client.SendMessageAsync(sendChatMessageOptions);
    Console.WriteLine(sendChatMessageResult.Value);
}

var i = 0;
while (true)
{
    //var input = Console.ReadLine();
    var input = i++;
    Console.WriteLine(input);
    var sendChatMessageOptions = new SendChatMessageOptions()
    {
        Content = input.ToString() ?? "Sample content.",
        MessageType = ChatMessageType.Text
    };

    if (i % 2 == 0)
    {
        await SendMessage(chatThreadClient1, sendChatMessageOptions);
    }
    else
    {
        await SendMessage(chatThreadClient2, sendChatMessageOptions);
    }

    Thread.Sleep(1250);
}