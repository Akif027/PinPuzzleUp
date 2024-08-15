using System.Diagnostics;
using UnityEngine;

public class PatternPyramidGenerator : MonoBehaviour
{
    public GameObject slotPrefab;  // Assign your SlotPrefab here
    public PatternType patternType;
    public float spacing = 100f;   // Adjust this value according to your slot size and desired spacing
    public Camera mainCamera;
    private int[,] patternPyramid = new int[,]
    {
        { 1, 1, 0, 0, 0 },
        { 1, 1, 1, 0, 0 },
        { 1, 1, 1, 1, 0 },
        { 1, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 1 }
    };

    private int[,] patternVegas = new int[,]
    {
        { 0, 1, 1, 0, 0 },
        { 0, 1, 1, 1, 0 },
        { 1, 1, 1, 1, 1 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 1, 1, 0 }
    };

    private int[,] patternArt = new int[,]
    {
        { 1, 1, 1, 1, 1, 0, 0 },
        { 0, 0, 0, 1, 1, 1, 1 },
        { 0, 0, 0, 1, 1, 0, 0 },
        { 1, 1, 1, 1, 0, 0, 0 },
        { 0, 0, 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 1,0 }
    };

    private void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
        switch (patternType)
        {
            case PatternType.Pyramid:
                GeneratePattern(patternPyramid);
                break;
            case PatternType.Vegas:
                GeneratePattern(patternVegas);
                break;
            case PatternType.Art:
                GeneratePattern(patternArt);
                break;
        }
    }

    void GeneratePattern(int[,] pattern)
    {
        float patternWidth = pattern.GetLength(1) * spacing;
        float patternHeight = pattern.GetLength(0) * spacing;

        // Calculate the offset to center the pattern
        float xOffset = -patternWidth / 2 + spacing / 2;
        float yOffset = patternHeight / 2 - spacing / 2;

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                if (pattern[i, j] == 1)  // Only instantiate if the pattern value is 1
                {
                    // Instantiate slot prefab and set as a child of the parent object
                    GameObject slot = Instantiate(slotPrefab, transform);

                    // Calculate and set the local position of the slot relative to the parent
                    slot.transform.localPosition = new Vector3(j * spacing + xOffset, -i * spacing + yOffset, 0);

                    // Optionally set the name or tag for easier identification
                    slot.name = $"Slot_{i}_{j}";
                }
            }
        }
    }

    public enum PatternType
    {
        Pyramid,
        Vegas,
        Art
    }
}
