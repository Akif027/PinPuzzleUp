using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowHandler : MonoBehaviour
{
   public float rayLength = 10f;
   public LayerMask slotLayerMask;
   private Button arrowButton;
   private List<GameObject> emptySlots = new List<GameObject>();

   public List<GameObject> SlotContainer = new List<GameObject>();

   bool CanCheckEmptySlot = false;
   void Start()
   {
      arrowButton = GetComponent<Button>();
      if (arrowButton != null)
      {

         arrowButton.onClick.AddListener(() => HandleArrowButtonClick(true));
      }
   }

   void Update()
   {

      UpdateArrowButtonInteractable();
   }



   private void UpdateArrowButtonInteractable()
   {
      if (!CanCheckEmptySlot) return;
      bool hasEmptySlots = CheckForEmptySlots();
      arrowButton.interactable = hasEmptySlots;
   }

   private void HandleArrowButtonClick(bool playSound)
   {
      CanCheckEmptySlot = true;
      if (playSound && SoundManager.Instance != null)
      {
         SoundManager.Instance.PlayOnButtonPress();
      }

      emptySlots.Clear();
      emptySlots = GetEmptySlotsInDirection();

      if (emptySlots.Count == 0)
      {
         Debug.Log("No empty slots found in the specified direction.");
      }
      else
      {
         EventManager.PopulateSlots(emptySlots);
         GameManager.Instance.ClearList();
      }

      // Update the button interactability after processing.
      UpdateArrowButtonInteractable();
   }

   private List<GameObject> GetEmptySlotsInDirection()
   {
      var rayDirection = transform.right;
      RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, rayLength, slotLayerMask);
#if UNITY_EDITOR
      Debug.DrawRay(transform.position, rayDirection * rayLength, Color.red, 2f);
#endif

      SlotContainer.Clear(); // Clear SlotContainer before adding new slots

      foreach (var hit in hits)
      {
         var slot = hit.collider.gameObject;
         SlotContainer.Add(slot); // Add slot to SlotContainer
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

   private bool CheckForEmptySlots()
   {
      // This function checks if there are any empty slots in the SlotContainer
      foreach (var slot in SlotContainer)
      {
         if (IsSlotEmpty(slot))
         {
            return true;
         }
      }
      return false;
   }

   void OnDisable()
   {
      SlotContainer.Clear();
      emptySlots.Clear();
   }
}
