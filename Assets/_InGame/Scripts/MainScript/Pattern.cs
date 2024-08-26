
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    // Enums
    public PatternType patternType;

    // Public References
    public GameObject slotPrefab;
    public Camera mainCamera;
    public GameObject pyramidAContainer;
    public GameObject VegasAContainer;
    public GameObject ArtAContainer;
    public GameObject BottomFieldContainer;
    public PointSystem pointSystem;

    // Public Variables
    public float spacing = 100f;

    // Private Variables
    [SerializeField] private List<GameObject> Slots = new List<GameObject>();
    private GameObject[,] slotGrid = new GameObject[2, 5];
    private readonly HashSet<GameObject> slotsWithEffect = new HashSet<GameObject>();
    private List<GameObject> redMatchedSlots = new List<GameObject>();

    // Pattern Definitions
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

    private void Awake()
    {
        patternType = IPlayerPrefs.GetMode();
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

        GenerateBottomToGrid(BottomFieldContainer);
    }

    private void Update()
    {
        UpdateSlotInteractivity();
    }

    private void OnDisable()
    {
        Slots.Clear();
    }

    // Initialization Methods
    private void ActivateContainerAndGeneratePattern(GameObject container, int[,] pattern)
    {
        GeneratePattern(pattern);
        container.SetActive(true);
    }

    private void GeneratePattern(int[,] pattern)
    {
        int rows = pattern.GetLength(0);
        int columns = pattern.GetLength(1);


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
                }
            }
        }
    }

    private void GenerateBottomToGrid(GameObject grid)
    {
        int childCount = grid.transform.childCount;
        int index = 0;

        for (int i = 0; i < 2; i++) // 2 rows
        {
            for (int j = 0; j < 5; j++) // 5 columns
            {
                if (index < childCount)
                {
                    GameObject slot = grid.transform.GetChild(index).GetChild(0).gameObject;
                    slotGrid[i, j] = slot;
                    Slots.Add(slot);
                    index++;
                }
            }
        }
    }

    // Match Checking Methods
    public void CheckForMatches()
    {
        List<GameObject> matchedSlots = new List<GameObject>();

        // Group slots by type and check for repetitions
        CheckForRepetitionsByType(matchedSlots);

        // Ensure uniqueness
        HashSet<GameObject> uniqueMatchedSlots = new HashSet<GameObject>(matchedSlots);

        // Separate red symbols from other matched symbols
        redMatchedSlots = uniqueMatchedSlots.Where(slot => slot.GetComponentInChildren<Slot>().slotType == SlotType.Red).ToList();
        matchedSlots = uniqueMatchedSlots.Where(slot => slot.GetComponentInChildren<Slot>().slotType != SlotType.Red).ToList();

        List<GameObject> allMatchedSlots = redMatchedSlots.Concat(matchedSlots).ToList();

        if (allMatchedSlots.Count >= 3 || (matchedSlots.Count >= 10))
        {
            pointSystem?.CalculatePoints(allMatchedSlots);
            StartCoroutine(DestroySlotsAndShift(allMatchedSlots));
        }
        else if (CannotFillMoreSymbols())
        {
            UpdatePlayerPrefsScore();
            UIhandler.Instance.EndGame();
        }
    }

    // New method to check for repetitions by type
    private void CheckForRepetitionsByType(List<GameObject> matchedSlots)
    {
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        Dictionary<SlotType, List<GameObject>> slotTypeGroups = new Dictionary<SlotType, List<GameObject>>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (slotGrid[i, j] == null) continue;

                Slot slotComp = slotGrid[i, j].GetComponentInChildren<Slot>();
                if (slotComp == null) continue;

                SlotType slotType = slotComp.slotType;

                if (!slotTypeGroups.ContainsKey(slotType))
                {
                    slotTypeGroups[slotType] = new List<GameObject>();
                }

                slotTypeGroups[slotType].Add(slotComp.gameObject);
            }
        }

        foreach (var group in slotTypeGroups)
        {
            SlotType slotType = group.Key;
            List<GameObject> slots = group.Value;

            if (slotType == SlotType.Red)
            {
                if (slots.Count >= 7)
                {
                    matchedSlots.AddRange(slots);
                }
            }
            else
            {
                if (slots.Count >= 3)
                {
                    matchedSlots.AddRange(slots);
                }
            }
        }
    }


    private IEnumerator DestroySlotsAndShift(List<GameObject> matchedSlots)
    {
        yield return new WaitForSeconds(0.4f);

        foreach (var slot in matchedSlots)
        {
            if (slot != null && !slotsWithEffect.Contains(slot))
            {
                slotsWithEffect.Add(slot);
                GameManager.Instance.GetPopEffect(slot.transform);
                yield return new WaitForSeconds(0.1f);
                doTweenAnimations.ScaleOut(slot, 0.1f, shouldDestroy: true);
                SoundManager.Instance?.PlayOnCombo();
            }
        }

        yield return new WaitForSeconds(0.5f);

        ShiftSymbolsToLeft();
        redMatchedSlots.Clear();
        slotsWithEffect.Clear();

        pointSystem.AllowPointsCalculation();
    }

    // Symbol Shifting Methods
    private void ShiftSymbolsToLeft()
    {
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            int emptyIndex = -1;
            for (int j = 0; j < columns; j++)
            {
                if (slotGrid[i, j] == null) continue;

                Transform slotTransform = slotGrid[i, j].transform;

                if (slotTransform.childCount > 0)
                {
                    if (emptyIndex != -1 && slotGrid[i, emptyIndex] != null)
                    {
                        GameObject symbol = slotTransform.GetChild(0).gameObject;
                        symbol.transform.SetParent(slotGrid[i, emptyIndex].transform);
                        symbol.transform.SetPositionAndRotation(
                            slotGrid[i, emptyIndex].transform.position,
                            slotGrid[i, emptyIndex].transform.rotation
                        );
                        emptyIndex++;
                    }
                }
                else if (emptyIndex == -1)
                {
                    emptyIndex = j;
                }
            }
        }

        CheckForMatches();
    }

    // Utility Methods
    public List<GameObject> GetEmptySlots()
    {
        return Slots.Where(slot => slot.transform.childCount == 0).ToList();
    }

    public GameObject GetArrowContainer()
    {
        return patternType switch
        {
            PatternType.Pyramid => pyramidAContainer,
            PatternType.Vegas => VegasAContainer,
            PatternType.Art => ArtAContainer,
            _ => throw new InvalidOperationException("Unsupported pattern type.")
        };
    }

    private void UpdateSlotInteractivity()
    {
        foreach (Transform child in GetArrowContainer().GetComponentsInChildren<Transform>(true))
        {
            ArrowHandler arrowHandler = child.GetComponentInChildren<ArrowHandler>();

            if (arrowHandler != null)
            {
                bool canPlace = arrowHandler.getAllSlotsList.Count <= GetEmptySlots().Count;

                // Assuming disableOrEnableTheButton is the method that takes a boolean to enable/disable the button
                arrowHandler.disableOrEnableTheButton(canPlace);


            }
        }
        if (CannotFillMoreSymbols())
        {
            CheckForMatches();
        }
    }


    public bool CannotFillMoreSymbols()
    {
        return GetArrowContainer()
            .GetComponentsInChildren<ArrowHandler>(true)
            .All(arrowHandler => !arrowHandler.isButtonIntractable());
    }

    private void UpdatePlayerPrefsScore()
    {
        int totalPoints = pointSystem.GetTotalPoints();

        switch (patternType)
        {
            case PatternType.Pyramid:
                IPlayerPrefs.AddPyramidScore(totalPoints);
                break;
            case PatternType.Vegas:
                IPlayerPrefs.AddVegasScore(totalPoints);
                break;
            case PatternType.Art:
                IPlayerPrefs.AddArtScore(totalPoints);
                break;
        }
    }

    // Editor Methods
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
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    public bool IsPoolFilledMoreThan10OrRedRepetition()
    {
        int filledSlotsCount = 0;
        // int redSymbolCount = 0;

        foreach (var slotObject in Slots)
        {
            foreach (Transform child in slotObject.transform)
            {
                Slot slotComponent = child.GetComponentInChildren<Slot>();
                if (slotComponent != null)
                {
                    filledSlotsCount++;

                    // Check if the slot contains a red symbol
                    // if (slotComponent.slotType == SlotType.Red)
                    // {
                    //     redSymbolCount++;
                    //     // Debug.LogError(redSymbolCount);
                    // }

                    // If either condition is met, return true
                    if (filledSlotsCount >= 10 /*|| redSymbolCount >= 7 */)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    // Debug Methods
    public void DebugGridState()
    {
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (slotGrid[i, j] != null)
                {
                    Slot slot = slotGrid[i, j].GetComponentInChildren<Slot>();
                    if (slot != null)
                    {
                        Debug.Log($"Slot at ({i},{j}) contains symbol of type: {slot.slotType}");
                    }
                    else
                    {
                        Debug.Log($"Slot at ({i},{j}) is empty.");
                    }
                }
            }
        }
    }

}
