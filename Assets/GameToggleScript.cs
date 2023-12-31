using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameToggleScript : MonoBehaviour
{
    private Toggle toggle;
    private Image image;
    private TMP_Text game;
    public int id;
    private Endleficator endleficator;

    void Awake()
    {
        toggle = GetComponentInChildren<Toggle>();
        image = GetComponentInChildren<Image>();
        game = GetComponentInChildren<TMP_Text>();
        endleficator = FindObjectOfType<Endleficator>();
    }

    public void SetImage(Sprite newSprite)
    {
        image.sprite = newSprite;
    }

    public void SetValue()
    {
        endleficator.gamesCheck[id] = toggle.isOn;
    }

    public void SetName(string newName)
    {
        game.text = newName;
    }

    public void SetID(int newID)
    {
        id = newID;
    }
}
