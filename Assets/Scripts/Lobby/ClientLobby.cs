using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class ClientLobby : MonoBehaviour
    {
        public ClientBehaviour Client;
        public string PlayerName;

        public GameObject LobbyUI;
        Text CommandLine;

        void Start()
        {
            Client = this.gameObject.AddComponent<ClientBehaviour>();
            Client.PlayerName = PlayerName;
            Client.Lobby = this;

            LobbyUI = Instantiate(Resources.Load("ClientLobby") as GameObject);
            Button b = LobbyUI.transform.Find("Disconnect").GetComponent<Button>();
            b.onClick.AddListener(delegate () { Client.ClientDisconnect(); });

            CommandLine = LobbyUI.transform.Find("Commandline").GetComponent<Text>();
        }

        public void PrintMessage(string _message, Color _color)
        {
            CommandLine.text = _message;
            CommandLine.color = _color;
        }
    }
}
