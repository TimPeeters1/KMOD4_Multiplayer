using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    class RequestDenied : Message
    {
        public int DeniedMessageID { get; set; }


        public RequestDenied()
        {
            Type = MessageType.RequestDenied;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);

            writer.WriteInt(DeniedMessageID);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);

            DeniedMessageID = reader.ReadInt();
        }

    }
}

