using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Newtonsoft;
using System.IO.Compression;
using System.IO;
using System;
using UnityEngine.Networking;

public class Endleficator : MonoBehaviour
{
    public TextAsset remixJSON;
    public TextAsset remix2JSON;
    public TextAsset presetJSON;
    public TextAsset MusicBinary;
    public TextAsset madnessJSON;
    public TextAsset gamesData;
    public RHREData remix;
    public NewGameList EEEList;
    public int beat;
    public int loops;
    public int offset;
    public bool[] gamesCheck = new bool[115];
    public Toggle beggining;
    public GameObject gamePrefab;
    public GameObject gameContainer;
    public GameObject presetPrefab;
    public GameObject presetPrefab2;
    public GameObject presetContainer;
    public GameObject[] settingPrefabs = new GameObject[4];
    public GameObject settingContainer;
    public GameObject endSetting;
    public string url;
    public GameObject updatePopUp;
    public GameObject savePopUp;
    public int customPresetCounter;
    // Start is called before the first frame update
    void Start()
    {
        customPresetCounter = 1;
        for(int i=0; i<gamesCheck.Length; i++)
        {
            gamesCheck[i] = true;
        }
        if(!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Endless Remix")))
        {
            InitnData();
        }
        //LoadData();
        //CreateRemix();
        StartGames();
        //CreateTestRemix();
        StartCoroutine(CheckForUpdateTest());
        /*remix.version = "v3.20.6";
        remix.databaseVersion = 138;
        remix.playbackStart = 0;
        remix.musicStartSec = 0;
        remix.trackCount = 8;
        remix.isAutosave = false;
        remix.midiInstruments = 0;
        remix.musicData = new MusicData(true, "Endless Remix.ogg", "ogg");
        //remix.textures
        //remix.entities
        remix.trackers
        remix.timeSignatures*/
        /*List<Entity> entis = new List<Entity>{};
        List<List<Entity>> entitis = new List<List<Entity>>();
        List<List<List<Entity>>> entititis = new List<List<List<Entity>>>();
        Entity e = new Entity("model", 0, 1, 1, 1, 0, 100, "", "", false);
        //Debug.Log(e.type);
        entis.Add(e);
        entis.Add(e);
        entitis.Add(entis);
        entitis.Add(entis);
        entititis.Add(entitis);
        entititis.Add(entitis);
        //Debug.Log(entititis[0][0][0].volume);
        EEEList = new NewGameList(entititis);
        string data = Newtonsoft.Json.JsonConvert.SerializeObject(EEEList, Newtonsoft.Json.Formatting.Indented); //Newtonsoft.Json.Formatting
        //Debug.Log(data);
        System.IO.File.WriteAllText(Application.dataPath + "/test3.json", data);*/
    }

    public void InitnData()
    {
        Debug.Log("Initing data");
        string pathToDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        Directory.CreateDirectory(Path.Combine(pathToDocuments, "Endless Remix"));
        Directory.CreateDirectory(Path.Combine(pathToDocuments, "Endless Remix/data"));
        File.WriteAllText(Path.Combine(pathToDocuments, "Endless Remix/data/remix.json"), remixJSON.text);
        //MusicBinary = Resources.Load<TextAsset>("myBinaryAsset");
        File.WriteAllBytes(Path.Combine(pathToDocuments, "Endless Remix/data/music.bin"), MusicBinary.bytes);
    }

    private void LoadData()
    {
        //Debug.Log(60/119.0f*8);
        if(beggining.isOn)
            remix = JsonUtility.FromJson<RHREData>(remix2JSON.text);
        else
            remix = JsonUtility.FromJson<RHREData>(remixJSON.text);
        EEEList = Newtonsoft.Json.JsonConvert.DeserializeObject<NewGameList>(madnessJSON.text);
        //Debug.Log(EEEList.games[0][0][0].datamodel);
    }

    private void CreateTestRemix()
    {
        remix = JsonUtility.FromJson<RHREData>(remixJSON.text);
        EEEList = Newtonsoft.Json.JsonConvert.DeserializeObject<NewGameList>(madnessJSON.text);
        beat = 0;
        offset = 8;
        //Entity musicEntity = new Entity("model", offset, 1, 224, 1, 0, 60, "endlessRemixMusic_endlessRemixLong", "", true);
        //Add every version of every game to the remix
        for(int i=0; i<EEEList.games.Count; i++)
        {
            for(int j=0; j<EEEList.games[i].Count; j++)
            {
                foreach(Entity entity in EEEList.games[i][j])
                {
                    if(entity.datamodel!="special/gameChange")
                    {
                        entity.beat += beat+offset;
                        remix.entities.Add(entity);
                    }
                }
                beat += 16;
                if(beat%224==0&&beat!=0)
                {
                    remix.entities.Add(new Entity("model", offset+beat, 1, 224, 1, 0, 70, "endlessRemixMusic_endlessRemixLong", "", true));
                    Debug.Log("added at " + (beat+offset).ToString());
                }
            }
        }
        string newRemix = JsonUtility.ToJson(remix, true);
        string pathToDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //Debug.Log(pathToDocuments);
        File.Delete (Path.Combine(pathToDocuments, "Endless Remix/Endless Remix - test.rhre3"));
        File.WriteAllText(Path.Combine(pathToDocuments, "Endless Remix/data/remix.json"), newRemix);
        ZipFile.CreateFromDirectory(Path.Combine(pathToDocuments, "Endless Remix/data"), Path.Combine(pathToDocuments, "Endless Remix/Endless Remix - test.rhre3"));
        
    }

    public void CreateRemix()
    {
        LoadData();
        beat = 0;
        if(beggining.isOn)
            beat = 96;
        offset = 8;
        SettingScript[] settings = settingContainer.transform.parent.gameObject.GetComponentsInChildren<SettingScript>();
        PresetScript[] presets = presetContainer.transform.parent.gameObject.GetComponentsInChildren<PresetScript>();
        Dictionary<int, bool[]> gameChanges  = new Dictionary<int, bool[]>();
        bool[] tempGames = new bool[115];
        int endBeat = 0;
        for(int i=0; i<settings.Length; i++)
        {
            switch(settings[i].type)
            {
                case 1:
                    remix.trackers.tempos.trackers.Add(new TempoTracker(settings[i].lap*224+offset, 0, settings[i].bpm, 0f, 50, 1f));
                    break;
                case 2:
                    remix.trackers.tempos.trackers.Add(new TempoTracker(settings[i].lap+offset, 0, settings[i].bpm, 0f, 50, 1f));
                    break;
                case 3:
                    tempGames = new bool[115];
                    for(int j=0; j<settings[i].games.Count; j++)
                    {
                        //Debug.Log(presets[settings[i].games[j]-1].presetName);
                        for(int k=0; k<presets[settings[i].games[j]-1].gamesCheck.Length; k++)
                        {
                            if(presets[settings[i].games[j]-1].gamesCheck[k])
                                tempGames[k] = true;
                        }
                    }
                    gameChanges.Add(settings[i].lap*224, tempGames);
                    //Debug.Log(settings[i].beat*224);
                    break;
                case 4:
                    tempGames = new bool[115];
                    for(int j=0; j<settings[i].games.Count; j++)
                    {
                        for(int k=0; k<presets[j].gamesCheck.Length; k++)
                        {
                            if(presets[j].gamesCheck[k])
                                tempGames[k] = true;
                        }
                    }
                    gameChanges.Add(settings[i].lap, tempGames);
                    break;
                case 5:
                    endBeat = (settings[i].lap)*224;
                    break;
            }
        }
        //Debug.Log(gameChanges.get);
        tempGames = gameChanges[0];
        /*string testOutput = "";
        for(int i=0; i<tempGames.Count(); i++)
        {
            if(tempGames[i])
                testOutput += i.ToString() + ", ";
        }
        Debug.Log(testOutput);*/
        int lastGame = -1;
        /*Debug.Log(EEEList.games[1][0][1].datamodel);
        Debug.Log(EEEList.games[115][0][1].datamodel);
        int helper = 0;
        int error = 1/helper;*/
        while(beat<endBeat)
        {
            if(gameChanges.ContainsKey(beat))
            {
                tempGames = gameChanges[beat];
                //Debug.Log("Games changed");
            }
            if(beat%224==0&&beat!=0)
            {
                remix.entities.Add(new Entity("model", offset+beat, 1, 224, 1, 0, 70, "endlessRemixMusic_endlessRemixLong", "", true));
                //Debug.Log("added at " + (beat+offset).ToString());
            }
            int game = UnityEngine.Random.Range(1, 116);
            while(tempGames[game-1] == false || game==lastGame)
                game = UnityEngine.Random.Range(1, 116);
            int version = UnityEngine.Random.Range(0, EEEList.games[game].Count);
            //Debug.Log(game.ToString() + "/" + version.ToString());
            //Debug.Log(game.ToString());
            foreach(Entity entity in EEEList.games[game][version])
            {
                //Entity(string type, float beat, int track, float width, int heigh, int semitone=0, int volume=100, string datamodel="", string subtitle="", bool isCustom=false)
                if(entity.datamodel!="special/gameChange")
                {
                    //Акшсл
                    if(entity.datamodel.Contains("120"))
                    {
                        int bpm = (int)remix.trackers.tempos.trackers[^1].bpm;
                        Debug.Log("Beat is " + (beat+offset).ToString());
                        for(int i=0; i<remix.trackers.tempos.trackers.Count; i++)
                        {
                            if(remix.trackers.tempos.trackers[i].beat>beat+offset)
                            {
                                bpm = (int)remix.trackers.tempos.trackers[i-1].bpm;
                                //Debug.Log(remix.trackers.tempos.trackers[i].beat.ToString() + ">" + (beat+offset).ToString());
                                //Debug.Log("Next tracker is " + remix.trackers.tempos.trackers[i].bpm.ToString());
                                Debug.Log("Current tracker is " + remix.trackers.tempos.trackers[i-1].bpm.ToString());
                                break;
                            }
                        }
                        bpm = (int)(Mathf.Floor(bpm/10f) * 10);
                        if(bpm<70)
                        {
                            bpm = 70;
                        }
                        if(bpm>170)
                        {
                            bpm = 170;
                        }
                        Debug.Log("bpm is " + bpm.ToString());
                        remix.entities.Add(new Entity(entity.type, entity.beat+beat+offset, entity.track, entity.width, entity.height, entity.semitone, entity.volume, entity.datamodel.Substring(0, (entity.datamodel.Length)-3)+bpm.ToString(), entity.subtitle, entity.isCustom));
                        //print("");
                    }
                    else
                    remix.entities.Add(new Entity(entity.type, entity.beat+beat+offset, entity.track, entity.width, entity.height, entity.semitone, entity.volume, entity.datamodel, entity.subtitle, entity.isCustom));
                }
            }
            lastGame = game;
            beat += 16;
        }
        remix.entities.Add(new Entity("model", endBeat+offset, 0, 0.125f, 8, 0, 0, "special_endEntity", "", false));
        string newRemix = JsonUtility.ToJson(remix, true);
        string pathToDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //Debug.Log(pathToDocuments);
        File.Delete (Path.Combine(pathToDocuments, "Endless Remix/Endless Remix.rhre3"));
        File.WriteAllText(Path.Combine(pathToDocuments, "Endless Remix/data/remix.json"), newRemix);
        ZipFile.CreateFromDirectory(Path.Combine(pathToDocuments, "Endless Remix/data"), Path.Combine(pathToDocuments, "Endless Remix/Endless Remix.rhre3"));
        StartCoroutine(SaveNotification());
        
    }

    private void StartGames()
    {
        string[] lines = ReadByLines(gamesData);
        int counter = 0;
        ListOfChecks lst = JsonUtility.FromJson<ListOfChecks>(presetJSON.text);
        
        foreach (string line in lines)
        {
            GameToggleScript game = Instantiate(gamePrefab, gameContainer.transform).GetComponent<GameToggleScript>();
            Sprite loadedSprite = Resources.Load<Sprite>(line.Replace(" ", "").Replace("!", "")[..^1]);
            //print(line.Replace(" ", ""));
            game.SetName(line);
            game.SetImage(loadedSprite);
            game.SetID(counter);
            counter++;
        }

        for(int i=0; i<lst.data.Count; i++)
        {
            PresetScript preset = Instantiate(presetPrefab2, presetContainer.transform).GetComponent<PresetScript>();
            preset.SetID(i+1);
            preset.SetName(lst.data[i].name);
            preset.SetChecks(lst.data[i].data);
            //Debug.Log(lst.data[i].name);
        }
        /*PresetScript[] presets = presetContainer.GetComponentsInChildren<PresetScript>();
        foreach(PresetScript preset in presets)
        {
            Debug.Log(preset.presetName);
        }*/
    }

    public void Test()
    {
        bool[] tempGames = new bool[115];
        tempGames[114] = true;
        tempGames[113] = true;
        tempGames[112] = true;
        tempGames[111] = true;
        int lastGame = -1;
        for(int i=0; i<1000; i++)
        {
            int game = UnityEngine.Random.Range(1, 116);
            while(tempGames[game-1] == false || game==lastGame)
                game = UnityEngine.Random.Range(1, 116);
            Debug.Log(game.ToString());
            lastGame = game;
        }
    } 

    public void Test2ElectricBoogaloo()
    {
        //Шэь ф акшслштп швшще
        List<List<float>> settings = new List<List<float>>
        {
            new List<float> { 0, 117 },
            new List<float> { 2, 134 },
            new List<float> { 4, 159 },
        };
        for(int i=0; i<=6; i++)
        {
            //To lower the number of outputs
            if(i%2==0)
                continue;
            int amount = (int)settings[^1][1];
            Debug.Log("Step is " + i.ToString());
            for(int j=0; j<settings.Count; j++)
            {
                //I need to get value of the last applied setting. They apply when setting[0] = i 
                if(settings[j][0]>i)
                {
                    amount = (int)settings[j-1][1];
                    //Debug.Log(setting[j][0].ToString() + ">" + i.ToString());
                    //Debug.Log("Next setting is " + setting[j][1]);
                    Debug.Log("Current setting is " + settings[j-1][1].ToString());
                    break;
                }
            }
            //get only tens of the amount. E. g. 135->130
            amount = (int)(Mathf.Floor(amount/10f) * 10);
            if(amount<70)
            {
                amount = 70;
            }
            if(amount>170)
            {
                amount = 170;
            }
            Debug.Log("amount is " + amount.ToString());
        }
                    /*if(entity.datamodel.Contains("120"))
                    {
                        int bpm = (int)remix.trackers.tempos.trackers[^1].bpm;
                        print("Beat is " + (beat+offset).ToString());
                        for(int i=0; i<remix.trackers.tempos.trackers.Count; i++)
                        {
                            if(remix.trackers.tempos.trackers[i].beat>beat+offset)
                            {
                                bpm = (int)remix.trackers.tempos.trackers[i-1].bpm;
                                //Debug.Log(remix.trackers.tempos.trackers[i].beat.ToString() + ">" + (beat+offset).ToString());
                                //Debug.Log("Next tracker is " + remix.trackers.tempos.trackers[i].bpm);
                                print("Current tracker is " + remix.trackers.tempos.trackers[i-1].bpm);
                            }
                        }
                        bpm = (int)(Mathf.Floor(bpm/10f) * 10);
                        if(bpm<70)
                        {
                            bpm = 70;
                        }
                        if(bpm>170)
                        {
                            bpm = 170;
                        }
                        print("bpm is " + bpm);
                        remix.entities.Add(new Entity(entity.type, entity.beat+beat+offset, entity.track, entity.width, entity.height, entity.semitone, entity.volume, entity.datamodel.Substring(0, (entity.datamodel.Length)-3)+bpm.ToString(), entity.subtitle, entity.isCustom));
                        print("");
                    }*/
    }

    public void SaveJSON()
    {
        ListOfChecks lst = new ListOfChecks();
        lst.data.Add(new Checks(gamesCheck, "test"));
        string data = Newtonsoft.Json.JsonConvert.SerializeObject(lst, Newtonsoft.Json.Formatting.Indented); //Newtonsoft.Json.Formatting
        //Debug.Log(data);
        File.WriteAllText(Application.dataPath + "/test2323.json", data);
    }

    public void TurnAllOff()
    {
        Toggle[] scripts = gameContainer.GetComponentsInChildren<Toggle>();
        for(int i=0; i<gamesCheck.Length; i++)
        {
            //gamesCheck[i] = false;
            scripts[i].isOn = false;
        }
    }

    public void TurnAllOn()
    {
        Toggle[] scripts = gameContainer.GetComponentsInChildren<Toggle>();
        for(int i=0; i<gamesCheck.Length; i++)
        {
            //gamesCheck[i] = true;
            scripts[i].isOn = true;
        }
    }

    //Because reasons
    public void TurnSomeOn(bool[] newChecks)
    {
        Toggle[] scripts = gameContainer.GetComponentsInChildren<Toggle>();
        for(int i=0; i<newChecks.Length; i++)
        {
            //gamesCheck[i] = newChecks[i];
            scripts[i].isOn = newChecks[i];
        }
    }

    public void CreateSetting(int type)
    {
        SettingScript setting = Instantiate(settingPrefabs[type], settingContainer.transform).GetComponent<SettingScript>();
        endSetting.transform.SetSiblingIndex(settingContainer.transform.childCount - 1);
        //setting.SetType(type);
    }

    public void AddPreset()
    {
        PresetScript preset = Instantiate(presetPrefab, presetContainer.transform).GetComponent<PresetScript>();
        preset.SetID(presetContainer.transform.childCount);
        preset.SetName("Custom Preset");
        bool[] newChecks = new bool[gamesCheck.Length];
        for(int i=0; i<gamesCheck.Length; i++)
        {
            newChecks[i] = gamesCheck[i];
        }
        preset.SetChecks(newChecks);
        customPresetCounter++;
    }

    private IEnumerator CheckForUpdateTest()
    {
        //"https://raw.githubusercontent.com/VirusGames/EndlessRemixMaker/main/Assets/version.txt"
        //"https://raw.githubusercontent.com/VirusGames/EndlessRemixMaker/main/Test.txt"
        string url = "https://raw.githubusercontent.com/VirusGames/EndlessRemixMaker/main/Assets/version.txt";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 60;

        yield return request.SendWebRequest ( );

         if ( request.isDone )
         {
            if (request.result != UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.downloadHandler.text);
                if (Application.version != request.downloadHandler.text[..^1])
                {
                    updatePopUp.SetActive(true);
                }
            }
         }
    }

