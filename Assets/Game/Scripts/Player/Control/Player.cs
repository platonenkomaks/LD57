using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("Player States Sprites")] 
    public Sprite miningGoldSprite;
    public Sprite carryingGoldSprite;
    public Sprite fightingSprite;

    public SpriteRenderer playerSpriteRenderer;


    private void Awake()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        
        G.PlayerStateMachine = new PlayerStateMachine(
            playerSpriteRenderer,
            miningGoldSprite,
            carryingGoldSprite,
            fightingSprite
        );
        
      
    }
    private void Start ()
    {
        G.Player = this;
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
    }
    
}