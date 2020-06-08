using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class Player : MonoBehaviour
    {
        public ServerClient Client;
        public Room CurrentRoom; //Only known on serverside.

        void Start()
        {
            GetComponentInChildren<Light>().color = Client.ClientColour;
            GetComponentInChildren<UnityEngine.UI.Text>().text = Client.ClientName;
            GetComponentInChildren<Canvas>().transform.forward = -Camera.main.transform.forward;
        }
    }
}
