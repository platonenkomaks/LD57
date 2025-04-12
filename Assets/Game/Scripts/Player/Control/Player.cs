using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;


public class Player : MonoBehaviour
{
    [field: SerializeField] public BatteryLight BatteryLight { get; private set; } 
    
    [Header("Player States Sprites")] 
    public Sprite miningGoldSprite;
    public Sprite carryingGoldSprite;
    public Sprite fightingSprite;

    public SpriteRenderer playerSpriteRenderer;
    
    private IEnumerator Start()
    {
        yield return null;
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        
       
        
        G.PlayerStateMachine = new PlayerStateMachine(
            playerSpriteRenderer,
            miningGoldSprite,
            carryingGoldSprite,
            fightingSprite
        );
        
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
        G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChange);
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChange);
    }

    private void OnGameStateChange(OnGameStateChangedEvent e)
    {
        if (e.State == GameLoopStateMachine.GameLoopState.Ascend)
        {
            G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Fighting);
        }
        else
        {
            G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
        }
    }
}