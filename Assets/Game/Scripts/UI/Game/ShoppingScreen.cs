using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
  public class ShoppingScreen : UIScreen
  {
    [SerializeField] private Button shopScreenButton;

    protected override void Start()
    {
      base.Start();
      shopScreenButton.onClick.AddListener(() => G.UIManager.ShowScreen("ShopScreen"));
    }
  }
}