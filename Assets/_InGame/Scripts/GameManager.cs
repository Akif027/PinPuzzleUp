using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameData gameData; // Assign this in the Unity Editor
    public GameObject ImageSymbolContainer;

    void Start()
    {
        ChangeChildImages(ImageSymbolContainer);
    }

    public void ChangeChildImages(GameObject parentContainer)
    {
        // Check if parentContainer is null to avoid errors
        if (parentContainer == null)
        {
            Debug.LogWarning("Parent container is null.");
            return;
        }

        // Get all Image components in the parentContainer and its children
        Image[] images = parentContainer.GetComponentsInChildren<Image>(true);

        // Ensure we have sprites available
        if (gameData.sprites == null || gameData.sprites.Count == 0)
        {
            Debug.LogWarning("No sprites available in gameData.");
            return;
        }

        // Create a random index for selecting images
        System.Random random = new System.Random();

        // Iterate over all found Image components
        foreach (Image img in images)
        {
            // Skip Image components that are direct children of the parentContainer
            if (img.transform.parent == parentContainer.transform)
            {
                continue;
            }

            // Randomly select a sprite
            int randomIndex = random.Next(gameData.sprites.Count);

            // Apply the random sprite to the Image component
            img.sprite = gameData.sprites[randomIndex];
        }
    }
}
