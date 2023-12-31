using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class SettingScript : MonoBehaviour
{
    private Endleficator endleficator;
    public int type;
    public int lap;
    public int bpm;
    public List<int> games = new List<int>();
    public TMP_InputField beatinp;
    public TMP_InputField bpminp;

    private void Awake()
    {
        endleficator = FindObjectOfType<Endleficator>();
    }

    public void SetType(int type)
    {
        this.type = type;
    }

    public void SetBeat()
    {
        try{lap = int.Parse(beatinp.text);}
        catch (Exception e){Debug.Log(e);}
    }

    public void SetBPM()
    {
        try{bpm = int.Parse(bpminp.text);}
        catch (Exception e){Debug.Log(e);}
        try{games = GetIntsFromString(bpminp.text);}
        catch (Exception e){Debug.Log(e);}
    }

    public int GetBeat()
    {
        return lap;
    }

    public int GetBPM()
    {
        return bpm;
    }

    public List<int> GetGames()
    {
        return games;
    }

    public void Annihilate()
    {
        Destroy(this.gameObject);
    }

    List<int> GetIntsFromString(string str)
    {
        List<int> ints = new List<int>();
    
        string[] splitString = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in splitString)
        {
            try
            {
                ints.Add(Convert.ToInt32(item));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Value in string was not an int.");
                Debug.LogException(e);
            }
        }
        return ints;
    }
}
