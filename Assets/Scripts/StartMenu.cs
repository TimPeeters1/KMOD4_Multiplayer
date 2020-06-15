using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class StartMenu : MonoBehaviour
    {
        public InputField UserNameInput;
        public InputField IP_Input;
        public InputField Port_Input;
        public Toggle LocalToggle;
        public bool LocalIP = true;
        public string ClientName;
        public string IP;
        public string Port;

        private void Start()
        {
            var se = new InputField.SubmitEvent();
            se.AddListener(SubmitName);
            UserNameInput.onEndEdit = se;

            var se1 = new InputField.SubmitEvent();
            se1.AddListener(SubmitIP);
            IP_Input.onEndEdit = se1;

            var se2 = new InputField.SubmitEvent();
            se2.AddListener(SubmitPort);
            Port_Input.onEndEdit = se2;

            LocalToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(LocalToggle); });
        }

        public void CreateLobby()
        {
            GameObject LobbyObject = new GameObject();
            LobbyObject.name = "HostLobby";
            LobbyObject.AddComponent<HostLobby>();
            LobbyObject.AddComponent<ClientBehaviour>().PlayerName = ClientName;
            LobbyObject.GetComponent<ClientBehaviour>().IsHost = true;
            LobbyObject.GetComponent<ClientBehaviour>().IsLocal = LocalIP;
            if (!LocalIP)
            {
                LobbyObject.GetComponent<ClientBehaviour>().IP = IP;
                LobbyObject.GetComponent<ClientBehaviour>().Port = System.Convert.ToUInt16(Port.ToString());
            }

            this.gameObject.SetActive(false);
        }

        public void CreateClient()
        {
            GameObject LobbyObject = new GameObject();
            LobbyObject.name = "ClientLobby";
            LobbyObject.AddComponent<ClientLobby>().PlayerName = ClientName;
            LobbyObject.GetComponent<ClientLobby>().IsLocal = LocalIP;
            if (!LocalIP)
            {
                LobbyObject.GetComponent<ClientLobby>().IP = IP;
                LobbyObject.GetComponent<ClientLobby>().Port = System.Convert.ToUInt16(Port.ToString());
            }

            this.gameObject.SetActive(false);
        }

        private void SubmitName(string _name)
        {
            ClientName = _name;
        }

        private void SubmitIP(string _IP)
        {
            IP = _IP;
        }

        private void SubmitPort(string _Port)
        {
            Port = _Port;
        }

        void ToggleValueChanged(Toggle change)
        {
            LocalIP = change.isOn;
        }
    }
}
