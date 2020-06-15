using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class Player : MonoBehaviour
    {
        public ServerClient Client;
        public Room CurrentRoom; //Only known serverside.
        public ushort PlayerTreasureAmount;
        public bool noTurn; //If noturn is true, this player cannot peform a turn as it already left the dungeon or when it died.
        public ushort Health;

        void Start()
        {
            GetComponentInChildren<Light>().color = Client.ClientColour;
            GetComponentInChildren<UnityEngine.UI.Text>().text = Client.ClientName;
            GetComponentInChildren<Canvas>().transform.forward = -Camera.main.transform.forward;
        }
    }
}
