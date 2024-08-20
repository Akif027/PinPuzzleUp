using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class IPlayerPrefs
{
    private const int maxScores = 5;
    private static List<int> Pyramidscores = new List<int>();
    private static List<int> Vegascores = new List<int>();
    private static List<int> Artscores = new List<int>();
    // private static List<int> scores = new List<int>();
    private static bool isScoreAdded = false;
    private static readonly string PyramidScoreKeyFormat = "GameScore_Pyramid";
    private static readonly string VegasScoreKeyFormat = "GameScore_Vegas";
    private static readonly string ArtScoreKeyFormat = "GameScore_Art";
    static IPlayerPrefs()
    {
        LoadScores(PyramidScoreKeyFormat, Pyramidscores);
        LoadScores(VegasScoreKeyFormat, Vegascores);
        LoadScores(ArtScoreKeyFormat, Artscores);
    }

    public static void AddPyramidScore(int score)
    {
        if (isScoreAdded) return; // Exit if the score has already been added

        if (Pyramidscores.Count >= maxScores)
        {
            Pyramidscores.RemoveAt(Pyramidscores.Count - 1); // Remove the oldest score
        }
        Pyramidscores.Insert(0, score); // Insert the new score at the top
        SaveScores(PyramidScoreKeyFormat, Pyramidscores);
        isScoreAdded = true; // Set the flag to true after adding the score
    }
    public static void AddVegasScore(int score)
    {
        if (isScoreAdded) return; // Exit if the score has already been added

        if (Vegascores.Count >= maxScores)
        {
            Vegascores.RemoveAt(Vegascores.Count - 1); // Remove the oldest score
        }
        Vegascores.Insert(0, score); // Insert the new score at the top
        SaveScores(VegasScoreKeyFormat, Vegascores);
        isScoreAdded = true; // Set the flag to true after adding the score
    }
    public static void AddArtScore(int score)
    {
        if (isScoreAdded) return; // Exit if the score has already been added

        if (Artscores.Count >= maxScores)
        {
            Artscores.RemoveAt(Artscores.Count - 1); // Remove the oldest score
        }
        Artscores.Insert(0, score); // Insert the new score at the top
        SaveScores(ArtScoreKeyFormat, Artscores);
        isScoreAdded = true; // Set the flag to true after adding the score
    }

    public static void ResetScoreFlag()
    {
        isScoreAdded = false; // Reset the flag if needed
    }

    public static List<int> GetScores(List<int> scores)
    {
        return new List<int>(scores); // Return a copy to avoid external modification
    }

    private static void SaveScores(string key, List<int> scores)
    {

        // Remove all previously saved scores
        for (int i = 0; i < maxScores; i++)
        {
            PlayerPrefs.DeleteKey(GetScoreKey(key, i));
        }
        // Save new scores
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt(GetScoreKey(key, i), scores[i]);
            //Debug.LogWarning($"Loaded {key}: {scores[i]}");
        }
        PlayerPrefs.Save(); // Ensure changes are saved
    }

    private static void LoadScores(string key_, List<int> scores)
    {
        scores.Clear();

        for (int i = 0; i < maxScores; i++)
        {
            string key = GetScoreKey(key_, i);
            if (PlayerPrefs.HasKey(key))
            {
                int score = PlayerPrefs.GetInt(key);
                scores.Add(score);
            }
        }
    }

    private static string GetScoreKey(string key_, int index)
    {
        return $"{key_}{index}";
    }


    public static string GetPyramidScoreKey(int index)
    {
        return $"{PyramidScoreKeyFormat}{index}";
    }
    public static string GetVegasScoreKey(int index)
    {
        return $"{VegasScoreKeyFormat}{index}";
    }
    public static string GetArtScoreKey(int index)
    {
        return $"{ArtScoreKeyFormat}{index}";
    }


    public static PatternType GetMode()
    {
        PatternType type = (PatternType)PlayerPrefs.GetInt("Mode");
        return type;
    }

    public static void SetMode(int ModeNum)
    {

        PlayerPrefs.SetInt("Mode", ModeNum);

    }
}

