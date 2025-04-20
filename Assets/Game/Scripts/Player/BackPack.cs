using UnityEngine;
using UnityEngine.UI;

public class BackPack : MonoBehaviour
{
    [SerializeField] private int maxGold = 5;
    [SerializeField] private int currentGold = 0;

    [Header("Gold Slots")] [SerializeField]
    private Image goldSlot1Image;

    [SerializeField] private Image goldSlot2Image;
    [SerializeField] private Image goldSlot3Image;
    [SerializeField] private Image goldSlot4Image;
    [SerializeField] private Image goldSlot5Image;

    private Color defaultColor = new Color(0.47f, 0.47f, 0.47f);
    private Color fullColor = Color.white;

    public int MaxGold => maxGold;
    public int CurrentGold => currentGold;

    public void Awake()
    {
        G.BackPack = this;

        ResetSlotColors();
    }

    private void Update()
    {
        if (IsFull())
        {
            G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Carrying);
        }
    }

    public void AddGold(int amount)
    {
        int previousGold = currentGold;
        currentGold += amount;
        if (currentGold > maxGold)
        {
            currentGold = maxGold;
        }

        UpdateSlotColors();
    }

    public void ResetGold()
    {
        currentGold = 0;
        ResetSlotColors();
    }

    public bool IsFull()
    {
        return currentGold >= maxGold;
    }
    
    public bool IsEmpty()
    {
        return currentGold <= 0;
    }

    private void UpdateSlotColors()
    {
        ResetSlotColors();

        if (currentGold >= 1) goldSlot1Image.color = fullColor;
        if (currentGold >= 2) goldSlot2Image.color = fullColor;
        if (currentGold >= 3) goldSlot3Image.color = fullColor;
        if (currentGold >= 4) goldSlot4Image.color = fullColor;
        if (currentGold >= 5) goldSlot5Image.color = fullColor;
    }

    private void ResetSlotColors()
    {
        goldSlot1Image.color = defaultColor;
        goldSlot2Image.color = defaultColor;
        goldSlot3Image.color = defaultColor;
        goldSlot4Image.color = defaultColor;
        goldSlot5Image.color = defaultColor;
    }
}