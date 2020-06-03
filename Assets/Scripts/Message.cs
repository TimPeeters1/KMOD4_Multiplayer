using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;


namespace MutiplayerSystem
{
    public class Message
    {
        private static uint nextID = 0;

        private static uint NextID => ++nextID;

        public enum MessageType
        {
            None = 0,
            NewPlayer,
            Welcome,
            SetName,
            RequestDenied,
            PlayerLeft,
            StartGame,
            PlayerTurn,
            RoomInfo,
            PlayerEnterRoom,
            PlayerLeaveRoom,
            ObtainTreasure,
            HitMonster,
            HitByMonster,
            PlayerDefends,
            PlayerLeftDungeon,
            PlayerDies,
            EndGame,
            MoveRequest,
            AttackRequest,
            DefendRequest,
            ClaimTreasureRequest,
            LeaveDungeonRequest,
        }

        //Message ID
        public uint ID { get; protected set; } = NextID;

        //Type of Lobby Message to send
        public MessageType Type { get; set; }

        public virtual void SerializeObject(ref DataStreamWriter writer)
        {
            writer.WriteUShort((ushort)Type);
            writer.WriteUInt(ID);
        }

        public virtual void DeserializeObject(ref DataStreamReader reader)
        {
            ID = reader.ReadUInt();
        }
    }
}


/*
public class Message
{
    public void SendHeader(ClientBehaviour _client, v _messageType, uint _ID)
    {
        var _writer = _client.m_Driver.BeginSend(_client.m_Connection);
        _writer.WriteUShort((ushort)_messageType);
        _writer.WriteUInt(_ID);
        _client.m_Driver.EndSend(_writer);
    }

    public void ReceiveHeader(ushort _messageType, uint _ID)
    {
        LobbyMessage.MessageType messages = (LobbyMessage.MessageType)_messageType;

        switch (messages)
        {
            case LobbyMessage.MessageType.NewPlayer:
                Debug.Log("New Player" + _ID);
                break;
            case LobbyMessage.MessageType.Welcome:
                Debug.Log("Welcome" + _ID);
                break;
            case LobbyMessage.MessageType.SetName:
                Debug.Log("SetName" + _ID);
                break;
            case LobbyMessage.MessageType.RequestDenied:
                Debug.Log("RequestDenied" + _ID);
                break;
            case LobbyMessage.MessageType.PlayerLeft:
                Debug.Log("PlayerLeft" + _ID);
                break;
            case LobbyMessage.MessageType.StartGame:
                Debug.Log("StartGame" + _ID);
                break;
        }
    }
}
*/
