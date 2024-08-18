using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Pattern : MonoBehaviour
{
    public GameObject slotPrefab;
    public PatternType patternType;
    public Camera mainCamera;
    public float spacing = 100f;

    public GameObject pyramidAContainer;
    public GameObject VegasAContainer;
    public GameObject ArtAContainer;
    public PointSystem pointSystem;
    private List<GameObject> Slots = new List<GameObject>();
    private GameObject[,] slotGrid;

    private readonly int[,] patternPyramid = new int[,]
    {
        { 1, 1, 0, 0, 0 },
        { 1, 1, 1, 0, 0 },
        { 1, 1, 1, 1, 0 },
        { 1, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 1 }
    };

    private readonly int[,] patternVegas = new int[,]
    {
        { 0, 1, 1, 0, 0 },
        { 0, 1, 1, 1, 0 },
        { 1, 1, 1, 1, 1 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 1, 1, 0 }
    };

    private readonly int[,] patternArt = new int[,]
    {
        { 1, 1, 1, 1, 1, 0, 0 },
        { 0, 0, 0, 1, 1, 1, 1 },
        { 0, 0, 0, 1, 1, 0, 0 },
        { 1, 1, 1, 1, 0, 0, 0 },
        { 0, 0, 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 1, 0 }
    };

    private void Start()
    {
        mainCamera = mainCamera ?? Camera.main;

        switch (patternType)
        {
            case PatternType.Pyramid:
                ActivateContainerAndGeneratePattern(pyramidAContainer, patternPyramid);
                break;
            case PatternType.Vegas:
                ActivateContainerAndGeneratePattern(VegasAContainer, patternVegas);
                break;
            case PatternType.Art:
                ActivateContainerAndGeneratePattern(ArtAContainer, patternArt);
                break;
        }
    }

    private void ActivateContainerAndGeneratePattern(GameObject container, int[,] pattern)
    {
        GeneratePattern(pattern);
        container.SetActive(true);
    }

    public void GeneratePatternInEditor()
    {
        int[,] pattern = patternType switch
        {
            PatternType.Pyramid => patternPyramid,
            PatternType.Vegas => patternVegas,
            PatternType.Art => patternArt,
            _ => null
        };

        if (pattern != null)
        {
            GeneratePattern(pattern);
        }
    }

    public void DestroyPatternInEditor()
    {
        // We create a copy of the child collection to avoid issues during iteration
        List<Transform> children = new List<Transform>();

        // Gather all child transforms
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        // Destroy each child GameObject
        foreach (Transform child in children)
        {
            // This will destroy the GameObject immediately in the editor
            DestroyImmediate(child.gameObject);
        }
    }

    private void GeneratePattern(int[,] pattern)
    {
        int rows = pattern.GetLength(0);
        int columns = pattern.GetLength(1);

        slotGrid = new GameObject[rows, columns];
        Slots.Clear();
        DestroyPatternInEditor();

        float xOffset = -(columns - 1) * spacing / 2;
        float yOffset = (rows - 1) * spacing / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (pattern[i, j] == 1)
                {
                    GameObject slotObject = Instantiate(slotPrefab, transform);
                    slotObject.transform.localPosition = new Vector3(j * spacing + xOffset, -i * spacing + yOffset, 0);
                    slotObject.name = $"Slot_{i}_{j}";

                    GameObject slotComponent = slotObject.transform.GetChild(0)?.gameObject;
                    if (slotComponent != null)
                    {
                        slotGrid[i, j] = slotComponent;
                    }
                    else
                    {
                        Debug.LogWarning($"SlotComponent is null at ({i},{j})");
                    }

                    Slots.Add(slotObject);
                }
            }
        }
    }

    public void CheckForMatches()
    {
        List<GameObject> matchedSlots = new List<GameObject>();

        CheckPatternForMatches(matchedSlots, true);
        CheckPatternForMatches(matchedSlots, false);

        // Calculate points before destroying the slots

        if (pointSystem != null)
        {
            pointSystem.CalculatePoints(matchedSlots);
        }


        // Now destroy the matched slots
        foreach (var slot in matchedSlots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }

    }

    private void CheckPatternForMatches(List<GameObject> matchedSlots, bool horizontal)
    {
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        int maxI = horizontal ? rows : rows - 2;
        int maxJ = horizontal ? columns - 2 : columns;

        for (int i = 0; i < maxI; i++)
        {
            for (int j = 0; j < maxJ; j++)
            {
                GameObject slot1 = horizontal ? slotGrid[i, j] : slotGrid[i, j];
                GameObject slot2 = horizontal ? slotGrid[i, j + 1] : slotGrid[i + 1, j];
                GameObject slot3 = horizontal ? slotGrid[i, j + 2] : slotGrid[i + 2, j];

                if (slot1 != null && slot2 != null && slot3 != null)
                {
                    Slot slotComp1 = slot1.GetComponentInChildren<Slot>();
                    Slot slotComp2 = slot2.GetComponentInChildren<Slot>();
                    Slot slotComp3 = slot3.GetComponentInChildren<Slot>();

                    if (slotComp1 != null && slotComp2 != null && slotComp3 != null &&
                        slotComp1.slotType == slotComp2.slotType && slotComp2.slotType == slotComp3.slotType)
                    {
                        matchedSlots.Add(slotComp1.gameObject);
                        matchedSlots.Add(slotComp2.gameObject);
                        matchedSlots.Add(slotComp3.gameObject);
                    }
                }
            }
        }
    }


    public bool IsPoolFilledMoreThan10()
    {
        int filledSlotsCount = 0;

        foreach (var slotObject in Slots)
        {
            foreach (Transform child in slotObject.transform)
            {
                if (child.GetComponentInChildren<Slot>() != null)
                {
                    filledSlotsCount++;
                    if (filledSlotsCount >= 10)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public enum PatternType
    {
        Pyramid,
        Vegas,
        Art
    }

    private void OnDisable()
    {
        Slots.Clear();
    }
}
