using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SymbolPicker : MonoBehaviour
{

    Button button;

    [SerializeField] Slot child;
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
        child = GetComponentInChildren<Slot>();
        if (child != null)
        {
            EventManager.ButtonClickedSymbol(child.gameObject);
        }
    }
}
