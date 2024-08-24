using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameData gameData;

    [Header("PatternContainer/Canvas")]
    public GameObject PatternContainer;
    [SerializeField] Pattern pattern;
    [SerializeField] GameObject Canvas;

    public IReadOnlyCollection<GameObject> ProcessedObjects => processedObjects;
    public List<GameObject> PoolSlots = new List<GameObject>();
    public List<GameObject> processedObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.OnArrowButtonClick += UpdateBottomPoolSlots;
        EventManager.OnPopulateSymbol += AddChildSlot;
    }

    private void Start()
    {
        UpdateCenterieldSlots(PatternContainer);
    }

    private void OnDisable()
    {
        EventManager.OnArrowButtonClick -= UpdateBottomPoolSlots;
        EventManager.OnPopulateSymbol -= AddChildSlot;
        processedObjects.Clear();
        IPlayerPrefs.ResetScoreFlag();
    }

    public void UpdateCenterieldSlots(GameObject parentContainer)
    {
        if (!parentContainer || !gameData.SlotsList.Any()) return;

        // Calculate the total probability
        float totalProbability = gameData.SlotsList.Sum(slotPrefab => slotPrefab.GetComponent<Slot>().probability);

        // Iterate through all children recursively
        foreach (Transform child in parentContainer.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "SymbolPos" && !DoesSlotExistInHierarchy(child))
            {
                // Choose a random value between 0 and the total probability
                float randomValue = UnityEngine.Random.Range(0f, totalProbability);
                float cumulativeProbability = 0f;

                GameObject selectedPrefab = null;

                // Select a prefab based on the weighted probability
                foreach (var slotPrefab in gameData.SlotsList)
                {
                    Slot slotComponent = slotPrefab.GetComponent<Slot>();
                    cumulativeProbability += slotComponent.probability;

                    if (randomValue <= cumulativeProbability)
                    {
                        selectedPrefab = slotPrefab.gameObject;
                        break;
                    }
                }

                // Instantiate the selected prefab
                if (selectedPrefab != null)
                {
                    var slotInstance = Instantiate(selectedPrefab, child);
                    slotInstance.transform.localPosition = Vector3.zero;
                    slotInstance.transform.localRotation = Quaternion.identity;
                    slotInstance.transform.localScale = Vector3.one;

                    // Apply fade and optionally destroy the slot
                    doTweenAnimations.Fade(slotInstance.gameObject, 0.5f, fadeIn: true, shouldDestroy: false);
                }
            }
        }
    }

    private bool DoesSlotExistInHierarchy(Transform parent)
    {
        Slot existingSlot = parent.GetComponentInChildren<Slot>();
        if (existingSlot != null)
        {
            //   Debug.Log($"Slot found in {existingSlot.gameObject.name} under {parent.parent.name}");
            return true;
        }
        return false;
    }

    public void ResetProcessedSlots(bool canDestroy)
    {
        foreach (var processedObject in processedObjects)
        {
            if (processedObject != null)
            {
                //   Debug.Log($"Destroying: {processedObject.name}");
                if (canDestroy) Destroy(processedObject);
            }
        }

        processedObjects.Clear();
        PoolSlots.Clear();
        StartCoroutine(DelayedSlotCheck());
    }

    private IEnumerator DelayedSlotCheck()
    {
        yield return new WaitForEndOfFrame(); // Wait for one frame to ensure destruction is complete
        UpdateCenterieldSlots(PatternContainer);
    }

    public void AddChildSlot(List<GameObject> processedObjects_)
    {
        foreach (var obj in processedObjects_)
        {
            // Debug.Log($"Processing: {obj.name}");

            if (processedObjects.Contains(obj)) return;

            if (obj != null)
            {
                processedObjects.Add(obj.transform.GetChild(0).gameObject);
            }
            else
            {
                Debug.Log($"No Slot component found in child: {obj.name}");
            }
        }
    }

    public void RemoveChildSlot(GameObject childObject)
    {
        Debug.Log($"Searching in: {childObject.name}");

        if (!processedObjects.Contains(childObject)) return;

        if (childObject != null)
        {
            processedObjects.Remove(childObject);
        }
        else
        {
            Debug.Log($"No Slot component found in child: {childObject.gameObject.name}");
        }
    }

    private void UpdateBottomPoolSlots() // Gets called after clicking on the arrow button
    {
        PoolSlots = pattern.GetEmptySlots();

        if (processedObjects.Count == 0 || processedObjects.Count > PoolSlots.Count)
        {
            ClearList();
            return;
        }

        for (int i = 0; i < processedObjects.Count; i++)
        {
            if (PoolSlots[i].GetComponentInChildren<Slot>() == null)
            {
                var slotInstance = Instantiate(processedObjects[i], PoolSlots[i].transform);
                slotInstance.transform.localPosition = Vector3.zero;
                slotInstance.transform.localRotation = Quaternion.identity;
                slotInstance.transform.localScale = Vector3.one;

                Vector3 punchVector = new Vector3(0, 0.5f, 0);
                doTweenAnimations.PunchScale(PoolSlots[i].gameObject, punchVector, 0.5f, 10, 1);
                doTweenAnimations.Fade(slotInstance, 0.5f, fadeIn: true, shouldDestroy: false);

                if (SoundManager.Instance != null) SoundManager.Instance.PlayOnFillPool();
            }
        }

        ResetProcessedSlots(true);
        UpdateCenterieldSlots(PatternContainer);
    }

    public void ClearList()
    {
        PoolSlots.Clear();
        ResetProcessedSlots(false);
    }

    public PatternType GetpatternType => pattern.patternType;

    void Update()
    {
        if (pattern.IsPoolFilledMoreThan10OrRedRepetition())
        {
            pattern.CheckForMatches();
        }

        if (Input.GetMouseButtonDown(1))
        {
            pattern.CheckForMatches();
        }
    }

    public GameObject GetPopEffect(Transform pos)
    {
        GameObject effect = Instantiate(gameData.popEffect, pos.position, Quaternion.identity, Canvas.transform);
        Destroy(effect, 2);
        return effect;
    }
}
