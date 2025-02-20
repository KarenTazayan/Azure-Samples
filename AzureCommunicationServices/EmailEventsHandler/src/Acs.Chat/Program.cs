using Azure.Communication.Chat;
using Azure.Communication;

var acsEndpointUrl = "https://acs-email-events-handler-1.europe.communication.azure.com/";
var userAccessTokenForChat = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjU3Qjg2NEUwQjM0QUQ0RDQyRTM3OTRBRTAyNTAwRDVBNTE5MjA1RjUiLCJ4NXQiOiJWN2hrNExOSzFOUXVONVN1QWxBTldsR1NCZlUiLCJ0eXAiOiJKV1QifQ.eyJza3lwZWlkIjoiYWNzOjI3YzUxZjQ5LTczYjEtNGM3MC04MmVjLTRmNDA3MDA4YWQ4M18wMDAwMDAyNS05ZTJhLTZhNDYtNjVmMC1hZDNhMGQwMGE0NjUiLCJzY3AiOjE3OTIsImNzaSI6IjE3Mzk0MzI5NjkiLCJleHAiOjE3Mzk1MTkzNjksInJnbiI6ImVtZWEiLCJhY3NTY29wZSI6ImNoYXQiLCJyZXNvdXJjZUlkIjoiMjdjNTFmNDktNzNiMS00YzcwLTgyZWMtNGY0MDcwMDhhZDgzIiwicmVzb3VyY2VMb2NhdGlvbiI6ImV1cm9wZSIsImlhdCI6MTczOTQzMjk2OX0.oX9mQ_d6H7FSRVWJ2SsUVBgwM55lLw0BpH-0KFIlQAePXR2QNEXi-Xx1dpc0G2zrAWRcURr1FSIDidVuV_sBV342LjdMvUBkME2D9Cr2W25N2O7EA2sQ0ziaPCIH0nWeIDdaY2EFwmQI28Sn3nSPS8S_K0xvLOvM56H9PHMlxZPKZe5aXtatGBBmJVl2cR0ssAD0r59InCggki31sUwzpTKbQmBnmKmbEpK91I-nbz_dpPcchc-RZ6261Wa1OGFHOVmbiqiyiAMfklBompPz00XHoP20kjBDoeZfj95FHsoerTy7cmLKwzo7m9-uAQ_g6g5mhau_Eb7idB8SJFVx5g";

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

var id1 = "19:acsV1_sG55ouol0jL4zVITRrPXmxRx03G_nbgplXp18IGIV981@thread.v2";
var id2 = "19:acsV1_ko2b4Xd_WW_dtLT1jALi9vr0271RvrQXjDa-6A6PRXA1@thread.v2";

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