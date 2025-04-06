using Unity.VisualScripting;
using UnityEngine;
using IState = Game.Scripts.StateMachine.IState;

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
        G.Player.GetComponent<Animator>().SetInteger("State",1);
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