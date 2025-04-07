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
        _goldPiles = new GameObject[] { goldPile1, goldPile2, goldPile3, goldPile4, goldPile5 };
    }

    private void Start()
    {
        SetEnabled(false);
        G.GoldManager.OnGoldBalanceChange += UpdateGoldBalance;

        UpdateGoldBalance();
    }

    private void OnDestroy()
    {
        G.GoldManager.OnGoldBalanceChange -= UpdateGoldBalance;
    }

    private void UpdateGoldBalance()
    {
        int goldBalance = G.GoldManager.GoldBalance;

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