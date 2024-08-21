using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolPicker : MonoBehaviour
{
    Button button;

    [SerializeField] Slot child;

    private int clickCount = 0; // Click counter variable


    public int GetClickCount
    {
        set
        {
            clickCount = value;
        }
        get
        {
            return clickCount;
        }
    }
    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    public void HandleClick()
    {
        clickCount++;
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayOnButtonPress();
        }

        child = GetComponentInChildren<Slot>();
        if (child != null)
        {
            if (clickCount == 2)
            {
                GameManager.Instance?.RemoveChildSlot(child.gameObject);

                clickCount = 0; // Reset click count after the second click
            }
            else
            {
                EventManager.ButtonClickedSymbol(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("No child Slot found!");
        }
    }

    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
