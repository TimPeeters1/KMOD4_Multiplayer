using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientUI : MonoBehaviour
{
    public Text CommandLine;
    public Text PlayerInfo;
    public Text HealthInfo;

    public void ShowMessage(string _message)
    {
        CommandLine.text = _message;
    }
}
