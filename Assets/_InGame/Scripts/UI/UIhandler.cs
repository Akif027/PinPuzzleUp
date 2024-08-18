using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        Game_ExitUIPanel.SetActive(false);
        Game_FinishedUIPanel.SetActive(false);


    }
    public void EndGame()
    {

        Game_FinishedUIPanel.SetActive(true);
    }
    public void LoadTheScene(string S)
    {
        CustomSceneManager.LoadSceneAsync(S);

    }
    public void Restart()
    {
        CustomSceneManager.ReloadCurrentScene();

    }

}
