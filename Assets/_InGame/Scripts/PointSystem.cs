using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    private int totalPoints = 0;
    public TMP_Text scoreText;  // Reference to the UI Text component for the score
    // Points for different combinations
    private Dictionary<int, int> pointsTable = new Dictionary<int, int>
    {
        { 3, 10 },
        { 4, 20 },
        { 5, 40 },
        { 6, 80 },
        { 7, 200 },
        { 8, 450 },
        { 9, 700 },
        { 10, 1000 }
    };

    private Dictionary<int, int> redPointsTable = new Dictionary<int, int>
    {
        { 7, 500 },
        { 8, 750 },
        { 9, 1000 },
        { 10, 1500 }
    };

    // This method is called when combinations are destroyed
    public void CalculatePoints(List<GameObject> matchedSlots)
    {
        // Dictionary to store the count of each symbol type
        Dictionary<SlotType, int> symbolCount = new Dictionary<SlotType, int>();

        // Count occurrences of each symbol in the matched slots
        foreach (var slot in matchedSlots)
        {
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
            }
        }

        // Calculate points based on the counts
        foreach (var entry in symbolCount)
        {
            int count = entry.Value;
            if (entry.Key == SlotType.Red)
            {
                if (redPointsTable.ContainsKey(count))
                {
                    Debug.Log("Red Symbol Count: " + count + ", Points: " + redPointsTable[count]);
                    totalPoints += redPointsTable[count];
                }
                else
                {
                    Debug.LogWarning("No points found for Red Symbol Count: " + count);
                }
            }
            else
            {
                if (pointsTable.ContainsKey(count))
                {
                    Debug.Log("Symbol Count: " + count + ", Points: " + pointsTable[count]);
                    totalPoints += pointsTable[count];
                }
                else
                {
                    Debug.LogWarning("No points found for Symbol Count: " + count);
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
            scoreText.text = "Score: " + totalPoints.ToString();
        }
        else
        {
            Debug.LogWarning("Score Text UI is not assigned.");
        }
    }

    // Method to reset points (if needed)
    public void ResetPoints()
    {
        totalPoints = 0;
    }

    // Getter to retrieve the current points
    public int GetTotalPoints()
    {
        return totalPoints;
    }
}
