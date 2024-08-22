using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameData gameData;
    [Header("ChooseFiedl/Canvas")] public GameObject ImageSymbolContainer;
    public IReadOnlyCollection<GameObject> ProcessedObjects => processedObjects;
    public List<GameObject> PoolSlots = new List<GameObject>();
    public List<GameObject> processedObjects = new List<GameObject>();
    [SerializeField] Pattern pattern;
    [SerializeField] GameObject Canvas;

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
        EventManager.OnButtonClickSymbol += AddChildSlot;
        EventManager.OnPopulateSlots += UpdateSelectedPoolSlots;
    }

    private void Start() => UpdateBottomFieldSlots(ImageSymbolContainer);

    private void OnDisable()
    {
        EventManager.OnButtonClickSymbol -= AddChildSlot;
        EventManager.OnPopulateSlots -= UpdateSelectedPoolSlots;
        processedObjects.Clear();
        IPlayerPrefs.ResetScoreFlag();
    }

    public void UpdateBottomFieldSlots(GameObject parentContainer)
    {
        if (!parentContainer || !gameData.SlotsList.Any()) return;

        var random = new System.Random();

        // Iterate through all children recursively
        foreach (Transform child in parentContainer.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "SymbolPos")
            {
                if (!DoesSlotExistInHierarchy(child))
                {
                    var randomSlotPrefab = gameData.SlotsList[random.Next(gameData.SlotsList.Count)];
                    var slotInstance = Instantiate(randomSlotPrefab, child);

                    slotInstance.transform.localPosition = Vector3.zero;
                    slotInstance.transform.localRotation = Quaternion.identity;
                    slotInstance.transform.localScale = Vector3.one;

                    // Apply fade and optionally destroy the slot
                    doTweenAnimations.Fade(slotInstance.gameObject, 0.5f, fadeIn: true, shouldDestroy: false); // Set shouldDestroy to true or false based on your requirement
                }
            }
        }
    }


    private bool DoesSlotExistInHierarchy(Transform parent)
    {
        // Directly search for the Slot component in all children
        Slot existingSlot = parent.GetComponentInChildren<Slot>();
        if (existingSlot != null)
        {
            Debug.Log($"Slot found in {existingSlot.gameObject.name} under {parent.name}");
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
                ChangeColor(processedObject, Color.white);
                processedObject.transform.parent.parent.GetComponent<SymbolPicker>().GetClickCount = 0;
                Debug.Log($"Destroying: {processedObject.name}");
                if (canDestroy) Destroy(processedObject);
            }
        }

        processedObjects.Clear();

        // Add a small delay before rechecking
        StartCoroutine(DelayedSlotCheck());
    }

    private IEnumerator DelayedSlotCheck()
    {
        yield return new WaitForEndOfFrame(); // Wait for one frame to ensure destruction is complete
        UpdateBottomFieldSlots(ImageSymbolContainer);
    }
    private void ChangeColor(GameObject obj, Color c)
    {
        obj.transform.parent.parent.GetComponent<Image>().color = c;
    }
    public void AddChildSlot(GameObject childObject)
    {
        Debug.Log($"Searching in: {childObject.name}");

        if (processedObjects.Contains(childObject))
            return;


        if (childObject != null)
        {

            processedObjects.Add(childObject);
            ChangeColor(childObject, gameData.BottomFieldOnSelectButtonColor);

        }
        else
        {
            Debug.Log($"No Slot component found in child: {childObject.gameObject.name}");
        }

    }

    public void RemoveChildSlot(GameObject childObject)
    {
        Debug.Log($"Searching in: {childObject.name}");

        if (!processedObjects.Contains(childObject))
            return;


        if (childObject != null)
        {

            processedObjects.Remove(childObject);
            ChangeColor(childObject, Color.white);
        }
        else
        {
            Debug.Log($"No Slot component found in child: {childObject.gameObject.name}");
        }

    }

    private void UpdateSelectedPoolSlots(List<GameObject> slots) //its getting called after we click on the arrow button
    {

        PoolSlots = slots;

        if (processedObjects.Count == 0 || processedObjects.Count != PoolSlots.Count)
            return;

        for (int i = 0; i < PoolSlots.Count; i++)
        {
            foreach (Transform child in PoolSlots[i].transform)
            {
                if (child.GetComponent<Slot>() == null)
                {
                    ChangeColor(processedObjects[i], Color.white);
                    // Instantiate a Slot if none exists
                    var slotInstance = Instantiate(processedObjects[i], child);
                    slotInstance.transform.localPosition = Vector3.zero;
                    slotInstance.transform.localRotation = Quaternion.identity;
                    slotInstance.transform.localScale = Vector3.one;
                    Vector3 punchVector = new Vector3(0, 0.5f, 0);
                    doTweenAnimations.PunchScale(PoolSlots[i].gameObject, punchVector, 0.5f, 10, 1);
                    doTweenAnimations.Fade(slotInstance, 0.5f, fadeIn: true, shouldDestroy: false);
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayOnFillPool();

                }
            }
        }

        ResetProcessedSlots(true);
        //  UpdateBottomFieldSlots(ImageSymbolContainer);
    }

    public void ClearList()
    {
        PoolSlots.Clear();
        ResetProcessedSlots(false);
        // processedObjects.Clear();
    }

    public PatternType GetpatternType
    {

        get
        {
            return pattern.patternType;
        }

    }
    void Update()
    {
        if (pattern.IsPoolFilledMoreThan10())
        {

            pattern.CheckForMatches();
        }

    }

    public GameObject GetPopEffect(Transform pos)
    {
        GameObject effect = Instantiate(gameData.popEffect, pos.position, Quaternion.identity, Canvas.transform);
        Destroy(effect, 1);
        return effect;
    }

}

