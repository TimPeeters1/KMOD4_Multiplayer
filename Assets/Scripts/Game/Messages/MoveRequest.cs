using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class MoveRequest : Message
    {
        public Room.RoomDirections MoveDirection;

        //What type of message is this?
        public MoveRequest()
        {
            Type = MessageType.MoveRequest;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteByte((byte)MoveDirection);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            MoveDirection = (Room.RoomDirections)reader.ReadByte();
        }
    }
}
