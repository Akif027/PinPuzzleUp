using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIhandler : MonoBehaviour
{
    [SerializeField] TMP_Text TitleName;
    [SerializeField] GameObject Game_ExitUIPanel;
    [SerializeField] GameObject Game_FinishedUIPanel;

    public static UIhandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        TitleName.text = GameManager.Instance.GetpatternType.ToString();
        Game_FinishedUIPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = TitleName.text;
        Game_ExitUIPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(CloseExitMatchPanel);
        Game_ExitUIPanel.SetActive(false);
        Game_FinishedUIPanel.SetActive(false);


    }
    public void EndGame()
    {
        SoundManager.Instance?.PlayEndTheGame();
        Game_FinishedUIPanel.SetActive(true);

    }
    public void OpenExitMatchPanel()
    {
        SoundManager.Instance?.PlayOnButtonPress();
        Game_ExitUIPanel.SetActive(true);
    }

    public void CloseExitMatchPanel()
    {
        SoundManager.Instance?.PlayOnButtonPress();
        Game_ExitUIPanel.SetActive(false);
    }
    public void LoadTheScene(string S)
    {
        SoundManager.Instance?.PlayOnButtonPress();
        CustomSceneManager.LoadScene(S);

    }
    public void Restart()
    {
        SoundManager.Instance?.PlayOnButtonPress();
        CustomSceneManager.ReloadCurrentScene();

    }

}
