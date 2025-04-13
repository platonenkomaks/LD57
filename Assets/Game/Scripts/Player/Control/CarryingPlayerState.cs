using Game.Scripts.StateMachine;
using UnityEngine;

public class CarryingPlayerState : IState
{
    public void Enter()
    {
        G.AudioManager.Play("BagGold");
        Debug.Log("Entering Carrying State");
        G.Player.GetComponent<Animator>().SetInteger("State", 2);
    }

    public void Execute()
    {
       
    }

    public void Exit()
    {
       
    }
}