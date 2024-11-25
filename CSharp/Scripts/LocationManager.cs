using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour
{
    public LocationData currentLocation;
    public LocationData townData;
    public LocationData forestData;
    public LocationData coastData;


    public void TravelToLocation(LocationData location)
    {
        InteractionManager.instance.EndDialogue();


        if (travelCoroutine != null)
            StopCoroutine(travelCoroutine);

        DisablePanels();
        travelCoroutine = StartCoroutine(TravelCoroutine(location));

        ResetInfo();

        mapPanel.SetActive(false);
    }

    public Coroutine travelCoroutine;

    private IEnumerator TravelCoroutine(LocationData destination)
    {
        float distance = 0;
        if (currentLocation != null)
            distance = Vector2.Distance(currentLocation.position, destination.position);
        float travelTime = distance / Player.instance.travelSpeed;

        Player.instance.combatController.UpdateStatus("Traveling", travelTime);
        yield return new WaitForSeconds(travelTime);
        UpdateUIAndLocation(destination);
    }

    public void FastTravel(LocationData locationData)
    {
        DisablePanels();
        ResetInfo();
        UpdateUIAndLocation(locationData);
    }

    private void UpdateUIAndLocation(LocationData destination)
    {
        currentLocation = destination;
        if (destination.locationType == LocationType.Town)
        {
            GoToTown();
        }
        else if (destination.locationType == LocationType.Wilderness)
        {
            GoToWild();
        }
        ChangeBackground();
    }

    #region Wilderness


    [Header("Wilderness")]
    [SerializeField] private GameObject WildernessPanel;

    [Header("Actions Setup")]
    [SerializeField] private List<GameObject> actionPanels;
    [SerializeField] private Transform actionsHolder;
    [SerializeField] private GameObject actionButtonPrefab;

    #region ActionsSetup

    private void ActionsSetup(WildernessData wildernessData)
    {
        foreach (Transform child in actionsHolder)
            if (child.name != "Text") Destroy(child.gameObject);

        foreach (Activities action in wildernessData.activities)
        {
            GameObject newButton = Instantiate(actionButtonPrefab, actionsHolder);
            newButton.name = action.ToString();

            newButton.GetComponentInChildren<TextMeshProUGUI>().text = action.ToString();
            Button button = newButton.GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            switch (action)
            {
                case Activities.Hunting:
                    {
                        button.onClick.AddListener(() =>
                        {
                            actionPanels.Find(panel => panel.name == "HuntingPanel").transform.SetAsLastSibling();
                        });
                        break;
                    }
                case Activities.Woodcutting:
                    {
                        GetComponentInChildren<WoodcuttingManager>().InitWoodcutting(wildernessData.trees);
                        button.onClick.AddListener(() =>
                        {
                            actionPanels.Find(panel => panel.name == "WoodCuttingPanel").transform.SetAsLastSibling();
                        });
                        break;
                    }
            }

        }


        actionPanels.Find(panel => panel.name == "HuntingPanel").transform.SetAsLastSibling();
    }

    public void ToggleButtons(ButtonHandler buttonHandler)
    {
        foreach (Transform child in actionsHolder)
        {
            if (child.name == "Text") continue;
            if (child.TryGetComponent<ButtonHandler>(out var component))
            {
                if (component == buttonHandler) continue;

                component.ToggleButton(false);

            }
        }
    }

    #endregion


    #region GoToWild

    private void GoToWild()
    {
        MonsterManager.instance.InitWild(currentLocation as WildernessData);

        ActionsSetup(currentLocation as WildernessData);

        WildernessPanel.SetActive(true);
        Player.instance.SetRegeneration(false);
    }

    #endregion


    #region DropItem

    [Header("Drop")]
    public RectTransform itemsHolder;
    public List<ItemObject> loot = new List<ItemObject>();


    public void DropItem(ItemData itemDroped)
    {

        Item newItem = Player.instance.inventory.CastItem(null, itemDroped);

        foreach (ItemObject item in loot)
        {
            if (item.item.Name == newItem.Name)
            {
                item.item.quantity += 1;
                item.UpdateText();
                return;
            }
        }
        ItemObject newItemObject = Instantiate(Player.instance.inventory.itemObjectPrefab, itemsHolder);

        newItemObject.itemHolder = ItemHolder.Drop;
        newItemObject.InitItem(newItem);
        loot.Add(newItemObject);
        loot.Sort((a, b) => a.item.Name.CompareTo(b.item.Name));
        foreach (ItemObject item in loot)
        {
            item.transform.SetAsLastSibling();
        }


    }


    #endregion


    #endregion

    #region Town

    [Header("Town")]
    [SerializeField] private GameObject TownPanel;
    [SerializeField] private TextMeshProUGUI townNameText;
    [SerializeField] private NpcObject npcPrefab;
    [SerializeField] private Transform npcsHolder;

    private void GoToTown()
    {
        foreach (Transform child in npcsHolder)
            Destroy(child.gameObject);

        TownData townData = currentLocation as TownData;
        foreach (NPCData npc in townData.npcs)
        {
            NpcObject npcObject = Instantiate(npcPrefab, npcsHolder);
            npcObject.InitNPC(npc);
        }

        TownPanel.SetActive(true);

        Player.instance.SetRegeneration(true);
    }


    #endregion

    #region ChangeBackground

    public void ChangeBackground()
    {
        if (currentLocation != null)
        {
            Player.instance.combatController.background.sprite = currentLocation.locationImage;
            MonsterManager.instance.enemyController.background.sprite = currentLocation.locationImage;
            SoundManager.instance.PlayBackground(currentLocation);
        }
    }

    #endregion

    #region ResetOnDeath

    public void ResetOnDeath()
    {
        currentLocation = townData;
        ChangeBackground();
        WildernessPanel.SetActive(false);
        townNameText.text = townData.locationName;
        GoToTown();
    }

    #endregion

    #region Map
    [Header("Map")]

    [SerializeField] private List<LocationData> locationDatas = new List<LocationData>();
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private RectTransform locationsHolder;
    [SerializeField] private Button location;


    #region InitMap

    public void InitMap()
    {
        foreach (LocationData locationData in locationDatas)
        {
            Button button = Instantiate(location, locationsHolder);
            button.GetComponentInChildren<TextMeshProUGUI>().text = locationData.locationName;
            button.onClick.AddListener(() => DisplayLocationInfo(locationData));

            button.GetComponent<RectTransform>().anchoredPosition = locationData.position;
        }
    }

    #endregion

    #region DisplayMap

    public void DisplayMap()
    {
        if (mapPanel.activeSelf)
        {
            mapPanel.SetActive(false);
        }
        else
        {
            mapPanel.SetActive(true);
        }
        ResetInfo();
    }

    #endregion



    #endregion


    #region DisablePanels

    public void DisablePanels()
    {
        WildernessPanel.SetActive(false);
        TownPanel.SetActive(false);
    }

    #endregion










    #region DisplayLocationInfo

    [Header("LocationInfo")]
    [SerializeField] private Button TravelButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI travelTimeText;


    [Header("Monsters")]
    [SerializeField] private Transform monsterDisplay;
    [SerializeField] private Transform monsterHolder;
    [SerializeField] private GameObject mapMonsterPrefab;

    [Header("Town")]
    [SerializeField] private Transform townHolder;

    public void DisplayLocationInfo(LocationData locationData)
    {
        monsterDisplay.gameObject.SetActive(false);
        townHolder.gameObject.SetActive(false);

        nameText.text = locationData.locationName;

        float distance = Vector2.Distance(currentLocation.position, locationData.position);
        float travelTime = distance / (Player.instance.travelSpeed / 10);
        int hours = (int)travelTime / 3600;
        int minutes = (int)(travelTime % 3600) / 60;
        int seconds = (int)travelTime % 60;
        travelTimeText.text = "Travel time: <color=#BD9B79>" +
        (hours > 0 ? hours + " hour" + (hours > 1 ? "s" : "") + ", " : "") +
        (minutes > 0 ? minutes + " minute" + (minutes > 1 ? "s" : "") + ", " : "") +
        (seconds > 0 ? seconds + " second" + (seconds > 1 ? "s" : "") : "") +
        "</color>";


        foreach (Transform child in monsterHolder)
            Destroy(child.gameObject);

        if (locationData is WildernessData wildernessData)
        {
            foreach (MonsterData monster in wildernessData.monstersInArea)
            {
                MapMonster mapMonster = Instantiate(mapMonsterPrefab, monsterHolder).GetComponent<MapMonster>();
                mapMonster.InitMonster(monster);
            }
            monsterDisplay.gameObject.SetActive(true);
        }

        TravelButton.onClick.AddListener(() => TravelToLocation(locationData));
    }

    #region ResetInfo

    public void ResetInfo()
    {
        nameText.text = "__";
        travelTimeText.text = "Travel time: ";
    }

    #endregion

    #endregion




    #region Start

    public void Start()
    {
        InitMap();
    }

    #endregion
}