using MutiplayerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public List<Player> Players = new List<Player>();
    public Room CurrentRoom;

    [SerializeField] GameObject playerPrefab;

    private void Start()
    {
        CurrentRoom = FindObjectOfType<Room>();

        for (int i = 0; i < ClientBehaviour.Instance.Clients.Count; i++)
        {
            GameObject player = Instantiate(playerPrefab, CurrentRoom.spawnPositions[i].transform.position, CurrentRoom.spawnPositions[i].transform.rotation);
            player.GetComponent<Player>().Client = ClientBehaviour.Instance.Clients[i];
            Players.Add(player.GetComponent<Player>());
        }
    }
}
