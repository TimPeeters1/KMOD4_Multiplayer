using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;
using System.Linq;

namespace MutiplayerSystem
{
    public enum GameState
    {
        Lobby,
        InGame
    }

    [System.Serializable]
    public class ServerClient
    {
        public int ClientID;
        public string ClientName;
        public Color ClientColour;
        public ushort HealthPoints;

        public ServerClient(int clientID, string clientName, Color clientColour)
        {
            ClientID = clientID;
            ClientName = clientName;
            ClientColour = clientColour;
        }
    }

    public class ServerBehaviour : MonoBehaviour
    {

        #region Singleton
        private static ServerBehaviour instance = null;
        public static ServerBehaviour Instance
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
        public NativeList<NetworkConnection> m_Connections;

        public GameState CurrentGameState;

        Message Message = new Message();

        public List<ServerClient> Clients = new List<ServerClient>();
        public HostLobby Lobby;

        #region DataHandling
        public LobbyDataHandler LobbyData = new LobbyDataHandler(false);
        public AliveMessageHandler aliveMessage;
        public GameDataHandler GameData = new GameDataHandler(false);
        #endregion

        void Start()
        {
            CurrentGameState = GameState.Lobby;

            m_Driver = NetworkDriver.Create();
            var endpoint = NetworkEndPoint.AnyIpv4;
            endpoint.Port = 9000;
            if (m_Driver.Bind(endpoint) != 0)
            {
                Debug.Log("Failed to bind to port 9000");
            }
            else
            {
                m_Driver.Listen();
            }

            
            m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

            aliveMessage = new GameObject().AddComponent<AliveMessageHandler>();
            aliveMessage.InitializeMessage(3f, false);
            aliveMessage.gameObject.name = "AliveMessage_Server";
            DontDestroyOnLoad(aliveMessage);
        }

        public void OnDestroy()
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }

        void Update()
        {
            m_Driver.ScheduleUpdate().Complete();

            // Clean up connections
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (!m_Connections[i].IsCreated)
                {
                    m_Connections.RemoveAtSwapBack(i);
                    --i;
                }
            }

            // Accept new connections
            NetworkConnection c;
            while ((c = m_Driver.Accept()) != default(NetworkConnection) && Clients.Count < 4)
            {
                m_Connections.Add(c);
                Debug.Log("Accepted a connection");

                //Send Welcome Message to Client
                var color = Lobby.ColorList[Clients.Count];
                uint encodedColor = (uint)DataUtilities.EncodeColor(color);
                //uint colourInt = ((uint)colour.r << 24 | (uint)colour.g << 16 | (uint)colour.b << 8 | (uint)colour.a);

                ServerClient newClient = new ServerClient(c.InternalId, "", color);
                newClient.HealthPoints = Lobby.PlayerStartHealth;

                Clients.Add(newClient);

                var message = new WelcomeMessage
                {
                    PlayerID = c.InternalId,
                    Colour = encodedColor,
                };

                var writer = m_Driver.BeginSend(c);
                message.SerializeObject(ref writer);
                m_Driver.EndSend(writer);

            }

            DataStreamReader reader;
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (!m_Connections[i].IsCreated)
                    continue;


                NetworkEvent.Type cmd;
                while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out reader)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        var messageType = (Message.MessageType)reader.ReadUShort();

                        switch (CurrentGameState)
                        {
                            case GameState.Lobby:
                                LobbyData.HandleMessages(messageType, reader, i);
                                break;
                            case GameState.InGame:
                                GameData.HandleMessages(messageType, reader, i);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        for (int j = 0; j < m_Connections.Length; j++)
                        {
                            var leftMessage = new PlayerLeft
                            {
                                PlayerDisconnectID = Clients[i].ClientID,
                            };

                            var write = m_Driver.BeginSend(m_Connections[j]);
                            leftMessage.SerializeObject(ref write);
                            m_Driver.EndSend(write);
                        }

                        Clients.RemoveAt(i);
                        Lobby.LobbyUI.GetComponent<PlayerLobbyList>().RemoveFromList(i);

                        m_Connections[i] = default(NetworkConnection);
                    }
                }
            }
        }

        public void StartGame()
        {
            CurrentGameState = GameState.InGame;
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

            for (int i = 0; i < m_Connections.Length; i++)
            {
                var startGame = new StartGame()
                {
                    StartHP = Lobby.PlayerStartHealth
                };

                var writer = m_Driver.BeginSend(m_Connections[i]);
                startGame.SerializeObject(ref writer);
                m_Driver.EndSend(writer);
            }
        }

    }
}
