using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Records : MonoBehaviour
{
    public TextMeshProUGUI[] scoreTextFields;
    public Button PyramidB;
    public Button VegasB;
    public Button ArtB;
    private List<int> scores = new List<int>();

    private const int maxScores = 5;

    void Start()
    {

        PyramidB.onClick.AddListener(HandlePyramidMode);
        PyramidB.onClick.Invoke();
        EventSystem.current.SetSelectedGameObject(PyramidB.gameObject);
        VegasB.onClick.AddListener(HandleVegasMode);
        ArtB.onClick.AddListener(HandleArtMode);

    }
    void HandlePyramidMode()
    {
        SoundManager.Instance.PlayOnButtonPress();
        LoadPyramidScores();
        DisplayScores();

    }

    void HandleVegasMode()
    {
        SoundManager.Instance.PlayOnButtonPress();
        LoadVegasScores();
        DisplayScores();

    }
    void HandleArtMode()
    {
        SoundManager.Instance.PlayOnButtonPress();
        LoadArtScores();
        DisplayScores();

    }
    private void LoadPyramidScores()
    {
        scores.Clear(); // Clear the current scores list

        for (int i = 0; i < maxScores; i++)
        {
            string key = IPlayerPrefs.GetPyramidScoreKey(i);
            if (PlayerPrefs.HasKey(key))
            {
                int score = PlayerPrefs.GetInt(key);
                Debug.Log($"Loaded {key}: {score}"); // Debug log
                scores.Add(score);
            }
        }

        // Ensure scores list has exactly maxScores elements
        while (scores.Count < maxScores)
        {
            scores.Add(0); // Add default value if fewer scores are loaded
        }
    }
    private void LoadVegasScores()
    {
        scores.Clear(); // Clear the current scores list

        for (int i = 0; i < maxScores; i++)
        {
            string key = IPlayerPrefs.GetVegasScoreKey(i);
            if (PlayerPrefs.HasKey(key))
            {
                int score = PlayerPrefs.GetInt(key);
                Debug.Log($"Loaded {key}: {score}"); // Debug log
                scores.Add(score);
            }
        }

        // Ensure scores list has exactly maxScores elements
        while (scores.Count < maxScores)
        {
            scores.Add(0); // Add default value if fewer scores are loaded
        }
    }
    private void LoadArtScores()
    {
        scores.Clear(); // Clear the current scores list

        for (int i = 0; i < maxScores; i++)
        {
            string key = IPlayerPrefs.GetArtScoreKey(i);
            if (PlayerPrefs.HasKey(key))
            {
                int score = PlayerPrefs.GetInt(key);
                Debug.Log($"Loaded {key}: {score}"); // Debug log
                scores.Add(score);
            }
        }

        // Ensure scores list has exactly maxScores elements
        while (scores.Count < maxScores)
        {
            scores.Add(0); // Add default value if fewer scores are loaded
        }
    }

    private void DisplayScores()
    {
        // Ensure that scores are displayed from top to bottom
        int scoreCount = scores.Count;

        for (int i = 0; i < scoreTextFields.Length; i++)
        {
            int scoreIndex = i; // Latest score is at index 0, so start from 0

            if (scoreIndex < scoreCount)
            {
                scoreTextFields[i].text = $"{scores[scoreIndex]}";
            }
            else
            {
                scoreTextFields[i].text = "0";
            }
        }
    }



}
