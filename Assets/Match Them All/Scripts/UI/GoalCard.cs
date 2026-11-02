using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GoalCard : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private TextMeshProUGUI amountText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Configure(int initialAmount)
    {
        amountText.text = initialAmount.ToString();
    }

    public void UpdateAmount(int amount)
    {
        amountText.text = amount.ToString();
    }

    public void Complete()
    {
        gameObject.SetActive(false);
    }
    
}
