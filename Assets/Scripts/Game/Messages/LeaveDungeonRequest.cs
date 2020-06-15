using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class LeaveDungeonRequest : Message
    {

        //What type of message is this?
        public LeaveDungeonRequest()
        {
            Type = MessageType.LeaveDungeonRequest;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
        }
    }
}
