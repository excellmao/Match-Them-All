using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

public class ItemSpotManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform itemSpotsParent;
    private ItemSpot[] spots;

    [Header("Settings")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;
    private bool isBusy;

    [Header("Data")]
    private Dictionary<EItemName, ItemMergeData> itemMergeDataDictionnary = new Dictionary<EItemName, ItemMergeData>();
    

    
    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;

        StoreSpots();
    }

    private void OnDestroy()
    {
        InputManager.itemClicked -= OnItemClicked;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnItemClicked(Item item)
    {
        if (isBusy)
        {
            Debug.LogWarning("ItemSpotsManager is busy...");
            return;
        }
        
        // Free spot check
        if (!IsFreeSpotAvailable())
        {
            Debug.LogWarning("No Free spot! Game Over :(");
            return;
        }

        isBusy = true;

        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        if (itemMergeDataDictionnary.ContainsKey(item.ItemName))
        {
            HandleItemMergeDataFound(item);
        }
        else
        {
            MoveItemToFirstFreeSpot(item);
        }
    }

    private void HandleItemMergeDataFound(Item item)
    {
        ItemSpot idealSpot = GetIdealSpotFor(item);
        
        itemMergeDataDictionnary[item.ItemName].Add(item);

        TryMoveItemToIdealSpot(item, idealSpot);
    }

    private ItemSpot GetIdealSpotFor(Item item)
    {
        List<Item> items = itemMergeDataDictionnary[item.ItemName].items;
        List<ItemSpot> itemSpots = new List<ItemSpot>();

        for (int i = 0; i < items.Count; i++)
        {
            itemSpots.Add(items[i].Spot);
        }
        
        // We have a list of occupied spots but the items similar to item
        
        // If we only have one spot -> grab spot next to it
        if (itemSpots.Count >= 2)
        {
            itemSpots.Sort((a, b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex()));
        }

        int idealSpotIndex = itemSpots[0].transform.GetSiblingIndex() + 1;

        return spots[idealSpotIndex];
    }
    
    private void TryMoveItemToIdealSpot(Item item, ItemSpot targetSpot)
    {
        if (!targetSpot.IsEmpty())
        {
            HandleIdealSpotFull(item, targetSpot);
            return;
        }
        
        MoveItemToSpot(item, targetSpot);
    }

    private void MoveItemToSpot(Item item, ItemSpot targetSpot)
    {
        // 1. Turn item as a child of item spot
        targetSpot.Populate(item);
        
        // 2. Scale item down, set local pos 0,0,0
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;
        
        // 3. Disable shadow
        item.DisableShadows();
        
        // 4. Disable collider / physics
        item.DisablePhysics();

        HandleItemReachedSpot(item);
    }

    private void HandleItemReachedSpot(Item item)
    {
        if (itemMergeDataDictionnary[item.ItemName].CanMergeItems())
        {
            MergeItems(itemMergeDataDictionnary[item.ItemName]);
        } else CheckForGameOver();
    }

    private void MergeItems(ItemMergeData itemMergeData)
    {
        List<Item> items = itemMergeData.items;
        
        // Remove ItemMergeData from dictionary
        itemMergeDataDictionnary.Remove(itemMergeData.itemName);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Spot.Clear();
            Destroy(items[i].gameObject);
        }
        
        //TODO: Remove this line after moving items to left
        isBusy = false;
    }

    private void HandleIdealSpotFull(Item item, ItemSpot targetSpot)
    {
        throw new NotImplementedException();
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetSpot = GetFreeSpot();

        if (targetSpot == null)
        {
            Debug.LogError("Target spot is null => This should not happen");
            return;
        }

        CreateItemMergeData(item);
        
        // 1. Turn item as a child of item spot
        targetSpot.Populate(item);
        
        // 2. Scale item down, set local pos 0,0,0
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;
        
        // 3. Disable shadow
        item.DisableShadows();
        
        // 4. Disable collider / physics
        item.DisablePhysics();

        HandleFirstItemReachedSpot(item);
    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (GetFreeSpot() == null)
        {
            Debug.LogWarning("Gameover :(");
        }
        else
        {
            isBusy = false;
        }
    }

    private void CreateItemMergeData(Item item)
    {
        itemMergeDataDictionnary.Add(item.ItemName, new ItemMergeData(item));
    }

    private void StoreSpots()
    {
        spots = new ItemSpot[itemSpotsParent.childCount];
        for (int i = 0; i < itemSpotsParent.childCount; i++)
        {
            spots[i] = itemSpotsParent.GetChild(i).GetComponent<ItemSpot>();
        }
    }

    private ItemSpot GetFreeSpot()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return spots[i];
        }

        return null;
    }

    private bool IsFreeSpotAvailable()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return true;
        }

        return false;
    }
}
