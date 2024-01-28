﻿

using MmoNet.Core.Network.Packets;
using MmoNet.Core.PlayerSessions;

namespace MmoNet.Core.ServerApp; 
public abstract class Controller {
    protected IPacket Ok(object? response, ISession session) {
        return new OkPacket() {
            SessionId = session.Id,
            Result = response
        };
    }

    protected IPacket Deny(object? response, ISession session) {
        return new DenyPacket() {
            SessionId = session.Id,
            Result = response
        };
    }
}
