using System.Diagnostics;

namespace Game.Scripts.StateMachine.GameLoop
{
  public class ShoppingState : IState
  {
    public void Enter() {
     G.UIManager.ShowScreen("Shopping");
    }

    public void Execute() {}

    public void Exit() {}
  }
}