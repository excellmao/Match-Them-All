using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private ItemPlacer itemPlacer;

    public ItemLevelData[] GetGoals() => itemPlacer.GetGoals();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
