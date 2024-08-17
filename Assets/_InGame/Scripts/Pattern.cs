using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pattern : MonoBehaviour
{
    public GameObject slotPrefab;  // Assign your SlotPrefab here
    public PatternType patternType;
    public Camera mainCamera;
    public float spacing = 100f;   // Adjust this value according to your slot size and desired spacing

    public GameObject pyramidAContainer;
    public GameObject VegasAContainer;
    public GameObject ArtAContainer;

    private List<GameObject> Slots = new List<GameObject>();
    private Slot[,] slotGrid;  // 2D array to store the Slot references

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
        { 0, 0, 0, 0, 0, 1, 0 }
    };

    private void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;

        switch (patternType)
        {
            case PatternType.Pyramid:
                GeneratePattern(patternPyramid);
                InitializeSlotGrid(patternPyramid.GetLength(0), patternPyramid.GetLength(1));
                pyramidAContainer.SetActive(true);
                break;
            case PatternType.Vegas:
                GeneratePattern(patternVegas);
                InitializeSlotGrid(patternVegas.GetLength(0), patternVegas.GetLength(1));
                VegasAContainer.SetActive(true);
                break;
            case PatternType.Art:
                GeneratePattern(patternArt);
                InitializeSlotGrid(patternArt.GetLength(0), patternArt.GetLength(1));
                ArtAContainer.SetActive(true);
                break;
        }
    }

    // Method to generate pattern without entering play mode
    public void GeneratePatternInEditor()
    {
        int[,] pattern = null;
        switch (patternType)
        {
            case PatternType.Pyramid:
                pattern = patternPyramid;
                break;
            case PatternType.Vegas:
                pattern = patternVegas;
                break;
            case PatternType.Art:
                pattern = patternArt;
                break;
        }
        if (pattern != null)
            GeneratePattern(pattern);
    }

    // Method to destroy the existing pattern in the editor
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

    void GeneratePattern(int[,] pattern)
    {
        float patternWidth = pattern.GetLength(1) * spacing;
        float patternHeight = pattern.GetLength(0) * spacing;

        // Calculate the offset to center the pattern
        float xOffset = -patternWidth / 2 + spacing / 2;
        float yOffset = patternHeight / 2 - spacing / 2;

        // Destroy any existing children
        DestroyPatternInEditor();

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                if (pattern[i, j] == 1)  // Only instantiate if the pattern value is 1
                {
                    // Instantiate slot prefab and set as a child of the parent object
                    GameObject slot = Instantiate(slotPrefab, transform);
                    Slots.Add(slot);
                    // Calculate and set the local position of the slot relative to the parent
                    slot.transform.localPosition = new Vector3(j * spacing + xOffset, -i * spacing + yOffset, 0);

                    // Optionally set the name or tag for easier identification
                    slot.name = $"Slot_{i}_{j}";
                }
            }
        }
    }

    public void CheckForMatchesAndDestroy()
    {
        for (int i = 0; i < slotGrid.GetLength(0); i++)
        {
            for (int j = 0; j < slotGrid.GetLength(1); j++)
            {
                if (slotGrid[i, j] != null)
                {
                    // Check for horizontal match
                    if (i + 2 < slotGrid.GetLength(0) &&
                        slotGrid[i + 1, j] != null &&
                        slotGrid[i + 2, j] != null &&
                        slotGrid[i, j].slotType == slotGrid[i + 1, j].slotType &&
                        slotGrid[i + 1, j].slotType == slotGrid[i + 2, j].slotType)
                    {
                        DestroySlot(slotGrid[i, j].gameObject);
                        DestroySlot(slotGrid[i + 1, j].gameObject);
                        DestroySlot(slotGrid[i + 2, j].gameObject);
                    }

                    // Check for vertical match
                    if (j + 2 < slotGrid.GetLength(1) &&
                        slotGrid[i, j + 1] != null &&
                        slotGrid[i, j + 2] != null &&
                        slotGrid[i, j].slotType == slotGrid[i, j + 1].slotType &&
                        slotGrid[i, j + 1].slotType == slotGrid[i, j + 2].slotType)
                    {
                        DestroySlot(slotGrid[i, j].gameObject);
                        DestroySlot(slotGrid[i, j + 1].gameObject);
                        DestroySlot(slotGrid[i, j + 2].gameObject);
                    }
                }
            }
        }
    }

    private void DestroySlot(GameObject slot)
    {
        Slots.Remove(slot);
        Destroy(slot);
    }

    public List<GameObject> GetAllSlots()
    {
        return Slots;
    }

    public bool isPoolFilledMoreThan10()
    {
        int filledSlotsCount = 0;

        foreach (var gameObject in GetAllSlots())
        {
            foreach (Transform child in gameObject.transform)
            {
                Slot slot = child.GetComponentInChildren<Slot>();
                if (slot != null)
                {
                    filledSlotsCount++;
                    if (filledSlotsCount >= 10)
                    {
                        //CheckForMatchesAndDestroy(); // Trigger match check
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void InitializeSlotGrid(int rows, int cols)
    {
        slotGrid = new Slot[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject slotObject = GetSlotObjectAt(i, j); // Replace with your method to get the slot object at (i, j)
                if (slotObject != null)
                {
                    slotGrid[i, j] = slotObject.GetComponent<Slot>();
                }
            }
        }
    }

    GameObject GetSlotObjectAt(int row, int col)
    {
        foreach (var slot in Slots)
        {
            string expectedName = $"Slot_{row}_{col}";
            if (slot.name == expectedName)
            {
                return slot;
            }
        }
        return null;
    }

    public enum PatternType
    {
        Pyramid,
        Vegas,
        Art
    }

    void OnDisable()
    {
        Slots.Clear();
    }
}
