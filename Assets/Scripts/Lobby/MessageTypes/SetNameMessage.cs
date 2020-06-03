using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    class SetNameMessage : Message
    {
        public string Name { get; set; }


        public SetNameMessage()
        {
            Type = MessageType.SetName;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);

            writer.WriteString(Name);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);

            Name = reader.ReadString().ToString();
        }

    }
}

