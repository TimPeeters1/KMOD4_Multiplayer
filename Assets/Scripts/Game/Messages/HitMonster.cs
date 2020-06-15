using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class HitMonster : Message
    {
        public int PlayerAttackingID { get; set; }
        public ushort DamageDealt { get; set; }

        //What type of message is this?
        public HitMonster()
        {
            Type = MessageType.HitMonster;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteInt(PlayerAttackingID);
            writer.WriteUShort(DamageDealt);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            PlayerAttackingID = reader.ReadInt();
            DamageDealt = reader.ReadUShort();
        }
    }
}
