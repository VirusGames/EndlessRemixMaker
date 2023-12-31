using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{

    // Start is called before the first frame update
    public string resourceName; // Name of the sprite file without extension (e.g., "mySprite")

    void Start()
    {
        Sprite loadedSprite = Resources.Load<Sprite>(resourceName);
        if (loadedSprite != null)
        {
            Image spriteRenderer = GetComponent<Image>();
            if (spriteRenderer != null)
            {
                //Debug.Log(loadedSprite.name);
                spriteRenderer.sprite = loadedSprite;
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found!");
            }
        }
        else
        {
            Debug.LogError("Sprite not found in Resources folder: " + resourceName);
        }
    }
}
