
using MmoNet.Core.Middlewares;

namespace MmoNet.Core.ServerApp; 
public static class ServerApplicationExtensions {
    public static ServerApplication UseStateAuth(this ServerApplication app) {
        // Add the StateAuth middleware to the application's middleware pipeline
        app.AddMiddleware<StateAuthMiddleware>();
        return app;
    }
}
