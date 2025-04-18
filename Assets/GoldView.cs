using UnityEngine;

public class GoldView : MonoBehaviour
{
    public GameObject goldPile1;
    public GameObject goldPile2;
    public GameObject goldPile3;
    public GameObject goldPile4;
    public GameObject goldPile5;

    private GameObject[] _goldPiles;

    private void Awake()
    {
        G.GoldPilesView = this;
        _goldPiles = new GameObject[] { goldPile1, goldPile2, goldPile3, goldPile4, goldPile5 };
    }

    private void Start()
    {
        SetEnabled(false);
        G.ElevatorPlatform.GetComponent<PlatformWeight>().OnWeightChange += UpdateGoldBalance;

        UpdateGoldBalance();
    }

    private void OnDestroy()
    {
        if (G.GoldManager == null) return;
        G.ElevatorPlatform.GetComponent<PlatformWeight>().OnWeightChange -= UpdateGoldBalance;
    }

    private void UpdateGoldBalance()
    {
        int goldBalance = G.ElevatorPlatform.GetComponent<PlatformWeight>().goldOnPlatformBalance;

        for (int i = 0; i < _goldPiles.Length; i++)
        {
            _goldPiles[i].SetActive(false);
        }

        if (goldBalance > 0 && goldBalance <= _goldPiles.Length)
        {
            _goldPiles[goldBalance - 1].SetActive(true);
        }
    }
    public void SetEnabled(bool enabled)
    {
        for (int i = 0; i < _goldPiles.Length; i++)
        {
            _goldPiles[i].SetActive(enabled);
        }
    }
    
}