using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class ObtainTreasure : Message
    {
        public ushort TreasureAmount { get; set; }

        //What type of message is this?
        public ObtainTreasure()
        {
            Type = MessageType.ObtainTreasure;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteUShort(TreasureAmount);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            TreasureAmount = reader.ReadUShort();
        }
    }
}
