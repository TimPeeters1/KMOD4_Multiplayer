using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class WelcomeMessage : Message
    {
        public int PlayerID { get; set; }
        public uint Colour { get; set; }


        //What type of message is this?
        public WelcomeMessage()
        {
            Type = MessageType.Welcome;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerID);
            writer.WriteUInt(Colour);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);

            PlayerID = reader.ReadInt();
            Colour = reader.ReadUInt();
        }

    }
}
