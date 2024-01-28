using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Protocols;
using MmoNet.Core.Network.Serializers;
using MmoNet.Core.PlayerSessions;
using MmoNet.Core.ServerApp;
using Sample.Packets;
using Sample.Services;

var port = 42523;

var builder = new ServerBuilder();
builder.Services.AddProtocolLayer<TcpLayer>();
builder.Services.AddSessionManager<PlayerSessionManager>();
builder.Services.AddSerializer<JsonSerializer>();
builder.Services.AddSingleton<ILoginService, LoginService>();

var (app, _) = builder.Build();
app.UseStateAuth();


await app.StartAsync(port);