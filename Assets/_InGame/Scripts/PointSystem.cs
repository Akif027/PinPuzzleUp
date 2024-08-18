using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    private int totalPoints = 0;
    public TMP_Text scoreText;  // Reference to the UI Text component for the score
    public TMP_Text ScoretextGameFinished; // Reference to the UI Text component for the final score

    private readonly Dictionary<int, int> pointsTable = new Dictionary<int, int>
    {
        { 3, 10 }, { 4, 20 }, { 5, 40 }, { 6, 80 },
        { 7, 200 }, { 8, 450 }, { 9, 700 }, { 10, 1000 }
    };

    private readonly Dictionary<int, int> redPointsTable = new Dictionary<int, int>
    {
        { 7, 500 }, { 8, 750 }, { 9, 1000 }, { 10, 1500 }
    };

    public void CalculatePoints(List<GameObject> matchedSlots)
    {
        // Use a HashSet to track which slots have already been counted
        HashSet<GameObject> countedSlots = new HashSet<GameObject>();

        // Dictionary to store the count of each symbol type
        Dictionary<SlotType, int> symbolCount = new Dictionary<SlotType, int>();

        // Count occurrences of each symbol in the matched slots
        foreach (var slot in matchedSlots)
        {
            // Check if this slot has already been counted
            if (countedSlots.Contains(slot))
            {
                continue;
            }

            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent != null)
            {
                SlotType slotType = slotComponent.slotType;

                if (symbolCount.ContainsKey(slotType))
                {
                    symbolCount[slotType]++;
                }
                else
                {
                    symbolCount[slotType] = 1;
                }

                // Mark this slot as counted
                countedSlots.Add(slot);
            }
        }

        // Calculate points based on the counts
        foreach (var entry in symbolCount)
        {
            int count = entry.Value;
            if (entry.Key == SlotType.Red)
            {
                if (redPointsTable.TryGetValue(count, out int points))
                {
                    totalPoints += points;
                    Debug.Log($"Red Symbol Count: {count}, Points: {points}");
                }
            }
            else
            {
                if (pointsTable.TryGetValue(count, out int points))
                {
                    totalPoints += points;
                    Debug.Log($"Symbol Count: {count}, Points: {points}");
                }
            }
        }

        Debug.Log("Total Points: " + totalPoints);
        UpdateScoreText();
    }


    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = totalPoints.ToString();
        }
        if (ScoretextGameFinished != null)
        {
            ScoretextGameFinished.text = totalPoints.ToString();
        }
    }

    public void ResetPoints()
    {
        totalPoints = 0;
    }

    public int GetTotalPoints()
    {
        return totalPoints;
    }

    void OnDisable()
    {

    }
}
