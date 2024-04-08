using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Core.ServerApp;
using Sample.Services;
using Sample.Exceptions;
using Microsoft.Extensions.Logging;
using MmoNet.Shared.Packets;

var port = 42523;
var tickRate = 20;

var builder = new ServerBuilder();
builder.Services.AddProtocolLayer<TcpLayer>();
builder.Services.AddSessionManager<PlayerSessionManager>();
builder.Services.AddSerializer<JsonSerializer>();
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddExceptionFilter<SampleExceptionFilter>();
builder.Services.AddLogging(o => o.AddConsole());
builder.Services.AddPacketRegistry<PacketRegistry>();

var (app, _) = builder.Build();

await app.StartAsync(port, tickRate);