using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class NewPlayer : Message
    {
        public int PlayerID { get; set; }
        public uint Colour { get; set; }

        public string PlayerName { get; set; }

        //What type of message is this?
        public NewPlayer()
        {
            Type = MessageType.NewPlayer;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerID);
            writer.WriteUInt(Colour);
            writer.WriteString(PlayerName);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);

            PlayerID = reader.ReadInt();
            Colour = reader.ReadUInt();
            PlayerName = reader.ReadString().ToString();

            //Debug.Log(PlayerName);
        }

    }
}
