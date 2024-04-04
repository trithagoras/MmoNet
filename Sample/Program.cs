using Microsoft.Extensions.DependencyInjection;
using MmoNet.Core.Network.Protocols;
using MmoNet.Shared.Serializers;
using MmoNet.Core.Sessions;
using MmoNet.Core.ServerApp;
using Sample.Services;
using Sample.Exceptions;

var port = 42523;

var builder = new ServerBuilder();
builder.Services.AddProtocolLayer<TcpLayer>();
builder.Services.AddSessionManager<PlayerSessionManager>();
builder.Services.AddSerializer<JsonSerializer>();
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddExceptionFilter<SampleExceptionFilter>();

var (app, _) = builder.Build();

await app.StartAsync(port);