using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameData gameData;
    public GameObject ImageSymbolContainer;
    public List<Sprite> symbolContainer = new List<Sprite>();
    private HashSet<GameObject> processedObjects = new HashSet<GameObject>();
    public List<GameObject> PoolSlots = new List<GameObject>();

    private static readonly System.Random random = new System.Random();
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        // Singleton pattern to ensure one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Optionally make this GameObject persistent
        // DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        EventManager.OnButtonClickSymbol += AddChildSprite;
        EventManager.OnPopulateSlots += UpdateAllSlots;
    }

    private void Start() => ChangeChildImages(ImageSymbolContainer);

    private void OnDisable()
    {
        EventManager.OnButtonClickSymbol -= AddChildSprite;
        EventManager.OnPopulateSlots -= UpdateAllSlots;
        symbolContainer.Clear();
        processedObjects.Clear();
    }

    public void ChangeChildImages(GameObject parentContainer)
    {
        if (!parentContainer || !gameData.sprites.Any())
            return;

        var random = new System.Random();
        foreach (var img in parentContainer.GetComponentsInChildren<Image>(true))
        {
            // Corrected condition: Now it correctly skips direct children of the parentContainer
            if (img.transform.parent == parentContainer.transform)
                continue;

            img.sprite = gameData.sprites[random.Next(gameData.sprites.Count)];
        }
    }

    public void AddChildSprite(GameObject childObject)
    {
        Debug.Log($"Searching in: {childObject.name}");

        if (processedObjects.Contains(childObject))
            return;

        foreach (Transform child in childObject.transform)
        {
            var img = child.GetComponent<Image>();
            if (img?.sprite != null)
            {
                symbolContainer.Add(img.sprite);
                Debug.Log($"Added sprite from child: {img.sprite.name}");
            }
            else
            {
                Debug.Log($"No Image component found in child: {child.gameObject.name}");
            }
        }

        processedObjects.Add(childObject);
    }

    private void UpdateAllSlots(List<GameObject> slots)
    {
        PoolSlots = slots;
        Debug.Log($"{PoolSlots.Count} pool {symbolContainer.Count}");

        if (symbolContainer.Count == 0 || symbolContainer.Count != PoolSlots.Count)
            return;

        var symbolList = symbolContainer.ToList();

        for (int i = 0; i < symbolList.Count; i++)
        {
            foreach (Transform child in PoolSlots[i].transform)
            {
                child.gameObject.SetActive(true);
                var img = child.GetComponent<Image>();

                if (img != null)
                {
                    img.sprite = symbolList[i];
                    break;
                }
            }
        }


    }

    public void ClearList()
    {
        PoolSlots.Clear();
        symbolContainer.Clear();
        processedObjects.Clear();

    }
}