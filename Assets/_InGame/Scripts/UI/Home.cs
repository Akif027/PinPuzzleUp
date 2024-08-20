using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    Button HomeButton;

    void Start()
    {

        HomeButton = GetComponent<Button>();
        HomeButton.onClick.AddListener(GoTomenu);

    }


    void GoTomenu()
    {

        CustomSceneManager.LoadSceneAsync("Menu");

    }


    void OnDisable()
    {

        HomeButton.onClick.RemoveAllListeners();
    }
}
