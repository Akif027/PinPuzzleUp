using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private void Awake()
    {
        patternType = IPlayerPrefs.GetMode();

    }
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
    public void DebugSlotGrid()
    {
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject slotComponent = slotGrid[i, j];
                if (slotComponent != null)
                {
                    // Assuming Slot component has a property like 'slotType' or similar to identify it
                    Slot slotComp = slotComponent.GetComponentInChildren<Slot>();
                    if (slotComp != null)
                    {
                        Debug.LogError($"Slot at ({i},{j}) is filled with type: {slotComp.slotType}");
                    }
                    else
                    {
                        Debug.LogError($"Slot at ({i},{j}) is empty.");
                    }
                }
                else
                {
                    Debug.LogError($"Slot at ({i},{j}) is null.");
                }
            }
        }
    }
    private List<GameObject> redMatchedSlots = new List<GameObject>();
    public void CheckForMatches()
    {
        List<GameObject> matchedSlots = new List<GameObject>();

        // Check both horizontal and vertical patterns for matches
        CheckPatternForMatches(matchedSlots, true);  // Horizontal
        CheckPatternForMatches(matchedSlots, false); // Vertical

        // Use HashSet to ensure uniqueness
        HashSet<GameObject> uniqueMatchedSlots = new HashSet<GameObject>(matchedSlots);

        // Separate red symbols from other matched symbols
        redMatchedSlots = uniqueMatchedSlots.Where(slot => slot.GetComponentInChildren<Slot>().slotType == SlotType.Red).ToList();
        matchedSlots = uniqueMatchedSlots.Where(slot => slot.GetComponentInChildren<Slot>().slotType != SlotType.Red).ToList();

        // Handle red symbols separately: Only destroy if there are 7 or more
        if (redMatchedSlots.Count >= 7)
        {
            Debug.Log(redMatchedSlots.Count);
            // Calculate points for red symbols
            if (pointSystem != null)
            {
                pointSystem.CalculatePoints(redMatchedSlots);
            }

            // Destroy the red symbols
            StartCoroutine(DestroySlotsAndShift(redMatchedSlots));
        }

        // Continue with regular matched symbols (e.g., Blue, Green)
        if (matchedSlots.Count > 0)
        {
            // Calculate points for other symbols
            if (pointSystem != null)
            {
                pointSystem.CalculatePoints(matchedSlots);
            }

            StartCoroutine(DestroySlotsAndShift(matchedSlots));
        }
        else if (AreAllSlotsFilledAndNoMatches()) // Game over only if no matches and pool is filled
        {
            if (patternType == PatternType.Pyramid)
            {

                IPlayerPrefs.AddPyramidScore(pointSystem.GetTotalPoints());
            }
            else if (patternType == PatternType.Vegas)
            {

                IPlayerPrefs.AddVegasScore(pointSystem.GetTotalPoints());
            }
            else
            {

                IPlayerPrefs.AddArtScore(pointSystem.GetTotalPoints());
            }

            UIhandler.Instance.EndGame();
            return;
        }
    }
    public void SavePoints()
    {
        if (patternType == PatternType.Pyramid)
        {

            IPlayerPrefs.AddPyramidScore(pointSystem.GetTotalPoints());
        }
        else if (patternType == PatternType.Vegas)
        {
            Debug.LogError("added");
            IPlayerPrefs.AddVegasScore(pointSystem.GetTotalPoints());
        }
        else
        {

            IPlayerPrefs.AddArtScore(pointSystem.GetTotalPoints());
        }
        UIhandler.Instance.EndGame();


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
                if (slotGrid[i, j] == null) continue; // Skip null slots

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

    private IEnumerator DestroySlotsAndShift(List<GameObject> matchedSlots)
    {
        foreach (var slot in matchedSlots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }

        // Wait for the end of the frame to ensure all slots are destroyed
        yield return new WaitForEndOfFrame();

        // Shift remaining symbols to the left
        ShiftSymbolsToLeft();
        redMatchedSlots.Clear();
    }

    public void ShiftSymbolsToLeft()
    {
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            int emptyIndex = -1;
            for (int j = 0; j < columns; j++)
            {
                if (slotGrid[i, j] != null)
                {
                    Transform slotTransform = slotGrid[i, j].transform;
                    if (slotTransform.childCount > 0) // Check if the slot has any children
                    {
                        if (emptyIndex != -1) // Only proceed if an empty slot has been found
                        {
                            GameObject symbol = slotTransform.GetChild(0).gameObject;
                            symbol.transform.SetParent(slotGrid[i, emptyIndex].transform);
                            symbol.transform.SetPositionAndRotation(
                                slotGrid[i, emptyIndex].transform.position,
                                slotGrid[i, emptyIndex].transform.rotation
                            );

                            // Update the grid reference
                            slotGrid[i, emptyIndex] = slotGrid[i, j];
                            slotGrid[i, j] = null;

                            // Move the emptyIndex to the next empty position
                            emptyIndex++;
                        }
                    }
                    else if (emptyIndex == -1)
                    {
                        emptyIndex = j; // Found the first empty slot
                    }
                }
            }
        }
    }

    public bool AreAllSlotsFilledAndNoMatches()
    {
        // Check if all slots are filled
        bool allSlotsFilled = true;
        int rows = slotGrid.GetLength(0);
        int columns = slotGrid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (slotGrid[i, j] != null)
                {
                    Slot slot = slotGrid[i, j].GetComponentInChildren<Slot>();
                    if (slot == null)
                    {
                        Debug.Log($"Slot at ({i},{j}) has no Slot component.");
                        allSlotsFilled = false;
                    }
                }
            }
        }

        // Check if no matches are found
        bool noMatches = true;
        List<GameObject> matchedSlots = new List<GameObject>();
        CheckPatternForMatches(matchedSlots, true);  // Horizontal
        CheckPatternForMatches(matchedSlots, false); // Vertical

        if (matchedSlots.Count > 0)
        {
            Debug.Log("Matches found!");
            noMatches = false;
        }

        // Log final result
        Debug.Log($"All slots filled: {allSlotsFilled}");
        Debug.Log($"No matches found: {noMatches}");

        // Return true if all slots are filled and no matches are found
        return allSlotsFilled && noMatches;
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


    private void OnDisable()
    {
        Slots.Clear();
    }
}
