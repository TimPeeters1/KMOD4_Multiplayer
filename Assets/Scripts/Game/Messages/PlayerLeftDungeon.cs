using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class PlayerLeftDungeon : Message
    {
        public int PlayerLeftID { get; set; }

        //What type of message is this?
        public PlayerLeftDungeon()
        {
            Type = MessageType.PlayerLeftDungeon;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerLeftID);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            PlayerLeftID = reader.ReadInt();
        }
    }
}
