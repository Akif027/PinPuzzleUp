using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{

    public SlotType slotType;
    // Distance for the raycast
    public float raycastDistance = 5f;

    public GameObject DownSlot;
    // Offset the raycast origin slightly to avoid hitting self
    public float raycastOriginOffset = 0.1f;


    void GetDownEmptySlot()
    {
        // Create a ray that points downwards
        Vector2 rayDirection = Vector2.down;

        // Offset the raycast origin slightly downward to avoid self-collision
        Vector2 rayOrigin = (Vector2)transform.position + rayDirection * raycastOriginOffset;

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance);

        // Debug line to visualize the raycast in the Scene view
        Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red);

        // Check if the raycast hit a collider and it's not hitting itself
        if (hit.collider != null)
        {
            // If the raycast hit something, print details
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            // Check if the hit object is not the same as this GameObject
            if (hit.collider.gameObject != gameObject)
            {
                // Get the GameObject that was hit
                DownSlot = hit.collider.gameObject;

                // Do something with the hit object, e.g., print its name
                Debug.Log("Hit object: " + DownSlot.name);
            }
            else
            {
                Debug.Log("Raycast hit itself, ignoring.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }


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