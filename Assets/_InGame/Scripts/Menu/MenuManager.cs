
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button PlayB;
    [SerializeField] Button RulesB;
    [SerializeField] Button RecordB;



    void Start()
    {
        PlayB.onClick.AddListener(() => LoadTheScene("Mode"));
        RulesB.onClick.AddListener(() => LoadTheScene("Rules"));
        RecordB.onClick.AddListener(() => LoadTheScene("Records"));

    }

    void LoadTheScene(string S)
    {
        CustomSceneManager.LoadSceneAsync(S);

    }
}
