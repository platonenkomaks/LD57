using Game.Scripts.StateMachine;
using UnityEngine;

public class FightingPlayerState : IState
{
    private readonly PlayerController _playerController;
    private readonly SpriteRenderer _playerSpriteRenderer;

    public FightingPlayerState(PlayerController playerController, SpriteRenderer playerSpriteRenderer)
    {
        _playerSpriteRenderer = playerSpriteRenderer;
        _playerController = playerController;
    }


    public void Enter()
    {
        G.Player.GetComponent<Animator>().SetInteger("State", 3);
        _playerController.EnableCombatMode();
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        _playerSpriteRenderer.sprite = PlayerStateMachine.MiningGoldSprite;
    }
}