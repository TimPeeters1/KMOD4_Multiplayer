using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class HostLobby : MonoBehaviour
    {
        public ServerBehaviour Server;

        GameObject lobbyMenu;

        List<GameObject> PlayerUIList = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            GameObject _serverObject = new GameObject();
            _serverObject.name = "Server";
            Server = _serverObject.AddComponent<ServerBehaviour>() as ServerBehaviour;
            Server.Lobby = this;

            lobbyMenu = Instantiate(Resources.Load("HostLobby") as GameObject);
            Button b = lobbyMenu.transform.Find("StartGame").GetComponent<Button>();
            b.onClick.AddListener(delegate () { Server.StartGame(); });
        }

        public void UpdateLobby(ServerClient NewClient)
        {
            GameObject PlayerText = Instantiate(Resources.Load("PlayerLobbyName") as GameObject);
            PlayerText.transform.SetParent(lobbyMenu.transform.Find("PlayerList/Viewport/Content").transform);
            PlayerText.name = NewClient.ClientName;

            PlayerUIList.Add(PlayerText);

            PlayerText.GetComponent<Text>().text = "Player " + NewClient.ClientID + " | " + NewClient.ClientName;
            //PlayerText.GetComponent<Text>().color = 
        }

        public void RemoveFromList(int _index)
        {
            GameObject _clientUI = PlayerUIList[_index];
            PlayerUIList.RemoveAt(_index);
            Destroy(_clientUI);
        }
    }
}
