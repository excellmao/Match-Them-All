using UnityEngine;
using System;
using System.Xml;

public class InputManager : MonoBehaviour
{
    public static Action<Item> itemClicked;

    [Header("Settings")] 
    [SerializeField] private Material outlineMaterial;
    private Item currentItem;
    

    void Start()
    {
        
    }
    
    void Update()
    {
        if (Input.GetMouseButton(0))
            HandleDrag();
        else if (Input.GetMouseButtonUp(0))
            HandleMouseUp();
    }

    private void HandleDrag()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
        {
            DeselectCurrentItem();
            return;
        }

        Item item = hit.collider.GetComponentInParent<Item>();
        
        if (item == null)
        {
            DeselectCurrentItem();
            return;
        }
        
        if (item == currentItem)
            return;
        
        DeselectCurrentItem();
        
        currentItem = item;
        currentItem.Select(outlineMaterial);
    }

    private void DeselectCurrentItem()
    {
        if(currentItem != null)
            currentItem.Deselect();

        currentItem = null;
    }

    private void HandleMouseUp()
    {
        if(currentItem == null)
            return;
        
        currentItem.Deselect();
        
        itemClicked?.Invoke(currentItem);
        currentItem = null;
    }
}
