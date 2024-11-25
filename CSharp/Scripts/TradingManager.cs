using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Trade Properties")]
    public List<Item> playerItems = new List<Item>();
    public List<Item> npcItems = new List<Item>();

    public bool isTrading = false;

    [Header("Sounds")]
    public List<AudioClip> buySounds = new List<AudioClip>();

    public static System.Action<Item> OnBuy;

    public void StartTrade(List<Item> items)
    {
        npcItems = items;
        isTrading = true;
    }

    public void EndTrade()
    {
        isTrading = false;
    }

    public void BuyItem(ItemObject item, int amount)
    {
        if (npcItems.Contains(item.item) && player.inventory.marks >= item.item.value)
        {
            player.inventory.AddItem(item.item, amount);
            player.inventory.UpdateMarks(-item.item.value * amount);
            SoundManager.instance.PlayUISound(buySounds[Random.Range(0, buySounds.Count)]);
            if (item.item.Name.Contains("Life Potion") && OnBuy != null) OnBuy(item.item);
        }
        else
        {
            print("Not enough money");
        }
    }

    public void SellItem(ItemObject item, int amount)
    {
        player.inventory.RemoveItem(item, amount);
        player.inventory.UpdateMarks(item.item.value * amount);
        SoundManager.instance.PlayUISound(buySounds[Random.Range(0, buySounds.Count)]);

    }


    Player player;
    private void Start()
    {
        player = Player.instance;
    }
}
