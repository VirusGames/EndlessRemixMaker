using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PresetScript : MonoBehaviour
{
    private Endleficator endleficator;
    public string presetName;
    public int id;
    public TMP_Text presettext;
    public bool[] gamesCheck = new bool[115];
    private void Awake()
    {
        endleficator = FindObjectOfType<Endleficator>();
        presettext = GetComponentInChildren<TMP_Text>();
    }

    public void SetID(int newID)
    {
        id = newID;
    }

    public void SetName(string newName)
    {
        presetName = newName;
        presettext.text = (id).ToString() + ". " + newName;
    }

    public void Annihilate()
    {
        endleficator.customPresetCounter--;
        this.gameObject.SetActive(false);
        PresetScript[] presets = this.gameObject.transform.parent.gameObject.GetComponentsInChildren<PresetScript>();
        int counter = 1;
        foreach(PresetScript preset in presets)
        {
            preset.id = counter;
            preset.SetName(preset.presetName);
            counter++;
        }
        Destroy(this.gameObject);
    }

    public void SetChecks(bool[] newChecks)
    {
        gamesCheck = newChecks;
    }

    public void SetChecks(int start, int end)
    {
        for(int i=start; i<=end; i++)
        {
            gamesCheck[i] = true;
        }
    }

    public bool[] GetChecks()
    {
        return gamesCheck;
    }

    public void LoadChecks()
    {
        endleficator.TurnSomeOn(gamesCheck);
    }
}
