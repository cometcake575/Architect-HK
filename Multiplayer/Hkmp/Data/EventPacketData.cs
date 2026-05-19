using Hkmp.Networking.Packet;

namespace Architect.Multiplayer.Hkmp.Data;

public class EventPacketData : ScenePacketData
{
    public string Event;

    protected override void WriteExtData(IPacket packet)
    {
        packet.Write(Event);
    }

    protected override void ReadExtData(IPacket packet)
    {
        Event = packet.ReadString();
    }
}