using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor.Overlays;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [Header("Vacuum Elements")] 
    [SerializeField] private Vacuum vacuum;
    [SerializeField] private Transform vacuumSuckPosition;

    [Header("Settings")]
    private bool isBusy;
    private int vacuumItemsToCollect;
    private int vacuumCounter;
    
    [Header("Actions")] 
    public static Action<Item> itemPickedup;

    [Header("Data")] 
    [SerializeField] private int initialPUCount;
    private int vacuumPUCount;

    private void Awake()
    {
        LoadData();
        
        Vacuum.started += OnVacuumStarted;
        InputManager.powerupClicked += OnPowerupClicked;
    }

    private void OnDestroy()
    {
        Vacuum.started -= OnVacuumStarted;
        InputManager.powerupClicked -= OnPowerupClicked;
    }

    private void OnPowerupClicked(Powerup powerup)
    {
        if (isBusy)
            return;

        switch (powerup.Type)
        {
            case EPowerupType.Vacuum:
                HandleVacuumClicked();
                UpdateVacuumVisual();
                break;
        }
    }

    private void HandleVacuumClicked()
    {
        if (vacuumPUCount <= 0)
        {
            vacuumPUCount = 3;
            SaveData();
        }
        else
        {
            isBusy = true;

            vacuumPUCount--;
            SaveData();
            
            vacuum.Play();
        }
    }

    private void OnVacuumStarted()
    {
        VacuumPowerup();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    private void VacuumPowerup()
    {
        // Collect 3 target / goal items
        
        // Grab the items
        
        // Grab the goal items
        
        // Grab the goal with greatest amount
        // Grab 3 items

        Item[] items = LevelManager.instance.Items;
        ItemLevelData[] goals = GoalManager.instance.Goals;

        ItemLevelData? greatestGoal = GetGreatestGoal(goals);
        
        if (greatestGoal == null)
            return;

        ItemLevelData goal = (ItemLevelData)greatestGoal;
        
        vacuumCounter = 0;
        
        List<Item> itemsToCollect = new List<Item>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;
            
            if (items[i].ItemName == goal.itemPrefab.ItemName)
            {
                itemsToCollect.Add(items[i]);
                
                if(itemsToCollect.Count >= 3)
                    break;
            }
        }

        vacuumItemsToCollect = itemsToCollect.Count;

        for (int i = 0; i < itemsToCollect.Count; i++)
        {
            itemsToCollect[i].DisablePhysics();

            Item itemToCollect = itemsToCollect[i];

            List<Vector3> points = new List<Vector3>();

            points.Add(itemsToCollect[i].transform.position);
            points.Add(itemsToCollect[i].transform.position);
            
            points.Add(itemsToCollect[i].transform.position + Vector3.up * 2);
            
            points.Add(vacuumSuckPosition.position + Vector3.up * 2);
            
            points.Add(vacuumSuckPosition.position);
            points.Add(vacuumSuckPosition.position);

            LeanTween.moveSpline(itemsToCollect[i].gameObject, points.ToArray(), .75f)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));
            
            LeanTween.scale(itemsToCollect[i].gameObject, Vector3.zero, .75f);
        }
        
        for (int i = itemsToCollect.Count-1; i >= 0; i--)
        {
            itemPickedup?.Invoke(itemsToCollect[i]);   
            //Destroy(itemsToCollect[i].gameObject);
        }
    }

    private void ItemReachedVacuum(Item item)
    {
        vacuumCounter++;

        if (vacuumCounter >= vacuumItemsToCollect)
            isBusy = false;
        
        Destroy(item.gameObject);
    }

    private ItemLevelData? GetGreatestGoal(ItemLevelData[] goals)
    {
        int max = 0;
        int goalIndex = -1;

        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].amount >= max)
            {
                max = goals[i].amount;
                goalIndex = i;
            }
        }

        if (goalIndex <= -1)
            return null;
        
        return goals[goalIndex];
    }

    private void UpdateVacuumVisual()
    {
        vacuum.UpdateVisuals(vacuumPUCount);
    }
    
    private void LoadData()
    {
        vacuumPUCount = PlayerPrefs.GetInt("VacuumCount", initialPUCount);

        UpdateVacuumVisual();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("VacuumCount", vacuumPUCount);
    }
    
}
