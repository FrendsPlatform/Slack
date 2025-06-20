namespace Frends.Slack.DeleteMessage.Tests;

using System;
using System.Threading;
using Frends.Slack.DeleteMessage.Definitions;
using System.IO;
using dotenv.net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frends.Slack.DeleteMessage.Tests.Helpers;

[TestClass]
public class UnitTests
{
    private readonly string _token = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_TOKEN");
    private readonly string _channelId = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_CHANNEL_ID");

    private Connection _connection;
    private Options _defaultOptions;
    private string _existingMessageTs;

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
        _defaultOptions = new Options { ThrowErrorOnFailure = false };

        var existingMessageTs = await SlackSendMessage.SendMessage(
            "Message to be deleted",
            _channelId,
            _token,
            CancellationToken.None);

        _existingMessageTs = existingMessageTs;
    }

    [TestMethod]
    public async Task ShouldDeleteExistingMessage()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            MessageTs = _existingMessageTs,
        };

        var result = await Slack.DeleteMessage(
            input, _connection, _defaultOptions, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(_existingMessageTs, result.MessageTs);
    }

    [TestMethod]
    public async Task ShouldFailWhenMessageTsMissing()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            MessageTs = string.Empty,
        };

        var result = await Slack.DeleteMessage(
            input, _connection, _defaultOptions, CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "Message timestamp is required");
    }

    [TestMethod]
    public async Task ShouldFailWhenDeletingNonexistentMessage()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            MessageTs = "1234567890.000000",
        };

        var result = await Slack.DeleteMessage(
            input, _connection, _defaultOptions, CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "message_not_found");
    }
}