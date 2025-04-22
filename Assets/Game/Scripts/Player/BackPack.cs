using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Events;

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

    [Header("Pop-up")]
    [SerializeField] private GameObject popUpBackpackIsFull;
    
    [Header("Animation Settings")]
    [SerializeField] private float scaleAmount = 1.5f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;
    
    private Color defaultColor = new Color(0.47f, 0.47f, 0.47f);
    private Color fullColor = Color.white;
    
    public int MaxGold => maxGold;
    public int CurrentGold => currentGold;

    public void Awake()
    {
        G.BackPack = this;
        popUpBackpackIsFull.SetActive(false);
        ResetSlotColors();
        G.EventManager.Register<OnPlayerRespawn>(OnPlayerRespawn);
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnPlayerRespawn>(OnPlayerRespawn);
        G.BackPack = null;
    }

    private void Update()
    {
        if (IsFull())
        {
            if (G.BackPack.GetComponent<Tooltip>() != null)
            {
                G.BackPack.GetComponent<Tooltip>().HideTooltip();
                popUpBackpackIsFull.SetActive(true);
            }
           
            G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Carrying);
        }
        else 
        {
            popUpBackpackIsFull.SetActive(false);
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
        G.AudioManager.Play("Interact");
        UpdateSlotColors(previousGold);
    }

    public void ResetGold()
    {
        currentGold = 0;
        popUpBackpackIsFull.SetActive(false);
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

    private void UpdateSlotColors(int previousGold)
    {
        ResetSlotColors();

        if (currentGold >= 1) 
        {
            goldSlot1Image.color = fullColor;
            if (previousGold < 1) AnimateSlot(goldSlot1Image);
        }
        
        if (currentGold >= 2) 
        {
            goldSlot2Image.color = fullColor;
            if (previousGold < 2) AnimateSlot(goldSlot2Image);
        }
        
        if (currentGold >= 3) 
        {
            goldSlot3Image.color = fullColor;
            if (previousGold < 3) AnimateSlot(goldSlot3Image);
        }
        
        if (currentGold >= 4) 
        {
            goldSlot4Image.color = fullColor;
            if (previousGold < 4) AnimateSlot(goldSlot4Image);
        }
        
        if (currentGold >= 5) 
        {
            goldSlot5Image.color = fullColor;
            if (previousGold < 5) AnimateSlot(goldSlot5Image);
        }
    }
    
    private void OnPlayerRespawn(OnPlayerRespawn _)
    {
        ResetGold();
    }

    private void ResetSlotColors()
    {
        goldSlot1Image.color = defaultColor;
        goldSlot2Image.color = defaultColor;
        goldSlot3Image.color = defaultColor;
        goldSlot4Image.color = defaultColor;
        goldSlot5Image.color = defaultColor;
    }
    
    private void AnimateSlot(Image slotImage)
    {
       
        Vector3 originalScale = slotImage.transform.localScale;
        
        Sequence scaleSequence = DOTween.Sequence();
        
        scaleSequence.Append(slotImage.transform.DOScale(originalScale * scaleAmount, animationDuration / 2)
            .SetEase(easeType));
        
        scaleSequence.Append(slotImage.transform.DOScale(originalScale, animationDuration / 2)
            .SetEase(Ease.OutQuad));
        
        scaleSequence.Play();
    }
}