using Game.Scripts.StateMachine;
using UnityEngine;

public class MiningPlayerState : IState
{
    private readonly SpriteRenderer _playerSpriteRenderer;
    private readonly Sprite _miningGoldSprite;


    public MiningPlayerState(
        SpriteRenderer playerSpriteRenderer,
        Sprite miningGoldSprite
    )
    {
        _playerSpriteRenderer = playerSpriteRenderer;
        _miningGoldSprite = miningGoldSprite;
    }


    public void Enter()
    {
        Debug.Log("Entering Mining State");
        
        if (_playerSpriteRenderer == null)
        {
            Debug.LogError("PlayerSpriteRenderer is null");
            return;
        }
        if (_miningGoldSprite == null)
        {
            Debug.LogError("MiningGoldSprite is null");
            return;
        }
        _playerSpriteRenderer.sprite = _miningGoldSprite;
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}