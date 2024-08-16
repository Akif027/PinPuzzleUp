using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SymbolPicker : MonoBehaviour
{

    Button button;

    Image childImage;
    void Start()
    {
        button = GetComponent<Button>();
        childImage = GetComponentInChildren<Image>();
        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    public void HandleClick()
    {

        if (childImage != null)
        {
            EventManager.ButtonClickedSymbol(childImage.gameObject);
        }
    }
}