    public void DownloadUpdate()
    {
        //https://github.com/VirusGames/EndlessRemixMaker/releases/tag/publish
        Application.OpenURL("https://github.com/VirusGames/EndlessRemixMaker/releases/tag/publish");
    }

    public void HideUpdate()
    {
        updatePopUp.SetActive(false);
    }

    public IEnumerator SaveNotification()
    {
        savePopUp.SetActive(true);
        yield return new WaitForSeconds(5);
        savePopUp.SetActive(false);
    }

    private string[] ReadByLines(TextAsset textAsset)
    {
        string[] lines = textAsset.text.Split('\n');
        return lines;
    }

    //RHRE json data. If you don't understand this, it's not my problem.
    [System.Serializable]
    public class RHREData
    {
        public string version;
        public int databaseVersion;
        public float playbackStart;
        public float musicStartSec;
        public int trackCount;
        public bool isAutosave;
        public int midiInstruments;
        public MusicData musicData;
        public List<TextureData> texture;
        public List<Entity> entities;
        public TrackerList trackers;
        public List<TimeSignature> timeSignatures;
    }

    [System.Serializable]
    public class MusicData
    {
        public bool present;
        public string filename;
        public string extension;
        public MusicData(bool present, string filename, string extension)
        {
            this.present = present;
            this.filename = filename;
            this.extension = extension;
        }
    }

