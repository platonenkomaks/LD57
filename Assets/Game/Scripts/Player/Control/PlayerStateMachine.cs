using Events;
using Game.Scripts.StateMachine;
using UnityEngine;


public class PlayerStateMachine : StateMachine
{
    public enum PlayerState
    {
        Mining,
        Carrying,
        Fighting
    }

    public PlayerState CurrentState { get; private set; }

    public static SpriteRenderer PlayerSpriteRenderer;
    public static Sprite MiningGoldSprite;
    public static Sprite CarryingGoldSprite;
    public static Sprite FightingSprite;


    private readonly MiningPlayerState _miningPlayerState;
    private readonly CarryingPlayerState _carryingPlayerState;
    private readonly FightingPlayerState _fightingPlayerState;

    public PlayerStateMachine(
        SpriteRenderer playerSpriteRenderer,
        Sprite miningGoldSprite,
        Sprite carryingGoldSprite,
        Sprite fightingSprite
    )
    {
        
        G.PlayerStateMachine = this;
        PlayerSpriteRenderer = playerSpriteRenderer;
        MiningGoldSprite = miningGoldSprite;
        CarryingGoldSprite = carryingGoldSprite;
        FightingSprite = fightingSprite;
        
        _miningPlayerState = new(G.PlayerController, PlayerSpriteRenderer, MiningGoldSprite);
        _carryingPlayerState = new();
        _fightingPlayerState = new(G.PlayerController,PlayerSpriteRenderer);
    }


    public void SetState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Mining:
                ChangeState(_miningPlayerState);
                CurrentState = PlayerState.Mining;
                break;
            case PlayerState.Carrying:
                ChangeState(_carryingPlayerState);
                CurrentState = PlayerState.Carrying;
                break;
            case PlayerState.Fighting:
                ChangeState(_fightingPlayerState);
                CurrentState = PlayerState.Fighting;
                break;
        }

        G.EventManager.Trigger(new OnPlayerStateChangeEvent()
        {
            State = newState
        });
    }
}