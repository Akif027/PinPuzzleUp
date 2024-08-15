using System.Diagnostics;
using UnityEngine;

public class PatternPyramidGenerator : MonoBehaviour
{
    public GameObject slotPrefab;  // Assign your SlotPrefab here
    public PatternType patternType;
    public float spacing = 100f;   // Adjust this value according to your slot size and desired spacing
    public Camera mainCamera;


    public GameObject rightArrowPrefab;
    public GameObject leftArrowPrefab;
    public GameObject upArrowPrefab;
    public GameObject downArrowPrefab;
    private int[,] patternPyramid = new int[,]
    {
        { 0, 2, 0, 0, 0 ,0,0},
        { 2, 1, 1, 2, 0,0 ,0},
        { 0, 1, 1,1, 2,0 ,0},
        { 2, 1, 1, 1, 1,2 ,0},
        { 0, 1, 1, 1, 1 ,1,2},
          { 0, 0, 2, 0, 2 ,1,0}
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
        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                int element = pattern[i, j];

                if (element == 1)
                {
                    InstantiateElement(slotPrefab, i, j);
                }
                else if (element == 2)
                {
                    InstantiateElement(rightArrowPrefab, i, j);
                }
                else if (element == 3)
                {
                    InstantiateElement(leftArrowPrefab, i, j);
                }
                else if (element == 4)
                {
                    InstantiateElement(upArrowPrefab, i, j);
                }
                else if (element == 5)
                {
                    InstantiateElement(downArrowPrefab, i, j);
                }
            }
        }
    }

    void InstantiateElement(GameObject prefab, int i, int j)
    {
        GameObject element = Instantiate(prefab, transform);
        element.transform.localPosition = new Vector3(j * spacing, -i * spacing, 0);
        element.name = $"{prefab.name}_{i}_{j}";


    }

    public enum PatternType
    {
        Pyramid,
        Vegas,
        Art
    }
}
