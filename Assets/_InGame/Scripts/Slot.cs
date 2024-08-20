using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{

    public SlotType slotType;
    // Distance for the raycast
    public float raycastDistance = 5f;

}
public enum SlotType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple
    // Add other types as needed
}