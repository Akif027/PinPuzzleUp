
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ModeManager : MonoBehaviour
{


    [SerializeField] Button PyramidB;
    [SerializeField] Button VeagasB;
    [SerializeField] Button ArtB;


    void Start()
    {

        PyramidB.onClick.AddListener(() => ThePyramidMode());
        VeagasB.onClick.AddListener(() => TheVegasMode());
        ArtB.onClick.AddListener(() => TheArtMode());

    }

    void ThePyramidMode()
    {
        SoundManager.Instance.PlayOnButtonPress();
        IPlayerPrefs.SetMode((int)PatternType.Pyramid);
        LoadModeScene();

    }

    void TheVegasMode()
    {
        SoundManager.Instance.PlayOnButtonPress();
        IPlayerPrefs.SetMode((int)PatternType.Vegas);
        LoadModeScene();

    }
    void TheArtMode()
    {
        SoundManager.Instance.PlayOnButtonPress();
        IPlayerPrefs.SetMode((int)PatternType.Art);
        LoadModeScene();

    }

    void LoadModeScene()
    {
        CustomSceneManager.LoadSceneAsync("Game");

    }
    void OnDisable()
    {

        PyramidB.onClick.RemoveAllListeners();
        VeagasB.onClick.RemoveAllListeners();
        ArtB.onClick.RemoveAllListeners();
    }
}
