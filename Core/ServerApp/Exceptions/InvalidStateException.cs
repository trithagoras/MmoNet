using MmoNet.Core.States;
using MmoNet.Shared.Packets;

namespace MmoNet.Core.ServerApp.Exceptions; 
public class InvalidStateException(string message) : Exception(message) {
}
