

using UnityEngine;


using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Image ImageSymbolContainer;

    void Start()
    {

        if (ImageSymbolContainer.sprite == null)
        {

            Debug.LogError("nulll");
        }

    }



}
