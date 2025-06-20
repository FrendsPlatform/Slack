namespace Frends.Slack.SendMessage.Tests;

using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using Frends.Slack.SendMessage.Definitions;
using System.IO;
using dotenv.net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UnitTests
{
    private readonly string _token = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_TOKEN");
    private readonly string _channelId = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_CHANNEL_ID");

    private Connection _connection;
    private Options _defaultOptions;
    private Input _textInput;
    private Input _blocksInput;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        var root = Directory.GetCurrentDirectory();
        string projDir = Directory.GetParent(root).Parent.Parent.FullName;
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { $"{projDir}/.env.local" }));
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _connection = new Connection
        {
            Token = _token,
        };

        _defaultOptions = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Test error occurred",
            UnfurlLinks = true,
            UnfurlMedia = true,
        };

        _textInput = new Input
        {
            ChannelId = _channelId,
            Mode = MessageMode.PlainText,
            Text = "Hello from unit tests!",
        };

        _blocksInput = new Input
        {
            ChannelId = _channelId,
            Mode = MessageMode.Blocks,
            Blocks = JArray.Parse(@"[{
                'type': 'section',
                'text': {
                    'type': 'mrkdwn',
                    'text': '*Hello* from unit tests!'
                }
            }]"),
        };
    }

    [TestMethod]
    public async Task ShouldSendPlainTextMessage()
    {
        var result = await SendMessage.Slack.SendMessage(_textInput, _connection, _defaultOptions, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.MessageTs));
    }

    [TestMethod]
    public async Task ShouldSendBlocksMessage()
    {
        var result = await SendMessage.Slack.SendMessage(_blocksInput, _connection, _defaultOptions, CancellationToken.None);
        Assert.IsTrue(result.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.MessageTs));
    }

    [TestMethod]
    public async Task ShouldReplyInThreadWhenThreadTsProvided()
    {
        var parentInput = new Input
        {
            ChannelId = _channelId,
            Mode = MessageMode.PlainText,
            Text = "Parent message for thread test",
        };

        var parentResult = await SendMessage.Slack.SendMessage(parentInput, _connection, _defaultOptions, CancellationToken.None);
        Assert.IsTrue(parentResult.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(parentResult.MessageTs));

        var replyInput = new Input
        {
            ChannelId = _channelId,
            Mode = MessageMode.PlainText,
            Text = "Replay message",
            ThreadTs = parentResult.MessageTs,
        };

        var result = await SendMessage.Slack.SendMessage(replyInput, _connection, _defaultOptions, CancellationToken.None);
        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task ShouldFailWhenTokenMissing()
    {
        var invalidConnection = new Connection { Token = string.Empty };

        var result = await SendMessage.Slack.SendMessage(_textInput, invalidConnection, _defaultOptions, CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "Slack token is required");
    }

    [TestMethod]
    public async Task ShouldThrowWhenTokenMissingAndThrowOnFailureEnabled()
    {
        var invalidConnection = new Connection { Token = string.Empty };
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Test error occurred",
        };

        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await SendMessage.Slack.SendMessage(_textInput, invalidConnection, options, CancellationToken.None);
        });
    }

    // Confirm test behavior visually in Slack — link previews depend on UnfurlLinks and UnfurlMedia settings.
    // Note: Slack may unfurl a link only the first time it's posted in a channel to avoid duplication.
    [TestMethod]
    public async Task ShouldRespectUnfurlOptions()
    {
        var optionsUnfurlFalse = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Test error occurred",
            UnfurlLinks = false,
            UnfurlMedia = false,
        };

        var optionsUnfurlTrue = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Test error occurred",
            UnfurlLinks = true,
            UnfurlMedia = true,
        };

        var inputUnfurlFalse = new Input
        {
            ChannelId = _channelId,
            Mode = MessageMode.PlainText,
            Text = "Unfurl false https://www.youtube.com/watch?v=-ZaXNJ5fuas",
        };

        var inputUnfurlTrue = new Input
        {
            ChannelId = _channelId,
            Mode = MessageMode.PlainText,
            Text = "Unfurl true https://www.youtube.com/watch?v=-ZaXNJ5fuas",
        };

        var resultUnfurlFalse = await Slack.SendMessage(inputUnfurlFalse, _connection, optionsUnfurlFalse, CancellationToken.None);
        Assert.IsTrue(resultUnfurlFalse.Success);
        var resultUnfurlTrue = await Slack.SendMessage(inputUnfurlTrue, _connection, optionsUnfurlTrue, CancellationToken.None);
        Assert.IsTrue(resultUnfurlTrue.Success);
    }
}
