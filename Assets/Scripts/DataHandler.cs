using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.Networking;


namespace MutiplayerSystem
{
    public class DataHandler
    {
        public bool IsClient;

        public DataHandler(bool isClient)
        {
            IsClient = isClient;
        }

        public virtual void HandleMessages(Message.MessageType messageType, DataStreamReader reader, int ConnectionID)
        { }
    }

    public class LobbyDataHandler : DataHandler
    {
        public LobbyDataHandler(bool isClient) : base(isClient)
        {
        }

        public override void HandleMessages(Message.MessageType messageType, DataStreamReader reader, int ConnectionID)
        {
            switch (messageType)
            {
                case Message.MessageType.NewPlayer:
                    if (IsClient)
                    {
                        var newPlayer = new NewPlayer();
                        newPlayer.DeserializeObject(ref reader);
                        Debug.Log(newPlayer.PlayerName + " " + newPlayer.PlayerID + " " + newPlayer.Colour);

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage(newPlayer.PlayerName + ", joined the game.", Color.green);
                    }
                    break;
                case Message.MessageType.Welcome:
                    if (IsClient)
                    {
                        var welcomeMessage = new WelcomeMessage();
                        welcomeMessage.DeserializeObject(ref reader);
                        //Debug.Log(welcomeMessage.ID + " " + welcomeMessage.PlayerID + " " + welcomeMessage.Colour);

                        //Send Name
                        var nameMessage = new SetNameMessage
                        {
                            Name = ClientBehaviour.Instance.PlayerName
                        };

                        var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
                        nameMessage.SerializeObject(ref writer);
                        ClientBehaviour.Instance.m_Driver.EndSend(writer);

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Succesfully Connected to the Server.", Color.green);
                    }

                    break;
                case Message.MessageType.SetName:
                    if (!IsClient)
                    {
                        var _message = new SetNameMessage();
                        _message.DeserializeObject(ref reader);
                        //Debug.Log("Welcome User:" + _message.Name + i);

                        ServerBehaviour.Instance.Clients[ConnectionID].ClientName = _message.Name;

                        //Parse New Player to other clients.
                        var newPlayerMessage = new NewPlayer
                        {
                            PlayerID = ServerBehaviour.Instance.Clients[ConnectionID].ClientID,
                            Colour = ServerBehaviour.Instance.Clients[ConnectionID].ClientColour,
                            PlayerName = ServerBehaviour.Instance.Clients[ConnectionID].ClientName,
                        };

                        for (int f = 0; f < ServerBehaviour.Instance.m_Connections.Length; f++)
                        {
                            if (f != newPlayerMessage.PlayerID)
                            {
                                var write = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[f]);
                                newPlayerMessage.SerializeObject(ref write);
                                ServerBehaviour.Instance.m_Driver.EndSend(write);
                            }
                        }


                        ServerBehaviour.Instance.Lobby.UpdateLobby(ServerBehaviour.Instance.Clients[ConnectionID]);
                    }
                    break;
                case Message.MessageType.RequestDenied:
                    if (IsClient)
                    {
                        var deniedMessage = new RequestDenied();
                        deniedMessage.DeserializeObject(ref reader);
                        Debug.Log("Message ID: " + deniedMessage.DeniedMessageID + " was denied by the server.");
                    }
                    break;
                case Message.MessageType.PlayerLeft:
                    if (IsClient)
                    {
                        var playerLeft = new PlayerLeft();
                        playerLeft.DeserializeObject(ref reader);

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Player " + playerLeft.PlayerDisconnectID + " has disconnected.", Color.yellow);
                    }
                    break;
                case Message.MessageType.StartGame:
                    if (IsClient)
                    {
                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Game Starting...", Color.blue);
                    }
                    break;
                case Message.MessageType.None:
                default:
                    if (IsClient)
                    {
                        Debug.Log("Received Server Data | " + ConnectionID);
                    }
                    else
                    {
                        Debug.Log("Received Client Data | " + ConnectionID);
                    }
                    break;
            }
        }
    }

    public class GameDataHandler : DataHandler
    {
        public GameDataHandler(bool isClient) : base(isClient)
        {
        }

        public override void HandleMessages(Message.MessageType messageType, DataStreamReader reader, int ConnectionID)
        {
            switch (messageType)
            {
                case Message.MessageType.PlayerTurn:
                    break;
                case Message.MessageType.RoomInfo:
                    break;
                case Message.MessageType.PlayerEnterRoom:
                    break;
                case Message.MessageType.PlayerLeaveRoom:
                    break;
                case Message.MessageType.ObtainTreasure:
                    break;
                case Message.MessageType.HitMonster:
                    break;
                case Message.MessageType.HitByMonster:
                    break;
                case Message.MessageType.PlayerDefends:
                    break;
                case Message.MessageType.PlayerLeftDungeon:
                    break;
                case Message.MessageType.PlayerDies:
                    break;
                case Message.MessageType.EndGame:
                    break;
                case Message.MessageType.MoveRequest:
                    break;
                case Message.MessageType.AttackRequest:
                    break;
                case Message.MessageType.DefendRequest:
                    break;
                case Message.MessageType.ClaimTreasureRequest:
                    break;
                case Message.MessageType.LeaveDungeonRequest:
                    break;
            }
        }
    }
}