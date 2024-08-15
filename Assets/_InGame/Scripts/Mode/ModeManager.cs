
using UnityEngine;
using UnityEngine.UI;
public class ModeManager : MonoBehaviour
{

    [SerializeField] Button HomeB;
    [SerializeField] Button PyramidB;
    [SerializeField] Button VeagasB;
    [SerializeField] Button ArtB;


    void Start()
    {
        HomeB.onClick.AddListener(() => LoadTheScene("Menu"));
        PyramidB.onClick.AddListener(() => LoadTheScene("Pyramid"));
        VeagasB.onClick.AddListener(() => LoadTheScene("Vegas"));
        ArtB.onClick.AddListener(() => LoadTheScene("Art"));

    }

    void LoadTheScene(string S)
    {
        CustomSceneManager.LoadSceneAsync(S);

    }

}
