using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class HostLobby : MonoBehaviour
    {
        public ServerBehaviour Server;
        public GameObject LobbyUI;

        public Color[] ColorList = { Color.red, Color.green, Color.magenta, Color.blue };
        public ushort PlayerStartHealth = 10;

        // Start is called before the first frame update
        void Start()
        {
            GameObject _serverObject = new GameObject();
            _serverObject.name = "Server";
            Server = _serverObject.AddComponent<ServerBehaviour>() as ServerBehaviour;
            Server.Lobby = this;

            LobbyUI = Instantiate(Resources.Load("HostLobby") as GameObject);
            Button b = LobbyUI.transform.Find("StartGame").GetComponent<Button>();
            b.onClick.AddListener(delegate () { Server.StartGame(); });
        }

       
    }
}
