using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArrowHandler : MonoBehaviour
{
   public float rayLength = 10f;
   public Color selectedColor = Color.green;
   public LayerMask slotLayerMask;

   private Button arrowButton;
   private List<GameObject> emptySlots = new List<GameObject>();

   public List<GameObject> tmpslot = new List<GameObject>();

   void Start()
   {
      arrowButton = GetComponent<Button>();
      if (arrowButton != null)
      {
         HandleArrowButtonClick();
         arrowButton.onClick.AddListener(HandleArrowButtonClick);
      }
   }

   void Update()
   {
      bool hasEmptySlots = false;
      foreach (var item in tmpslot)
      {
         if (IsSlotEmpty(item))
         {
            hasEmptySlots = true;
            break;
         }
      }

      arrowButton.interactable = hasEmptySlots;
   }

   private void HandleArrowButtonClick()
   {
      emptySlots.Clear();
      emptySlots = GetEmptySlotsInDirection();
      EventManager.PopulateSlots(emptySlots);

      if (emptySlots.Count == 0)
      {
         Debug.Log("No empty slots found in the specified direction.");
      }
      GameManager.Instance.ClearList();
   }

   private List<GameObject> GetEmptySlotsInDirection()
   {
      var rayDirection = -transform.up;
      RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, rayLength, slotLayerMask);
      Debug.DrawRay(transform.position, rayDirection * rayLength, Color.red, 2f);

      tmpslot.Clear(); // Clear tmpslot before adding new slots

      foreach (var hit in hits)
      {
         var slot = hit.collider.gameObject;
         tmpslot.Add(slot); // Add slot to tmpslot
         if (IsSlotEmpty(slot))
         {
            emptySlots.Add(slot);

         }

      }

      return emptySlots;
   }

   private bool IsSlotEmpty(GameObject slot)
   {
      // Check if the slot has any children, if not it's empty
      if (slot.transform.childCount > 0)
      {
         var firstChild = slot.transform.GetChild(0);
         return firstChild.childCount == 0;
      }

      return true; // No children means the slot is empty
   }


   void OnDisable()
   {
      tmpslot.Clear();
      emptySlots.Clear();
   }
}
