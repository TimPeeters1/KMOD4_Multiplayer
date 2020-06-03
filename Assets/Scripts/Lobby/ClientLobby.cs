using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class ClientLobby : MonoBehaviour
    {
        public ClientBehaviour Client;

        GameObject lobbyMenu;

        public string PlayerName;

        void Start()
        {
            Client = this.gameObject.AddComponent<ClientBehaviour>();
            Client.PlayerName = PlayerName;
            Client.Lobby = this;

            lobbyMenu = Instantiate(Resources.Load("ClientLobby") as GameObject);
            Button b = lobbyMenu.transform.Find("Disconnect").GetComponent<Button>();
            b.onClick.AddListener(delegate () { Client.ClientDisconnect(); });
        }

        public void PrintMessage(string _message, Color _color)
        {
            Text _textObject = lobbyMenu.transform.Find("Commandline").GetComponent<Text>();
            _textObject.text = _message;
            _textObject.color = _color;
        }
    }
}
