using Game.Scripts.StateMachine;
using UnityEngine;

public class FightingPlayerState : IState
{
    public SpriteRenderer PlayerSpriteRenderer; 

    public FightingPlayerState(SpriteRenderer playerSpriteRenderer)
    {
        PlayerSpriteRenderer = playerSpriteRenderer;
    }


    public void Enter()
    {
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}