using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject StartMenu, LoginMenu, RegisterMenu;
    [SerializeField] string Username, Password;

    [Header("Login Variables")]
    public InputField UserNameInput;
    public InputField PasswordInput;

    private void Start()
    {
        var se = new InputField.SubmitEvent();
        se.AddListener(SetUsername);
        UserNameInput.onEndEdit = se;

        var se1 = new InputField.SubmitEvent();
        se1.AddListener(SetPassword);
        PasswordInput.onEndEdit = se1;
    }

    private void SetUsername(string _name)
    {
        Username = _name;
    }

    private void SetPassword(string _password)
    {
        Password = _password;
    }

    #region MenuLogic
    public void OpenStart()
    {
        StartMenu.SetActive(true);
        LoginMenu.SetActive(false);
        RegisterMenu.SetActive(false);
    }

    public void OpenLogin()
    {
        StartMenu.SetActive(false);
        LoginMenu.SetActive(true);
        RegisterMenu.SetActive(false);
    }

    public void OpenRegister()
    {
        StartMenu.SetActive(false);
        LoginMenu.SetActive(false);
        RegisterMenu.SetActive(true);
    }
    #endregion

    public void PlayerLogin()
    {
        DatabaseManager.Instance.ClientLogin(Username, Password);
    }
}
