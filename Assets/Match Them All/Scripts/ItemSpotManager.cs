using System;
using UnityEngine;

public class ItemSpotManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform itemSpot;

    [Header("Settings")] 
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;
    

    
    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;
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
        // 1. Turn item as a child of item spot
        item.transform.SetParent(itemSpot);
        
        // 2. Scale item down, set local pos 0,0,0
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;
        
        // 3. Disable shadow
        item.DisableShadows();
        
        // 4. Disable collider / physics
        item.DisablePhysics();
    }
}
