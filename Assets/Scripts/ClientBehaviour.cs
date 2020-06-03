using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

namespace MutiplayerSystem
{
    public class ClientBehaviour : MonoBehaviour
    {

        #region Singleton
        private static ClientBehaviour instance = null;
        public static ClientBehaviour Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            // if the singleton hasn't been initialized yet
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        public NetworkDriver m_Driver;
        public NetworkConnection m_Connection;
        public bool Done;

        //Client Variables
        public string PlayerName;
        public int PlayerID;

        public ClientLobby Lobby;
        GameState currentGameState;

        public bool IsHost;

        #region DataHandling
        public LobbyDataHandler LobbyData = new LobbyDataHandler(true);
        AliveMessageHandler aliveMessage;
        public GameDataHandler GameData = new GameDataHandler(true);
        #endregion

        void Start()
        {
            currentGameState = GameState.Lobby;

            m_Driver = NetworkDriver.Create();
            m_Connection = default(NetworkConnection);

            //var endpoint = NetworkEndPoint.Parse("77.167.147.11", 9000);
            var endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = 9000;
            m_Connection = m_Driver.Connect(endpoint);

            aliveMessage = new GameObject().AddComponent<AliveMessageHandler>();
            aliveMessage.InitializeMessage(3f, true);
            aliveMessage.gameObject.name = "AliveMessage_Client";
            DontDestroyOnLoad(aliveMessage);
        }

        public void OnDestroy()
        {
            m_Driver.Dispose();
        }

        void Update()
        {
            m_Driver.ScheduleUpdate().Complete();

            if (!m_Connection.IsCreated)
            {
                if (!Done)
                    if (!IsHost)
                        Lobby.PrintMessage("Can't Connect to Server!", Color.red);
                return;
            }

            DataStreamReader reader;
            NetworkEvent.Type cmd;
            while ((cmd = m_Connection.PopEvent(m_Driver, out reader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {

                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    var messageType = (Message.MessageType)reader.ReadUShort();

                    switch (currentGameState)
                    {
                        case GameState.Lobby:
                            LobbyData.HandleMessages(messageType, reader, m_Connection.InternalId);
                            break;
                        case GameState.InGame:
                            GameData.HandleMessages(messageType, reader, m_Connection.InternalId);
                            break;
                        default:
                            break;
                    }

                    /*
                    switch (messageType)
                    {
                        case Message.MessageType.NewPlayer:
                            var newPlayer = new NewPlayer();
                            newPlayer.DeserializeObject(ref reader);
                            Debug.Log(newPlayer.PlayerName + " " + newPlayer.PlayerID + " " + newPlayer.Colour);

                            if (!IsHost)
                                Lobby.PrintMessage(newPlayer.PlayerName + ", joined the game.", Color.green);

                            break;

                        case Message.MessageType.Welcome:
                            var welcomeMessage = new WelcomeMessage();
                            welcomeMessage.DeserializeObject(ref reader);
                            //Debug.Log(welcomeMessage.ID + " " + welcomeMessage.PlayerID + " " + welcomeMessage.Colour);


                            //Send Name
                            var nameMessage = new SetNameMessage
                            {
                                Name = PlayerName
                            };

                            var writer = m_Driver.BeginSend(m_Connection);
                            nameMessage.SerializeObject(ref writer);
                            m_Driver.EndSend(writer);

                            if (!IsHost)
                                Lobby.PrintMessage("Succesfully Connected to the Server.", Color.green);

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

                            if (!IsHost)
                                Lobby.PrintMessage("Player " + playerLeft.PlayerDisconnectID + " has disconnected.", Color.yellow);
                            break;
                        case Message.MessageType.StartGame:
                            if (!IsHost)
                                Lobby.PrintMessage("Game Starting...", Color.blue);
                            break;
                        case Message.MessageType.None:
                        default:
                            break;
                    }
                    */
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    m_Connection = default(NetworkConnection);
                }
            }
        }

        public void ClientDisconnect()
        {
            if (!IsHost)
                Lobby.PrintMessage("Disconnected from server!", Color.red);

            m_Connection.Disconnect(m_Driver);
            m_Connection = default(NetworkConnection);
        }
    }
}