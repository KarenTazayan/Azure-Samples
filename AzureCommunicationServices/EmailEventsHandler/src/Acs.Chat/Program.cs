using Azure.Communication.Chat;
using Azure.Communication;

var acsEndpointUrl = "https://xxx.europe.communication.azure.com/";
var userAccessTokenForChat = "";

var communicationTokenCredential = new CommunicationTokenCredential(userAccessTokenForChat);
var chatClient = new ChatClient(new Uri(acsEndpointUrl), communicationTokenCredential);
var list = chatClient.GetChatThreads();
foreach (var item in list)
{
    Console.WriteLine(item.Id);
}

//var result = await chatClient.CreateChatThreadAsync("Sample 1", []);
var id = "";
Console.WriteLine(id);
var chatThreadClient = chatClient.GetChatThreadClient(id);

while (true)
{
    var input = Console.ReadLine();
    var sendChatMessageOptions = new SendChatMessageOptions()
    {
        Content = input ?? "Sample content.",
        MessageType = ChatMessageType.Text
    };
    var sendChatMessageResult = 
        await chatThreadClient.SendMessageAsync(sendChatMessageOptions);
    Console.WriteLine(sendChatMessageResult.Value);
}