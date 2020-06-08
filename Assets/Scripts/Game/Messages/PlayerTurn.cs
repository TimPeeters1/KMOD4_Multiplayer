using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class PlayerTurn : Message
    {
        public int PlayerID { get; set; }

        //What type of message is this?
        public PlayerTurn()
        {
            Type = MessageType.PlayerTurn;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerID);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            PlayerID = reader.ReadInt();
        }
    }
}
