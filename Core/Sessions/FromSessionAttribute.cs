
namespace MmoNet.Core.Sessions;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromSessionAttribute : Attribute {
    public FromSessionAttribute() {

    }
}
