using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    public class RoomInfo : Message
    {
        public byte MoveDirections { get; set; }
        public ushort TreasureInRoom { get; set; }
        public byte ContainsMonster { get; set; }
        public byte ContainsExit { get; set; }
        public byte NumberOfOtherPlayers { get; set; }
        public int[] OtherPlayerIDs { get; set; }

        //What type of message is this?
        public RoomInfo()
        {
            Type = MessageType.RoomInfo;
        }

        public void SerializeObject(ref DataStreamWriter writer, int[] otherPlayers)
        {
            base.SerializeObject(ref writer);
            writer.WriteByte(MoveDirections);
            writer.WriteUShort(TreasureInRoom);
            writer.WriteByte(ContainsMonster);
            writer.WriteByte(ContainsExit);
            writer.WriteByte(NumberOfOtherPlayers);

            OtherPlayerIDs = otherPlayers;
            for (int i = 0; i < NumberOfOtherPlayers; i++)
            {
                writer.WriteInt(OtherPlayerIDs[i]);
            }

        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            MoveDirections = reader.ReadByte();
            TreasureInRoom = reader.ReadUShort();
            ContainsMonster = reader.ReadByte();
            ContainsExit = reader.ReadByte();
            NumberOfOtherPlayers = reader.ReadByte();

            OtherPlayerIDs = new int[NumberOfOtherPlayers];
            for (int i = 0; i < NumberOfOtherPlayers; i++)
            {
               OtherPlayerIDs[i] = reader.ReadInt();
            }
            //Debug.Log(PlayerName);
        }

    }
}
