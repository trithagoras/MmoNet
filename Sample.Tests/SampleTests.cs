using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Packets;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;
using MmoNet.Core.ServerApp;
using Newtonsoft.Json;
using Sample.Packets;
using Sample.Services;
using System.Net.Sockets;

namespace MmoNet.Sample.Tests;
[TestClass]
public class SampleTests {

    static ServerApplication app = null!;
    static ServiceProvider provider = null!;
    static TcpLayer layer = null!;
    static int Port => layer.Port;

    TcpClient client = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context) {
        var port = 0;

        var builder = new ServerBuilder();
        builder.Services.AddProtocolLayer<TcpLayer>();
        builder.Services.AddSessionManager<PlayerSessionManager>();
        builder.Services.AddSerializer<Core.Network.Serializers.JsonSerializer>();
        builder.Services.AddSingleton<ILoginService, LoginService>();

        var (app, provider) = builder.Build();
        app.UseStateAuth();
        SampleTests.app = app;
        SampleTests.provider = provider;
        layer = (TcpLayer)provider.GetRequiredService<IProtocolLayer>();

        _ = app.StartAsync(port);   // don't await here, we want to run the server in the background
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
    public async Task TestConnection() {
        await client.ConnectAsync("localhost", Port);
    }

    [TestMethod]
    public async Task TestLogin() {
        await client.ConnectAsync("localhost", Port);
        var stream = client.GetStream();
        var reader = new StreamReader(stream);
        var writer = new StreamWriter(stream) { AutoFlush = true };

        // read Id packet
        var response = await reader.ReadLineAsync();
        IPacket packet = JsonConvert.DeserializeObject<IdPacket>(response);
        Assert.IsNotNull(packet);

        var login = new LoginPacket {
            Username = "test",
            Password = "test",
            SessionId = packet.SessionId
        };
        
        var json = JsonConvert.SerializeObject(login, new JsonSerializerSettings() {
            NullValueHandling = NullValueHandling.Ignore
        });
        await writer.WriteLineAsync(json);

        // TODO: This won't work probably because of session ID??
        response = await reader.ReadLineAsync();
        packet = JsonConvert.DeserializeObject<OkPacket>(response);
        Assert.IsNotNull(packet);
    }

    [ClassCleanup]
    public static async Task ClassCleanup() {
        await app.StopAsync();
    }
}