    [System.Serializable]
    public class TextureData
    {
        public int textureID;
    }

    [System.Serializable]
    public class Entity
    {
        public string type;
        public float beat;
        public int track;
        public float width;
        public int height;
        public int semitone;
        public int volume;
        public string datamodel;
		public string subtitle;
        public bool isCustom;
        public Entity(string type, float beat, int track, float width, int heigh, int semitone=0, int volume=100, string datamodel="", string subtitle="", bool isCustom=false)
        {
            this.type = type;
            this.beat = beat;
            this.track = track;
            this.width = width;
            this.height = heigh;
            this.semitone = semitone;
            this.volume = volume;
            this.datamodel = datamodel;
            this.subtitle = subtitle;
            this.isCustom = isCustom;
        }
    }

    [System.Serializable]
    public class TempoTracker
    {
        public float beat;
        public float seconds;
        public float bpm;
        public float width;
        public int swingRatio;
        public float swingDivision;
        public TempoTracker(float beat, float seconds, float bpm, float width, int swingRatio, float swingDivision)
        {
            this.beat = beat;
            this.seconds = seconds;
            this.bpm = bpm;
            this.width = width;
            this.swingRatio = swingRatio;
            this.swingDivision = swingDivision;
        }
    }

    [System.Serializable]
    public class VolumeTracker
    {
        public float beat;
        public float width;
        public int volume;
        //Why do I even have this constructor?
        public VolumeTracker(float beat, float width, int volume)
        {
            this.beat = beat;
            this.width = width;
            this.volume = volume;
        }
    }

