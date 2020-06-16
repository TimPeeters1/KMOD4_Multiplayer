using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    public void OpenStatistics()
    {
        Application.OpenURL("https://studenthome.hku.nl/~tim.peeters/Database/Statistics.php");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
