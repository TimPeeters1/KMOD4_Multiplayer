using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class PlayerDefend : Message
    {
        public int PlayerDefendingID { get; set; }
        public ushort NewHP { get; set; }

        //What type of message is this?
        public PlayerDefend()
        {
            Type = MessageType.PlayerDefends;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerDefendingID);
            writer.WriteUShort(NewHP);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            PlayerDefendingID = reader.ReadInt();
            NewHP = reader.ReadUShort();
        }
    }
}
