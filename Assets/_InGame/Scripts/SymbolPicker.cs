using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SymbolPicker : MonoBehaviour
{

    Button button;
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
        Image childImage = GetComponentInChildren<Image>();
        if (childImage != null)
        {
            EventManager.ButtonClickedSymbol(childImage.gameObject);
        }
    }
}
