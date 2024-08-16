using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowHandler : MonoBehaviour
{
   public float rayLength = 10f;
   public Color selectedColor = Color.green;
   public LayerMask slotLayerMask;

   private Button arrowButton;

   void Start()
   {
      arrowButton = GetComponent<Button>();
      if (arrowButton != null)
      {
         arrowButton.onClick.AddListener(HandleArrowButtonClick);
      }
   }

   private void HandleArrowButtonClick()
   {
      var emptySlots = GetEmptySlotsInDirection();
      EventManager.PopulateSlots(emptySlots);

      if (emptySlots.Count == 0)
      {
         Debug.Log("No empty slots found in the specified direction.");
      }
      GameManager.Instance.ClearList();

   }

   private List<GameObject> GetEmptySlotsInDirection()
   {
      var emptySlots = new List<GameObject>();
      var rayDirection = -transform.up;

      RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, rayLength, slotLayerMask);
      Debug.DrawRay(transform.position, rayDirection * rayLength, Color.red, 2f);

      foreach (var hit in hits)
      {
         var slot = hit.collider.gameObject;
         var parentImage = slot.GetComponent<Image>();
         var childImages = slot.GetComponentsInChildren<Image>(true);

         bool isEmpty = true;

         foreach (var img in childImages)
         {
            if (img != parentImage && img.sprite != null)
            {
               isEmpty = false;
               break;
            }
         }

         if (isEmpty)
         {
            emptySlots.Add(slot);
            Debug.Log("Empty slot detected: " + slot.name);
         }
         else
         {
            Debug.Log("Slot is not empty: " + slot.name);
         }
      }

      return emptySlots;
   }
}
