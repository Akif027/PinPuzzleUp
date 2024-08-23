
using UnityEngine;
using System;
using System.Collections.Generic;
public static class EventManager
{
    // Define a delegate and event for button clicks
    public static event Action OnArrowButtonClick;
    public static event Action<List<GameObject>> OnPopulateSymbol;
    public static void ButtonClickedSymbol()
    {
        OnArrowButtonClick?.Invoke();
    }

    public static void PopulateSlots(List<GameObject> slots)
    {
        OnPopulateSymbol?.Invoke(slots);
    }

}
