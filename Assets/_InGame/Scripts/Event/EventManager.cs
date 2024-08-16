
using UnityEngine;
using System;
using System.Collections.Generic;
public static class EventManager
{
    // Define a delegate and event for button clicks
    public static event Action<GameObject> OnButtonClickSymbol;
    public static event Action<List<GameObject>> OnPopulateSlots;
    public static void ButtonClickedSymbol(GameObject child)
    {
        OnButtonClickSymbol?.Invoke(child);
    }

    public static void PopulateSlots(List<GameObject> slots)
    {
        OnPopulateSlots?.Invoke(slots);
    }

}
