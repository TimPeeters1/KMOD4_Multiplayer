using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class EndGame : Message
    {
        public byte NumberOfScores;

        public Dictionary<int, ushort> PlayerScorePair = new Dictionary<int, ushort>();


        //What type of message is this?
        public EndGame()
        {
            Type = MessageType.EndGame;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteByte(NumberOfScores);

            for (int i = 0; i < NumberOfScores; i++)
            {
                writer.WriteInt(PlayerScorePair.ElementAt(i).Key);
                writer.WriteUShort(PlayerScorePair.ElementAt(i).Value);
            }
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            NumberOfScores = reader.ReadByte();

            for (int i = 0; i < NumberOfScores; i++)
            {
                PlayerScorePair.Add(reader.ReadInt(), reader.ReadUShort());
            }
        }
    }
}
