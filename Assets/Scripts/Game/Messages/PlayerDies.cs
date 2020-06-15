using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class PlayerDies : Message
    {
        public int PlayerDeathID { get; set; }

        //What type of message is this?
        public PlayerDies()
        {
            Type = MessageType.PlayerDies;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerDeathID);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            PlayerDeathID = reader.ReadInt();
        }
    }
}
