using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowHandler : MonoBehaviour
{
   public float rayLength = 10f;
   public LayerMask slotLayerMask;
   private Button arrowButton;
   public List<GameObject> FilledSlots = new List<GameObject>();

   public List<GameObject> SlotContainer = new List<GameObject>();

   public List<GameObject> getAllSlotsList
   {

      get
      {
         return SlotContainer;
      }

   }

   void Start()
   {
      arrowButton = GetComponent<Button>();
      if (arrowButton != null)
      {

         arrowButton.onClick.AddListener(() => HandleArrowButtonClick(true));

         StartCoroutine(DelayedAddSlot());
      }
   }
   private IEnumerator DelayedAddSlot()
   {
      yield return new WaitForSeconds(1); // Wait for 2 seconds

      AddSlot(); // Call AddSlot after the wait
   }

   public bool isButtonIntractable()
   {

      return arrowButton.interactable;

   }
   public void disableOrEnableTheButton(bool isTrue)
   {

      arrowButton.interactable = isTrue;

   }
   private void HandleArrowButtonClick(bool playSound)
   {

      if (playSound && SoundManager.Instance != null)
      {
         SoundManager.Instance.PlayOnButtonPress();
      }

      FilledSlots.Clear();
      FilledSlots = GetNotEmptySlotsInDirection();

      if (FilledSlots.Count == 0)
      {
         Debug.Log(" empty slots found in the specified direction.");
         GameManager.Instance.ClearList();
      }
      else
      {
         EventManager.PopulateSlots(FilledSlots);
         EventManager.ButtonClickedSymbol();

      }


   }

   private List<GameObject> GetNotEmptySlotsInDirection()
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
         if (IsSlotNotEmpty(slot))
         {
            FilledSlots.Add(slot.transform.GetChild(0).gameObject);

         }
      }

      return FilledSlots;
   }
   private void AddSlot()
   {
      FilledSlots.Clear();
      GetNotEmptySlotsInDirection();

   }

   private bool IsSlotNotEmpty(GameObject slot)
   {
      // Check if the slot has any children
      if (slot.transform.childCount > 0)
      {
         var firstChild = slot.transform.GetChild(0);
         return firstChild.childCount > 0;
      }

      return false; // No children means the slot is empty, so return false
   }


   void OnDisable()
   {
      SlotContainer.Clear();
      FilledSlots.Clear();
   }
}
