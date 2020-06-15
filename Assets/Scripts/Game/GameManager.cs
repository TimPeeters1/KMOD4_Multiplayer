using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MutiplayerSystem
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager instance = null;
        public static GameManager Instance
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
            //DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        public Player PlayerCurrentTurn;
        public List<Player> Players = new List<Player>();
        public List<Player> LeftPlayers = new List<Player>();
        public List<Player> DeadPlayers = new List<Player>();

        [Space]
        public RoomObject RoomGameObject;
        public GridGeneration Grid;

        [Space]
        [SerializeField] GameObject playerPrefab;

        [Header("UI")]
        public Text PlayerTurnUI; //UI that tells which players turn it is

        Interactable HoveredObject = null;

        private void Start()
        {
            //CurrentRoom.SetRoomDirections((Room.RoomDirections)UnityEngine.Random.Range((int)1, (int)15));

            //Setup players in current room.
            for (int i = 0; i < ClientBehaviour.Instance.Clients.Count; i++)
            {
                GameObject player = Instantiate(playerPrefab, RoomGameObject.spawnPositions[i].transform.position, RoomGameObject.spawnPositions[i].transform.rotation);
                player.name = ClientBehaviour.Instance.Clients[i].ClientName;
                player.GetComponent<Player>().Health = ClientBehaviour.Instance.Clients[i].StartHealth;
                player.GetComponent<Player>().Client = ClientBehaviour.Instance.Clients[i];

                Players.Add(player.GetComponent<Player>());
                player.SetActive(false);
            }

            if (ClientBehaviour.Instance.IsHost) //Send game info on start of game.
            {
                //Generate Grid if host.
                Grid = GetComponent<GridGeneration>();
                Grid.GenerateGrid();

                for (int i = 0; i < Players.Count; i++)
                {
                    Grid.RoomGrid[0, 0].PlayersInRoom.Add(Players[i]);
                    Players[i].CurrentRoom = Grid.RoomGrid[0, 0];
                }

                //Set player turn
                PlayerCurrentTurn = Players[0];

                #region RoomInfoMessage
                //Send Room Info
                var roomInfo = new RoomInfo()
                {
                    MoveDirections = (byte)Grid.RoomGrid[0, 0].possibleDirections,
                    TreasureInRoom = Grid.RoomGrid[0,0].TreasureAmount,
                    ContainsMonster = Grid.RoomGrid[0, 0].ContainsMonster,
                    ContainsExit = Grid.RoomGrid[0, 0].ContainsExit,
                    NumberOfOtherPlayers = (byte)ServerBehaviour.Instance.Clients.Count
                };

                int[] playerIDs = new int[roomInfo.NumberOfOtherPlayers];
                for (int f = 0; f < roomInfo.NumberOfOtherPlayers; f++)
                {
                    playerIDs[f] = ServerBehaviour.Instance.Clients[f].ClientID;
                    //Debug.Log(playerIDs[f]);
                }
                #endregion

                #region NextPlayerMessage
                //Send which player's turn it is.
                var playerTurn = new PlayerTurn
                {
                    PlayerID = PlayerCurrentTurn.Client.ClientID
                };

                #endregion

                #region SendMessages
                //Send Messages.
                for (int i = 0; i < ServerBehaviour.Instance.Clients.Count; i++)
                {
                    var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[i]);
                    roomInfo.SerializeObject(ref writer, playerIDs);
                    ServerBehaviour.Instance.m_Driver.EndSend(writer);

                    var writer1 = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[i]);
                    playerTurn.SerializeObject(ref writer1);
                    ServerBehaviour.Instance.m_Driver.EndSend(writer1);
                }
                #endregion

            }
        }

        private void Update()
        {
            if (PlayerCurrentTurn != null && PlayerCurrentTurn.Client.ClientID == ClientBehaviour.Instance.PlayerID)
            {
                DoSelection();
            }
        }

        void DoSelection()
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Interactable>())
            {
                HoveredObject = hit.collider.gameObject.GetComponent<Interactable>();
                HoveredObject.IsHovered = true;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    HoveredObject.DoInteraction();
                }
            }
            else if (HoveredObject != null)
            {
                if (HoveredObject.GetComponent<Interactable>().IsHovered == true)
                {
                    HoveredObject.GetComponent<Interactable>().IsHovered = false;
                }
            }
        }

        public void NextTurn()
        {
            int i = Players.IndexOf(PlayerCurrentTurn);
            
            if(i != (Players.Count - 1))
            {
                i++;
            }
            else
            {
                i = 0;
            }

            PlayerCurrentTurn = Players[i];

            if (PlayerCurrentTurn.noTurn)
            {
                NextTurn();
                return;
            }

            var nextTurn = new PlayerTurn
            {
                PlayerID = GameManager.Instance.PlayerCurrentTurn.Client.ClientID
            };

            //Send which player's turn it is.
            for (int f = 0; f < ServerBehaviour.Instance.Clients.Count; f++)
            {
                var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[f]);
                nextTurn.SerializeObject(ref writer);
                ServerBehaviour.Instance.m_Driver.EndSend(writer);
            }
        }

        public void EndGame()
        {
            var endGame = new EndGame()
            {
                NumberOfScores = (byte)GameManager.Instance.LeftPlayers.Count
            };

            for (int i = 0; i < endGame.NumberOfScores; i++)
            {
                endGame.PlayerScorePair.Add(GameManager.Instance.LeftPlayers[i].Client.ClientID, GameManager.Instance.LeftPlayers[i].PlayerTreasureAmount);
            }

            for (int i = 0; i < ServerBehaviour.Instance.Clients.Count; i++)
            {
                var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[i]);
                endGame.SerializeObject(ref writer);
                ServerBehaviour.Instance.m_Driver.EndSend(writer);
            }
        }

    }
}
