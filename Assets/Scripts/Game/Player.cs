using MutiplayerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public ServerClient Client;

    void Start()
    {
        GetComponentInChildren<Light>().color = Client.ClientColour;
        GetComponentInChildren<UnityEngine.UI.Text>().text = Client.ClientName;
        GetComponentInChildren<Canvas>().transform.LookAt(Camera.main.transform);
    }
}
