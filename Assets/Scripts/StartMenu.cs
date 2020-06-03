using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class StartMenu : MonoBehaviour
    {
        public InputField field;
        public string ClientName;

        private void Start()
        {
            var se = new InputField.SubmitEvent();
            se.AddListener(SubmitName);
            field.onEndEdit = se;
        }

        public void CreateLobby()
        {
            GameObject LobbyObject = new GameObject();
            LobbyObject.name = "HostLobby";
            LobbyObject.AddComponent<HostLobby>();
            LobbyObject.AddComponent<ClientBehaviour>().PlayerName = ClientName;
            LobbyObject.GetComponent<ClientBehaviour>().IsHost = true;

            this.gameObject.SetActive(false);
        }

        public void CreateClient()
        {
            GameObject LobbyObject = new GameObject();
            LobbyObject.name = "ClientLobby";
            LobbyObject.AddComponent<ClientLobby>().PlayerName = ClientName;


            this.gameObject.SetActive(false);
        }

        private void SubmitName(string _name)
        {
            ClientName = _name;
        }
    }
}
