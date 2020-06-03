using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    class PlayerLeft : Message
    {
        public int PlayerDisconnectID { get; set; }

        public PlayerLeft()
        {
            Type = MessageType.PlayerLeft;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);

            writer.WriteInt(PlayerDisconnectID);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);

            PlayerDisconnectID = reader.ReadInt();
        }

    }
}

