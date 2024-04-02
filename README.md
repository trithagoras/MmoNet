# MMO.NET

MMO.NET is a MMO game server framework for .NET inspired by ASP.NET.

## Sample setup

This is a simple example of how to create a server using MMO.NET. It's nearly identical to the example setup shown in the `Sample` project.

```csharp
var port = 0;

var builder = new ServerBuilder();
builder.Services.AddProtocolLayer<TcpLayer>();
builder.Services.AddSessionManager<PlayerSessionManager>();
builder.Services.AddSerializer<JsonSerializer>();
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddExceptionFilter<ExceptionFilter>();

var (app, _) = builder.Build();
app.UseStateAuth();

await app.StartAsync(port);
```

This will create a server that listens on a random port and uses TCP as the transport layer. It will also use a JSON serializer and a session manager that manages `PlayerSession` instances. The `ILoginService` will be used to authenticate incoming connections.

### Getting started

Creating a server is done by creating a `ServerBuilder` instance and adding services to it. The `ServerBuilder` is a wrapper around the ASP.NET `WebHostBuilder` and can be used in the same way.

A `ProtocolLayer`, `SessionManager` and `Serializer` are required to create a server. The `ProtocolLayer` is responsible for handling incoming connections and sending data to clients. The `SessionManager` is responsible for managing sessions. The `Serializer` is responsible for serializing and deserializing data.

The `ServerBuilder` also allows you to add middleware to the server. Middleware can be used to add functionality to the server. For example, the `UseStateAuth` middleware can be used to authorize sessions against specific actions.

Add controllers and extend the `Controller` class to create endpoints. Controllers can be used to handle incoming requests and send responses back to the client. Each `Controller` action must have exactly one parameter that is an `IPacket`. It must be unique for each action in order for the server to know which action to invoke.

### Example:

```csharp
namespace Sample; 
public class EntryController(ILoginService service, ILogger<EntryController> logger, ISessionManager sessionManager) : Controller {
    readonly ILoginService service = service;
    readonly ILogger<EntryController> logger = logger;
    readonly ISessionManager sessionManager = sessionManager;

    public async Task<IPacket> Login(LoginPacket packet) {
        await service.LoginAsync(packet.Username, packet.Password);
        return Ok(result);
    }

    public async Task<IPacket> Logout(LogoutPacket _) {
        await service.LogoutAsync();
        return Ok(result);
    }
}
```

This controller has three actions. The `Login` action requires the session to be in the `Entry` state. The `Logout` action requires the session to be in any state except the `Entry` state. These actions are dispatched automatically by the server when a packet is received.

Note that any exceptions thrown in the controller actions will be caught by the server and handled by the `ExceptionFilter` middleware. The `ExceptionFilter` middleware can be used to handle exceptions and send an appropriate response back to the client.