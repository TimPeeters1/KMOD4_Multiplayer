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
            if (IsClient)//Clientside handling
            {
                switch (messageType)
                {
                    case Message.MessageType.NewPlayer:
                        var newPlayer = new NewPlayer();
                        newPlayer.DeserializeObject(ref reader);
                        //Debug.Log(newPlayer.PlayerName + " " + newPlayer.PlayerID + " " + newPlayer.Colour);


                        ServerClient _newClient = new ServerClient(newPlayer.PlayerID, newPlayer.PlayerName, DataUtilities.DecodeColor((int)newPlayer.Colour));
                        ClientBehaviour.Instance.Clients.Add(_newClient);

                        if (!ClientBehaviour.Instance.IsHost)
                        {
                            ClientBehaviour.Instance.Lobby.PrintMessage(newPlayer.PlayerName + ", joined the game.", Color.green);
                            ClientBehaviour.Instance.Lobby.LobbyUI.GetComponent<PlayerLobbyList>().UpdateLobby(_newClient);
                        }

                        break;
                    case Message.MessageType.Welcome:
                        var welcomeMessage = new WelcomeMessage();
                        welcomeMessage.DeserializeObject(ref reader);
                        ClientBehaviour.Instance.PlayerID = welcomeMessage.PlayerID;

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Succesfully joined the server.", Color.green);

                        //Send Name to Server.
                        var nameMessage = new SetNameMessage
                        {
                            Name = ClientBehaviour.Instance.PlayerName
                        };

                        var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
                        nameMessage.SerializeObject(ref writer);
                        ClientBehaviour.Instance.m_Driver.EndSend(writer);

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Succesfully Connected to the Server.", Color.green);


                        break;
                    case Message.MessageType.SetName:
                        break;
                    case Message.MessageType.RequestDenied:
                        var deniedMessage = new RequestDenied();
                        deniedMessage.DeserializeObject(ref reader);
                        Debug.Log("Message ID: " + deniedMessage.DeniedMessageID + " was denied by the server.");

                        break;
                    case Message.MessageType.PlayerLeft:
                        var playerLeft = new PlayerLeft();
                        playerLeft.DeserializeObject(ref reader);

                        ClientBehaviour.Instance.Clients.RemoveAt((int)playerLeft.ID);

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Player " + playerLeft.PlayerDisconnectID + " has disconnected.", Color.yellow);

                        break;
                    case Message.MessageType.StartGame:
                        var startGame = new StartGame();
                        startGame.DeserializeObject(ref reader);
                        for (int i = 0; i < ClientBehaviour.Instance.Clients.Count; i++)
                        {
                            ClientBehaviour.Instance.Clients[i].HealthPoints = startGame.StartHP;
                        }

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Game Starting...", Color.blue);

                        ClientBehaviour.Instance.CurrentGameState = GameState.InGame;
                        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                        break;
                    case Message.MessageType.None:
                    default:
                        //Debug.Log("Received Server Data | " + ConnectionID);
                        break;
                }
            }
            else //Serverside handling
            {
                switch (messageType)
                {
                    case Message.MessageType.NewPlayer:
                        break;
                    case Message.MessageType.Welcome:
                        break;
                    case Message.MessageType.SetName:
                        var _message = new SetNameMessage();
                        _message.DeserializeObject(ref reader);
                        //Debug.Log("Welcome User:" + _message.Name + i);

                        ServerBehaviour.Instance.Clients[ConnectionID].ClientName = _message.Name;

                        var newPlayerMessage = new NewPlayer
                        {
                            PlayerID = ServerBehaviour.Instance.Clients[ConnectionID].ClientID,
                            Colour = (uint)DataUtilities.EncodeColor(ServerBehaviour.Instance.Clients[ConnectionID].ClientColour),
                            PlayerName = ServerBehaviour.Instance.Clients[ConnectionID].ClientName,
                        };

                        for (int f = 0; f < ServerBehaviour.Instance.m_Connections.Length; f++)
                        {
                            if (f != newPlayerMessage.PlayerID)
                            {
                                //Parse New Client to other clients.
                                var write = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[f]);
                                newPlayerMessage.SerializeObject(ref write);
                                ServerBehaviour.Instance.m_Driver.EndSend(write);
                            }
                            else
                            {
                                for (int h = 0; h < ServerBehaviour.Instance.m_Connections.Length; h++)
                                {
                                    //Parse All other clients to new client.
                                    var otherPlayer = new NewPlayer
                                    {
                                        PlayerID = ServerBehaviour.Instance.Clients[h].ClientID,
                                        Colour = (uint)DataUtilities.EncodeColor(ServerBehaviour.Instance.Clients[h].ClientColour),
                                        PlayerName = ServerBehaviour.Instance.Clients[h].ClientName,
                                    };

                                    var write = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[f]);
                                    otherPlayer.SerializeObject(ref write);
                                    ServerBehaviour.Instance.m_Driver.EndSend(write);
                                }
                            }
                        }

                        ServerBehaviour.Instance.Lobby.LobbyUI.GetComponent<PlayerLobbyList>().UpdateLobby(ServerBehaviour.Instance.Clients[ConnectionID]);
                        break;
                    case Message.MessageType.RequestDenied:
                        break;
                    case Message.MessageType.PlayerLeft:
                        break;
                    case Message.MessageType.StartGame:
                        //Server Start is handled in serverbehaviour.
                        break;
                    case Message.MessageType.None:
                    default:
                        Debug.Log("Received Client Data | " + ConnectionID);
                        break;

                }
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

    public static class DataUtilities
    {
        public static int EncodeColor(Color32 _color)
        {
            int rgb = ((_color.r & 0x0ff) << 16) | ((_color.g & 0x0ff) << 8) | (_color.b & 0x0ff);
            //Debug.Log(rgb);
            return rgb;
        }

        public static Color32 DecodeColor(int _colorInt)
        {
            int red = (_colorInt >> 16) & 0x0ff;
            int green = (_colorInt >> 8) & 0x0ff;
            int blue = (_colorInt) & 0x0ff;

            //Debug.Log(red + "," + green + "," + blue + "," + 1);
            return new Color(red, green, blue, 1); //no support for alpha
        }
    }
}
