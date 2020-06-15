using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

                        ClientBehaviour.Instance.Clients.RemoveAt(playerLeft.PlayerDisconnectID);

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Player " + playerLeft.PlayerDisconnectID + " has disconnected.", Color.yellow);

                        break;
                    case Message.MessageType.StartGame:
                        var startGame = new StartGame();
                        startGame.DeserializeObject(ref reader);
                        for (int i = 0; i < ClientBehaviour.Instance.Clients.Count; i++)
                        {
                            ClientBehaviour.Instance.Clients[i].StartHealth = startGame.StartHP;
                            //Debug.Log(startGame.StartHP);
                        }

                        if (!ClientBehaviour.Instance.IsHost)
                            ClientBehaviour.Instance.Lobby.PrintMessage("Game Starting...", Color.blue);

                        ClientBehaviour.Instance.CurrentGameState = GameState.InGame;
                        ClientBehaviour.Instance.ClientUI = GameObject.Instantiate(Resources.Load("ClientUI") as GameObject).GetComponent<ClientUI>();
                        ClientBehaviour.Instance.ClientUI.PlayerInfo.text = ClientBehaviour.Instance.PlayerName;
                        ClientBehaviour.Instance.ClientUI.HealthInfo.text = startGame.StartHP + "/" + startGame.StartHP + " HP";
                        GameObject.DontDestroyOnLoad(ClientBehaviour.Instance.ClientUI.gameObject);

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
            if (IsClient)
            {
                //Clientside Handling
                switch (messageType)
                {
                    case Message.MessageType.PlayerTurn:

                        //This part handles the client side of a player turn.
                        var playerTurn = new PlayerTurn();
                        playerTurn.DeserializeObject(ref reader);
                        GameManager.Instance.PlayerTurnUI.text = "Current Turn: " + ClientBehaviour.Instance.Clients[playerTurn.PlayerID].ClientName;
                        GameManager.Instance.PlayerCurrentTurn = GameManager.Instance.Players[playerTurn.PlayerID];

                        break;
                    case Message.MessageType.RoomInfo:
                        var roomInfo = new RoomInfo();
                        roomInfo.DeserializeObject(ref reader);

                        GameManager.Instance.RoomGameObject.SetRoomProperties(
                            (Room.RoomDirections)roomInfo.MoveDirections, roomInfo.TreasureInRoom,
                                roomInfo.ContainsMonster, roomInfo.ContainsExit);

                        #region SetNewPlayers
                        //Disable all players first.
                        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
                        {
                            GameManager.Instance.Players[i].gameObject.SetActive(false);
                        }

                        //Then enable all players that are currently in this room.
                        for (int f = 0; f < roomInfo.NumberOfOtherPlayers; f++)
                        {
                            if (!GameManager.Instance.Players[roomInfo.OtherPlayerIDs[f]].noTurn)
                            {
                                GameManager.Instance.Players[roomInfo.OtherPlayerIDs[f]].gameObject.SetActive(true);
                            }
                        }
                        #endregion

                        break;
                    case Message.MessageType.PlayerEnterRoom:
                        var playerEnter = new PlayerEnterRoom();
                        playerEnter.DeserializeObject(ref reader);

                        GameManager.Instance.Players[playerEnter.PlayerID].gameObject.SetActive(true);

                        if (playerEnter.PlayerID != ConnectionID)
                        {
                            string enterMessage = GameManager.Instance.Players[playerEnter.PlayerID].Client.ClientName + " joined the room";
                            ClientBehaviour.Instance.ClientUI.ShowMessage(enterMessage);
                        }

                        break;
                    case Message.MessageType.PlayerLeaveRoom:
                        var playerLeave = new PlayerLeaveRoom();
                        playerLeave.DeserializeObject(ref reader);

                        GameManager.Instance.Players[playerLeave.PlayerID].gameObject.SetActive(false);

                        if (playerLeave.PlayerID != ConnectionID)
                        {
                            string leftMessage = GameManager.Instance.Players[playerLeave.PlayerID].Client.ClientName + " left the room";
                            ClientBehaviour.Instance.ClientUI.ShowMessage(leftMessage);
                        }

                        break;
                    case Message.MessageType.ObtainTreasure:
                        var obtainedTreasure = new ObtainTreasure();
                        obtainedTreasure.DeserializeObject(ref reader);

                        GameManager.Instance.RoomGameObject.ChestObject.SetActive(false);

                        #region SetTreasureAmount
                        if (ClientBehaviour.Instance.IsHost)
                        {
                            for (int f = 0; f < GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count; f++)
                            {
                                GameManager.Instance.Players[GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom[f].Client.ClientID].PlayerTreasureAmount += obtainedTreasure.TreasureAmount;
                            }
                        }
                        else
                        {
                            GameManager.Instance.Players[ClientBehaviour.Instance.PlayerID].PlayerTreasureAmount += obtainedTreasure.TreasureAmount;
                        }
                        #endregion

                        string obtainMessage = GameManager.Instance.PlayerCurrentTurn.name + " took a treasure and shared it with everyone in the room. You gained "
                            + obtainedTreasure.TreasureAmount + "$.";
                        ClientBehaviour.Instance.ClientUI.ShowMessage(obtainMessage);


                        break;
                    case Message.MessageType.HitMonster:
                        var hitMessage = new HitMonster();
                        hitMessage.DeserializeObject(ref reader);

                        string hitString = GameManager.Instance.Players[hitMessage.PlayerAttackingID].Client.ClientName + " killed the monster.";
                        ClientBehaviour.Instance.ClientUI.ShowMessage(hitString);

                        GameManager.Instance.RoomGameObject.MonsterObject.SetActive(false);

                        break;
                    case Message.MessageType.HitByMonster:
                        var playerHit = new HitByMonster();
                        playerHit.DeserializeObject(ref reader);

                        GameManager.Instance.Players[playerHit.PlayerHitID].Health -= playerHit.DamageDealt;
                        if (playerHit.PlayerHitID == ClientBehaviour.Instance.PlayerID)
                        {
                            ClientBehaviour.Instance.ClientUI.HealthInfo.text = GameManager.Instance.Players[playerHit.PlayerHitID].Health + "/" + ClientBehaviour.Instance.Clients[playerHit.PlayerHitID].StartHealth + " HP";
                        }

                        string playerHitMessage = GameManager.Instance.Players[playerHit.PlayerHitID].Client.ClientName + " took " + playerHit.DamageDealt + " damage.";
                        ClientBehaviour.Instance.ClientUI.ShowMessage(playerHitMessage);


                        break;
                    case Message.MessageType.PlayerDefends:
                        var playerDefend = new PlayerDefend();
                        playerDefend.DeserializeObject(ref reader);

                        if (playerDefend.PlayerDefendingID == ClientBehaviour.Instance.PlayerID)
                        {
                            ClientBehaviour.Instance.ClientUI.HealthInfo.text = GameManager.Instance.Players[playerDefend.PlayerDefendingID].Health + "/" + ClientBehaviour.Instance.Clients[playerDefend.PlayerDefendingID].StartHealth + " HP";
                        }

                        string playerDefendMessage = GameManager.Instance.Players[playerDefend.PlayerDefendingID].Client.ClientName + " defended hismelf from the monster, and healed for 1 HP.";
                        ClientBehaviour.Instance.ClientUI.ShowMessage(playerDefendMessage);

                        break;
                    case Message.MessageType.PlayerLeftDungeon:
                        var playerLeft = new PlayerLeftDungeon();
                        playerLeft.DeserializeObject(ref reader);

                        string leaveMessage = GameManager.Instance.Players[playerLeft.PlayerLeftID].Client.ClientName + " has left the dungeon.";
                        ClientBehaviour.Instance.ClientUI.ShowMessage(leaveMessage);

                        GameManager.Instance.Players[playerLeft.PlayerLeftID].gameObject.SetActive(false);
                        GameManager.Instance.Players[playerLeft.PlayerLeftID].noTurn = true;
                        GameManager.Instance.LeftPlayers.Add(GameManager.Instance.Players[playerLeft.PlayerLeftID]);

                        break;
                    case Message.MessageType.PlayerDies:
                        var playerDies = new PlayerDies();
                        playerDies.DeserializeObject(ref reader);

                        string deathMessage = GameManager.Instance.Players[playerDies.PlayerDeathID].Client.ClientName + " has died.";
                        ClientBehaviour.Instance.ClientUI.ShowMessage(deathMessage);

                        if (playerDies.PlayerDeathID == ClientBehaviour.Instance.PlayerID)
                        {
                            ClientBehaviour.Instance.ClientUI.HealthInfo.text = "0/" + ClientBehaviour.Instance.Clients[playerDies.PlayerDeathID].StartHealth + " HP";
                        }

                        GameManager.Instance.Players[playerDies.PlayerDeathID].gameObject.SetActive(false);
                        GameManager.Instance.Players[playerDies.PlayerDeathID].noTurn = true;
                        GameManager.Instance.DeadPlayers.Add(GameManager.Instance.Players[playerDies.PlayerDeathID]);
                        break;
                    case Message.MessageType.EndGame:
                        ClientBehaviour.Instance.ClientUI.ShowMessage("Game Over");

                        UnityEngine.SceneManagement.SceneManager.LoadScene("EndGame");

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
            else
            {
                //Serverside Handling
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
                    case Message.MessageType.MoveRequest: //Handle Player Leaving the room.
                        var moveRequest = new MoveRequest();
                        moveRequest.DeserializeObject(ref reader);

                        //Remove Player form room.
                        GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Remove(GameManager.Instance.Players[ConnectionID]);

                        #region RoomLeaveInfo
                        //Send to other clients in room that player left.
                        var playerLeave = new PlayerLeaveRoom()
                        {
                            PlayerID = ConnectionID
                        };

                        for (int j = 0; j < GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count; j++)
                        {
                            var writer = ServerBehaviour.Instance.m_Driver.BeginSend(
                                ServerBehaviour.Instance.m_Connections[
                                    GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom[j].Client.ClientID
                                    ]);

                            playerLeave.SerializeObject(ref writer);
                            ServerBehaviour.Instance.m_Driver.EndSend(writer);
                        }
                        #endregion

                        #region SetPlayerPosition
                        //Set new room position.

                        Vector2 currentGridPosition = GameManager.Instance.Players[ConnectionID].CurrentRoom.GridPosition;
                        Vector2 positionTranslation = GameManager.Instance.Players[ConnectionID].CurrentRoom.MoveToNeighbour(moveRequest.MoveDirection);

                        Vector2Int newPosition = new Vector2Int((int)currentGridPosition.x + (int)positionTranslation.x, (int)currentGridPosition.y + (int)positionTranslation.y);
                        #endregion

                        //Set new room variable.
                        GameManager.Instance.Players[ConnectionID].CurrentRoom = GameManager.Instance.Grid.RoomGrid[newPosition.x, newPosition.y];
                        GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Add(GameManager.Instance.Players[ConnectionID]);

                        #region RoomEnterInfo
                        //Send to other clients in room that player entered.
                        var playerJoin = new PlayerEnterRoom()
                        {
                            PlayerID = ConnectionID
                        };

                        //Send to other clients.
                        for (int e = 0; e < GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count; e++)
                        {
                            var writer = ServerBehaviour.Instance.m_Driver.BeginSend(
                                ServerBehaviour.Instance.m_Connections[
                                    GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom[e].Client.ClientID
                                    ]);

                            playerJoin.SerializeObject(ref writer);
                            ServerBehaviour.Instance.m_Driver.EndSend(writer);
                        }
                        #endregion

                        #region RoomInfo
                        //Make new room info.
                        var roomInfo = new RoomInfo()
                        {
                            MoveDirections = (byte)GameManager.Instance.Players[ConnectionID].CurrentRoom.possibleDirections,
                            TreasureInRoom = GameManager.Instance.Players[ConnectionID].CurrentRoom.TreasureAmount,
                            ContainsMonster = GameManager.Instance.Players[ConnectionID].CurrentRoom.ContainsMonster,
                            ContainsExit = GameManager.Instance.Players[ConnectionID].CurrentRoom.ContainsExit,
                            NumberOfOtherPlayers = (byte)GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count
                        };

                        //Search for ID's of players already in room.
                        int[] newRoomIDs = new int[GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count];
                        for (int i = 0; i < GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count; i++)
                        {
                            newRoomIDs[i] = GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom[i].Client.ClientID;
                        }
                        #endregion

                        //Send Room info to client that is moving room.
                        var writer1 = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[ConnectionID]);
                        roomInfo.SerializeObject(ref writer1, newRoomIDs);
                        ServerBehaviour.Instance.m_Driver.EndSend(writer1);

                        //Set next player's turn.
                        GameManager.Instance.NextTurn();

                        break;
                    case Message.MessageType.AttackRequest:

                        GameManager.Instance.Players[ConnectionID].CurrentRoom.ContainsMonster = 0;

                        var attackFeedback = new HitMonster()
                        {
                            PlayerAttackingID = ConnectionID,
                            DamageDealt = 1
                        };

                        //Experiment: use foreach instead of for loop. conclusion: works pretty good and i'm an idiot for not using it everywhere else.
                        foreach (Player player in GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom)
                        {

                            var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[player.Client.ClientID]);
                            attackFeedback.SerializeObject(ref writer);
                            ServerBehaviour.Instance.m_Driver.EndSend(writer);


                            //If the player's health is smaller than 2. the player dies of an attack by the monster. Else it'll take 2 damage.
                            if (GameManager.Instance.Players[ConnectionID].Health > 2)
                            {
                                //the player that attacks also gets hit by the enemy.
                                var attackPlayer = new HitByMonster()
                                {
                                    PlayerHitID = ConnectionID,
                                    DamageDealt = 2
                                };

                                var writer2 = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[player.Client.ClientID]);
                                attackPlayer.SerializeObject(ref writer2);
                                ServerBehaviour.Instance.m_Driver.EndSend(writer2);
                            }
                            else
                            {
                                if (GameManager.Instance.Players.Count == (GameManager.Instance.DeadPlayers.Count + 1))
                                {
                                    GameManager.Instance.EndGame();
                                }

                                var playerDeath = new PlayerDies()
                                {
                                    PlayerDeathID = ConnectionID
                                };

                                var writer2 = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[player.Client.ClientID]);
                                playerDeath.SerializeObject(ref writer2);
                                ServerBehaviour.Instance.m_Driver.EndSend(writer2);
                            }
                        }


                        GameManager.Instance.NextTurn();

                        break;
                    case Message.MessageType.DefendRequest:
                        var defendRequest = new DefendRequest();

                        ushort newPlayerHP = GameManager.Instance.Players[ConnectionID].Health++;

                        var defendFeedback = new PlayerDefend()
                        {
                            PlayerDefendingID = ConnectionID,
                            NewHP = newPlayerHP
                        };

                        foreach (Player player in GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom)
                        {
                            var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[player.Client.ClientID]);
                            defendFeedback.SerializeObject(ref writer);
                            ServerBehaviour.Instance.m_Driver.EndSend(writer);
                        }


                        GameManager.Instance.NextTurn();
                        break;
                    case Message.MessageType.ClaimTreasureRequest:

                        int dividedAmount = GameManager.Instance.Players[ConnectionID].CurrentRoom.TreasureAmount / System.Convert.ToUInt16(GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count);

                        #region SendTreasureAmount
                        //Send treasure amount to all players in room.
                        var treasureObtain = new ObtainTreasure()
                        {
                            TreasureAmount = (ushort)dividedAmount
                        };

                        //Send to other clients.
                        for (int e = 0; e < GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom.Count; e++)
                        {
                            var writer = ServerBehaviour.Instance.m_Driver.BeginSend(
                                ServerBehaviour.Instance.m_Connections[
                                    GameManager.Instance.Players[ConnectionID].CurrentRoom.PlayersInRoom[e].Client.ClientID
                                    ]);

                            treasureObtain.SerializeObject(ref writer);
                            ServerBehaviour.Instance.m_Driver.EndSend(writer);
                        }
                        #endregion

                        GameManager.Instance.Players[ConnectionID].CurrentRoom.TreasureAmount = 0;
                        GameManager.Instance.NextTurn();

                        break;
                    case Message.MessageType.LeaveDungeonRequest:

                        var dungeonLeftMessage = new PlayerLeftDungeon
                        {
                            PlayerLeftID = ConnectionID
                        };

                        for (int i = 0; i < ServerBehaviour.Instance.Clients.Count; i++)
                        {
                            var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[i]);
                            dungeonLeftMessage.SerializeObject(ref writer);
                            ServerBehaviour.Instance.m_Driver.EndSend(writer);
                        }

                        if (GameManager.Instance.Players.Count == (GameManager.Instance.LeftPlayers.Count + 1))
                        {
                            GameManager.Instance.EndGame();
                        }
                        else
                        {
                            //Set next player's turn.
                            GameManager.Instance.NextTurn();
                        }
                        break;
                }
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
