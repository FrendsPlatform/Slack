namespace Frends.Slack.UpdateMessage.Tests;

using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using Frends.Slack.UpdateMessage.Definitions;
using System.IO;
using dotenv.net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frends.Slack.UpdateMessage.Helpers;
using Frends.Slack.UpdateMessage.Tests.Helpers;

[TestClass]
public class UnitTests
{
    private readonly string _token = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_TOKEN");
    private readonly string _channelId = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_CHANNEL_ID");

    private Connection _connection;
    private Options _defaultOptions;
    private Input _textInput;
    private Input _blocksInput;
    private string _existingMessageTs; // Track message timestamp for updates

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        var root = Directory.GetCurrentDirectory();
        string projDir = Directory.GetParent(root).Parent.Parent.FullName;
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { $"{projDir}/.env.local" }));
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        _connection = new Connection { Token = _token };

        _defaultOptions = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Test error occurred",
            UnfurlLinks = true,
            UnfurlMedia = true,
        };

        var existingMessageTs = await SlackSendMessage.SendMessage(
            "Initial message to be updated",
            _channelId,
            _token,
            CancellationToken.None);

        _existingMessageTs = existingMessageTs;

        _textInput = new Input
        {
            ChannelId = _channelId,
            MessageTs = existingMessageTs,
            Mode = MessageMode.PlainText,
            Text = "Updated plain text content",
        };

        _blocksInput = new Input
        {
            ChannelId = _channelId,
            MessageTs = existingMessageTs,
            Mode = MessageMode.Blocks,
            Blocks = JArray.Parse(@"[{
                'type': 'section',
                'text': {
                    'type': 'mrkdwn',
                    'text': '*Updated* Block Kit content'
                }
            }]"),
        };
    }

    [TestMethod]
    public async Task ShouldUpdatePlainTextMessage()
    {
        var result = await Slack.UpdateMessage(
            _textInput,
            _connection,
            _defaultOptions,
            CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(_existingMessageTs, result.MessageTs);
    }

    [TestMethod]
    public async Task ShouldUpdateBlocksMessage()
    {
        var result = await Slack.UpdateMessage(
            _blocksInput,
            _connection,
            _defaultOptions,
            CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(_existingMessageTs, result.MessageTs);
    }

    [TestMethod]
    public async Task ShouldFailWhenMessageTsMissing()
    {
        var invalidInput = new Input
        {
            ChannelId = _channelId,
            MessageTs = string.Empty,
            Mode = MessageMode.PlainText,
            Text = "This should fail",
        };

        var result = await Slack.UpdateMessage(
            invalidInput,
            _connection,
            _defaultOptions,
            CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "Message timestamp is required");
    }

    [TestMethod]
    public async Task ShouldRespectUnfurlOptionsWhenUpdating()
    {
        var options = new Options
        {
            UnfurlLinks = false,
            UnfurlMedia = false,
        };

        var input = new Input
        {
            ChannelId = _channelId,
            MessageTs = _existingMessageTs,
            Mode = MessageMode.PlainText,
            Text = "Updated with unfurl disabled https://example.com",
        };

        var result = await Slack.UpdateMessage(
            input,
            _connection,
            options,
            CancellationToken.None);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task ShouldFailWhenUpdatingNonexistentMessage()
    {
        var invalidInput = new Input
        {
            ChannelId = _channelId,
            MessageTs = "1234567890.000000",
            Mode = MessageMode.PlainText,
            Text = "This message doesn't exist",
        };

        var result = await Slack.UpdateMessage(
            invalidInput,
            _connection,
            _defaultOptions,
            CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "message_not_found");
    }
}