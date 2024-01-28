using Microsoft.Extensions.DependencyInjection;
using MmoNet.Shared.Packets;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Core.ServerApp;
using Sample.Packets;
using Sample.Services;
using System.Net.Sockets;
using System.Text;

namespace MmoNet.Sample.Tests;
[TestClass]
public class SampleTests {
    static ServerApplication app = null!;
    static ServiceProvider provider = null!;
    static TcpLayer Layer => (TcpLayer)provider.GetRequiredService<IProtocolLayer>();
    static JsonSerializer Serializer => (JsonSerializer)provider.GetRequiredService<ISerializer>();
    static int Port => Layer.Port;

    TcpClient client = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context) {
        var port = 0;

        var builder = new ServerBuilder();
        builder.Services.AddProtocolLayer<TcpLayer>();
        builder.Services.AddSessionManager<PlayerSessionManager>();
        builder.Services.AddSerializer<JsonSerializer>();
        builder.Services.AddSingleton<ILoginService, LoginService>();

        var (app, provider) = builder.Build();
        app.UseStateAuth();
        SampleTests.app = app;
        SampleTests.provider = provider;

        _ = app.StartAsync(port)
            .ContinueWith(t => {
                t.Exception!.Handle(e => {
                    Assert.Fail(e.Message);
                    return true;
                });
            }, TaskContinuationOptions.OnlyOnFaulted);
    }

    [TestInitialize]
    public async Task TestInitialize() {
        client = new TcpClient();
    }

    [TestCleanup]
    public void TestCleanup() {
        client.Close();
    }

    [TestMethod]
    public async Task TestAssignSessionId() {
        await client.ConnectAsync("localhost", Port);
        var stream = client.GetStream();

        // read Id packet
        var bytes = new byte[1024];
        var length = await stream.ReadAsync(bytes);
        var packet = Serializer.Deserialize(bytes[..length]);
        Assert.IsNotNull(packet);
        Assert.IsTrue(packet is IdPacket);
    }

    [TestMethod]
    public async Task TestLogin() {
        await client.ConnectAsync("localhost", Port);
        var stream = client.GetStream();

        // read Id packet
        var bytes = new byte[1024];
        var length = await stream.ReadAsync(bytes);
        var packet = Serializer.Deserialize(bytes[..length]);
        Assert.IsNotNull(packet);
        Assert.IsTrue(packet is IdPacket);

        // send login packet
        var login = new LoginPacket {
            Username = "test",
            Password = "test",
            SessionId = packet.SessionId
        };
        
        var bs = Serializer.Serialize(login);
        await stream.WriteAsync(bs);
        await stream.FlushAsync();

        // read Ok packet
        bytes = new byte[1024];
        length = await stream.ReadAsync(bytes);
        packet = Serializer.Deserialize(bytes[..length]);
        Assert.IsNotNull(packet);
        Assert.IsTrue(packet is OkPacket);
    }

    [ClassCleanup]
    public static async Task ClassCleanup() {
        await app.StopAsync();
    }
}