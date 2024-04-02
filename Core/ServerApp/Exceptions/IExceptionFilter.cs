namespace MmoNet.Core.ServerApp.Exceptions;
public interface IExceptionFilter {
    public void OnException(ActionExceptionContext context);
}
