using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Net.Http.Headers;

#region Datas

[System.Serializable]
public class SerializableItem
{
    public string typeName;
    public string jsonData;

    public SerializableItem(string typeName, string jsonData)
    {
        this.typeName = typeName;
        this.jsonData = jsonData;
    }
}


[System.Serializable]
public class SaveData
{
    public PlayerSaveData playerData;
    public List<SerializableItem> items;
    public QuestSaveData questSaveData;

    public Item DeserializeItem(string json, string typeName)
    {
        switch (typeName)
        {
            case "Potion":
                return JsonUtility.FromJson<Potion>(json);
            case "Book":
                return JsonUtility.FromJson<Book>(json);
            case "Weapon":
                return JsonUtility.FromJson<Weapon>(json);
            default:
                return JsonUtility.FromJson<Item>(json);
        }
    }
}

#region PlayerData

[System.Serializable]
public class PlayerSaveData
{
    public string lastLocation;
    public int maxHP;
    public int hp;
    public int level;
    public int xp;
    public int xpToLvl;
    public int marks;
}


#endregion


#region QuestData

[System.Serializable]
public class SerializableQuest
{
    public string typeName;
    public string jsonData;

    public SerializableQuest(string typeName, string jsonData)
    {
        this.typeName = typeName;
        this.jsonData = jsonData;
    }
}

[System.Serializable]
public class QuestSaveData
{
    public bool isInit = false;
    public List<string> completedQuests = new List<string>();
    public List<SerializableQuest> activeQuests = new List<SerializableQuest>();

    public Quest DeserializeQuest(string json, string typeName)
    {
        switch (typeName)
        {
            case "HuntQuest":
                return JsonUtility.FromJson<HuntQuest>(json);
            default:
                return JsonUtility.FromJson<Quest>(json);
        }
    }
}



#endregion

#endregion

public class SaveManager : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        StartCoroutine(InitSave());
    }

    private IEnumerator InitSave()
    {
        while (Player.instance == null)
        {
            yield return null;
        }

        player = Player.instance;
        Load();

        TimeTickManager.instance.OnTick += AutoSave;
    }


    #region AutoSave

    private int saveTickInterval = 100;
    private int currentTick = 0;

    private void AutoSave()
    {
        currentTick++;

        if (currentTick > saveTickInterval)
        {
            Save();
            currentTick = 0;
        }
    }

    #endregion


    #region Save

    public void Save()
    {
        SaveData saveData = new SaveData();
        saveData.playerData = SavePlayer();
        saveData.items = SaveInventory();
        saveData.questSaveData = SaveQuests();

        string json = JsonUtility.ToJson(saveData, true);

        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    #endregion


    #region Load

    public void Load()
    {

        //PlayerPrefs.DeleteKey("SaveData");
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string json = PlayerPrefs.GetString("SaveData");

            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            LoadPlayer(saveData.playerData);
            LoadInventory(saveData);
            LoadQuests(saveData.questSaveData);

        }
        else
        {
            LoadPlayer(new PlayerSaveData());
            LoadQuests(new QuestSaveData());
            GameManager.instance.AtGameLaunch();
        }
        FindObjectOfType<LocationManager>().TravelToLocation(FindObjectOfType<LocationManager>().townData);
    }

    #region GetLocation

    private void GetLocation(SaveData saveData)
    {
        List<LocationData> locationDatas = new List<LocationData>(Resources.LoadAll<LocationData>("Locations"));

        bool isFound = false;
        foreach (LocationData locationData in locationDatas)
        {
            if (locationData.locationName == saveData.playerData.lastLocation)
            {
                FindObjectOfType<LocationManager>().TravelToLocation(locationData);
                isFound = true;
                break;
            }
        }

        if (!isFound) FindObjectOfType<LocationManager>().TravelToLocation(FindObjectOfType<LocationManager>().townData);
    }

    #endregion

    #endregion
    

    #region Save&LoadPlayer

    public PlayerSaveData SavePlayer()
    {
        PlayerSaveData playerData = new PlayerSaveData();

        playerData.lastLocation = FindObjectOfType<LocationManager>().currentLocation.locationName;
        playerData.maxHP = Player.instance.combatController.maxHP;
        playerData.hp = Player.instance.combatController.HP;
        playerData.level = Player.instance.level;
        playerData.xp = Player.instance.xp;
        playerData.xpToLvl = Player.instance.xpToLvl;
        playerData.marks = Player.instance.inventory.marks;

        return playerData;
    }

    public void LoadPlayer(PlayerSaveData playerData)
    {
        player.LoadPlayerData(playerData);
    }

    #endregion


    #region Save&LoadInventory

    public List<SerializableItem> SaveInventory()
    {
        List<SerializableItem> items = new List<SerializableItem>();
        foreach (ItemObject itemObject in Player.instance.inventory.items)
        {
            Item item = itemObject.item;
            string json = JsonUtility.ToJson(item);
            items.Add(new SerializableItem(item.GetType().Name, json));
        }
        return items;
    }

    public void LoadInventory(SaveData saveData)
    {
        foreach (SerializableItem item in saveData.items)
        {
            Item itemToLoad = saveData.DeserializeItem(item.jsonData, item.typeName);

            ItemData itemData = GameManager.instance.itemDatabase.GetItemByName(itemToLoad.Name);

            if (itemData != null)
            {
                itemToLoad.itemSprite = itemData.itemSprite;
                itemToLoad.itemIcon = itemData.itemIcon;
            }

            Player.instance.inventory.AddItem(itemToLoad, itemToLoad.quantity);
        }
    }

    #endregion


    #region Save&LoadQuest

    public QuestSaveData SaveQuests()
    {
        QuestSaveData questSaveData = new QuestSaveData();
        QuestManager questManager = QuestManager.instance;
        foreach (Quest quest in questManager.quests)
        {
            if (quest.isCompleted)
                questSaveData.completedQuests.Add(quest.questName);
        }

        foreach (Quest activeQuest in questManager.activeQuests)
        {
            string json = JsonUtility.ToJson(activeQuest);
            questSaveData.activeQuests.Add(new SerializableQuest(activeQuest.GetType().Name, json));
        }

        questSaveData.isInit = true;


        return questSaveData;
    }

    public void LoadQuests(QuestSaveData questSaveData)
    {
        QuestManager.instance.LoadQuests(questSaveData);
    }

    #endregion


    #region ResetPlayerData

    private void ResetPlayerData()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteKey("SaveData");

            List<ItemObject> temp = new List<ItemObject>(player.inventory.items);
            foreach (ItemObject item in temp)
            {
                player.inventory.RemoveItem(item, item.item.quantity);
            }
            player.LoadPlayerData(new PlayerSaveData());

            QuestManager.instance.activeQuests.Clear();
            QuestManager.instance.LoadQuests(new QuestSaveData());
        }
    }

    #endregion


    public void Update()
    {
        if (DebugManager.instance.isDebuging)
        {
            ResetPlayerData();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

}