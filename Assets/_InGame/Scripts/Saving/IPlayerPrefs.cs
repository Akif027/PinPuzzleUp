using System.Collections.Generic;
using UnityEngine;

public static class IPlayerPrefs
{
    private const string HighScoreKeyPrefix = "HighScore_";



    // Set High Score
    public static void SetHighScore(int rank, int score)
    {
        SetInt(HighScoreKeyPrefix + rank, score);
    }

    // Get High Score
    public static int GetHighScore(int rank)
    {
        return GetInt(HighScoreKeyPrefix + rank, 0);
    }

    // Save the new score if it's in the top 5
    public static void SaveScore(int score)
    {
        List<int> scores = new List<int>();

        // Load existing scores
        for (int i = 1; i <= 5; i++)
        {
            scores.Add(GetHighScore(i));
        }

        // Add the new score
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a)); // Sort descending

        // Save the top 5 scores
        for (int i = 1; i <= 5; i++)
        {
            SetHighScore(i, scores[i - 1]);
        }
    }
    // Set integer value
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    // Get integer value with default fallback
    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    // Set float value
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    // Get float value with default fallback
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    // Set string value
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    // Get string value with default fallback
    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    // Check if a key exists
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    // Delete a specific key
    public static void DeleteKey(string key)
    {
        if (HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    // Delete all keys
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    // Save PlayerPrefs (optional as it's called internally after every Set method)
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
