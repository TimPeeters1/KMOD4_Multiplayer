using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MutiplayerSystem
{
    public class PlayerLobbyList : MonoBehaviour
    {
        List<GameObject> PlayerUIList = new List<GameObject>();

        public void UpdateLobby(ServerClient newClient)
        {
            GameObject playerText = Instantiate(Resources.Load("PlayerLobbyName") as GameObject);
            playerText.transform.SetParent(transform.Find("PlayerList/Viewport/Content").transform);
            playerText.name = newClient.ClientName;

            //Color playerColor = new Color32(newClient.ClientColour)   

            //playerText.transform.Find("PlayerColour").GetComponent<Image>().color = ;

            PlayerUIList.Add(playerText);

            playerText.GetComponent<Text>().text = "Player " + newClient.ClientID + " | " + newClient.ClientName;
            playerText.transform.GetChild(0).GetComponent<Image>().color = newClient.ClientColour;
        }

        public void RemoveFromList(int _index)
        {
            GameObject _clientUI = PlayerUIList[_index];
            PlayerUIList.RemoveAt(_index);
            Destroy(_clientUI);
        }
    }
}
