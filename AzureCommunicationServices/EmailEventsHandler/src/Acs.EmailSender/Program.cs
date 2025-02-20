using Azure;
using Azure.Communication.Email;

var connectionString = "";
var emailClient = new EmailClient(connectionString);


var emailMessage = new EmailMessage(
    senderAddress: "DoNotReply@xxx",
    content: new EmailContent("Test Email")
    {
        PlainText = "Hello world via email.",
        Html = @"
		<html>
			<body>
				<h1>Hello world via email.</h1>
			</body>
		</html>"
    },
recipients: new EmailRecipients(new List<EmailAddress>
{
    new("")
}));
var emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);
Console.WriteLine($"Email send operation ID: {emailSendOperation.Id}");