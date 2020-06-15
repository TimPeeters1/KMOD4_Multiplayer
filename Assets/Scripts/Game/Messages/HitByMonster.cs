using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class HitByMonster : Message
    {
        public int PlayerHitID { get; set; }
        public ushort DamageDealt { get; set; }

        //What type of message is this?
        public HitByMonster()
        {
            Type = MessageType.HitByMonster;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerHitID);
            writer.WriteUShort(DamageDealt);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            PlayerHitID = reader.ReadInt();
            DamageDealt = reader.ReadUShort();
        }
    }
}