    [System.Serializable]
    public class TempoTrackerList
    {
        public List<TempoTracker> trackers;
    }

    [System.Serializable]
    public class VolumeTrackerList
    {
        public List<VolumeTracker> trackers;
    }

    [System.Serializable]
    public class TrackerList
    {
        public TempoTrackerList tempos;
        public VolumeTrackerList musicVolumes;
    }

    [System.Serializable]
    public class TimeSignature
    {
        public float beat;
        public int divisions;
        public int beatUnit;
        public int measure;
    }

    [System.Serializable]
    public class EntityList
    {
        public List<Entity> entities;
        public void Add(Entity item)
        {
            entities.Add(item);
        }
    }

    //Version of the game in endless remix
    [System.Serializable]
    public class VersionList
    {
        public List<EntityList> versions;
        public void Add(EntityList item)
        {
            versions.Add(item);
        }
    }

    //List of versions of the game in endless remix
    [System.Serializable]
    public class GameList
    {
        public List<VersionList> games;
        public void Add(VersionList item)
        {
            games.Add(item);
        }
    }

    //List of lists of versions of the game in endless remix
    [System.Serializable]
    public class NewGameList
    {
        public List<List<List<Entity>>> games;
        public void Add(Entity item)
        {
            
        }
        public NewGameList(List<List<List<Entity>>> games)
        {
            this.games = games;
        }
    }

    //I have no idea what is this but I'm afraid to delete this
    [System.Serializable]
    public class ListOfListsWrapper
    {
        public List<List<int>> data;

        public ListOfListsWrapper(List<List<int>> data)
        {
            this.data = data;
        }
    }

    [System.Serializable]
    public class Checks
    {
        public string name;
        public bool[] data;

        public Checks(bool[] newdata, string newname)
        {
            data = newdata;
            name = newname;
        }
    }

    [System.Serializable]
    public class ListOfChecks
    {
        public List<Checks> data = new List<Checks>();
    }
}
