using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Game
{
  public class ShoppingScreen : UIScreen
  {
    [SerializeField] private Button shopScreenButton;
    
    private void Awake()
    {
       shopScreenButton.onClick.AddListener(() => G.UIManager.ShowScreen("ShopScreen"));
    }
  }
}